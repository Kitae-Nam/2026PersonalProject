using System.Collections.Generic;
using _01_Script.Pool;
using UnityEngine;

namespace _01_Script.Map
{
    public class MapManage : MonoBehaviour
    {
        [Header("참조")] 
        [SerializeField] private PoolManager poolManager;
        [SerializeField] private Transform wallParent; // 벽 콜라이더를 담을 부모(없으면 자동 생성)
        [SerializeField] private Transform   player;

        [Header("풀 키")] 
        [SerializeField] private string grassKey = "GrassBlock";
        [SerializeField] private string treeKey = "Tree";
        [SerializeField] private string rockKey = "Rock";
        [SerializeField] private string stationKey = "Station"; // 역 본체 프리팹

        [Header("그리드 / 블록 설정")] 
        [SerializeField] private float gridSize = 1f; // 그리드 한 눈금 크기
        [SerializeField] private int gridPerBlock = 4; // 그리드 4칸당 블록 1개
        [SerializeField] private int depthBlocks = 15; // Z폭 = 블록 15개 (고정)
        [SerializeField] private int chunkBlocks = 16; // 청크 1개 X길이(블록)
        [SerializeField] private float groundY = 0f;
        [SerializeField] private float resourceY = 0f;

        [Header("맵 길이 (청크 수) — 인스펙터에서 조절")] 
        public int totalChunks = 6; // 시작~정거장 포함 전체 청크 수

        [Header("역 주변 자원 제거 반경 (블록 단위)")]
        [SerializeField] private int randomZ = 5;
        [SerializeField] private int clearRadiusX = 2;
        [SerializeField] private int clearRadiusZ = 2;
        
        [Header("플레이어 시작 주변 자원 제거 반경 (블록 단위)")]
        [SerializeField] private int playerClearRadiusX = 2;
        [SerializeField] private int playerClearRadiusZ = 2;

        [Header("바이옴 노이즈 (숲 vs 광산)")]
        [SerializeField] private float biomeScale = 0.12f;
        [SerializeField] private float biomeThreshold = 0.5f;
        [SerializeField] private int biomeSeed = 1000;

        [Header("자원 노이즈 (군집 vs 평원)")] 
        [SerializeField] private float resourceScale = 0.18f;
        [SerializeField] private float resourceThreshold = 0.62f;
        [SerializeField] private int resourceSeed = 2000;

        [Header("경계 벽 콜라이더")] 
        [SerializeField] private float wallHeight = 10f; // 벽 높이
        [SerializeField] private float wallThickness = 1f; // 벽 두께
        [SerializeField] private LayerMask wallLayer;
        
        [Header("바이옴 균형 보정 (공정성)")]
        [SerializeField] private bool balanceBiomes  = true;
        [SerializeField] private int  maxGapChunks    = 2;   // 한 자원이 연속으로 안 나와도 되는 최대 청크 수
        [SerializeField] [Range(0f,0.5f)] private float minBiomeRatio    = 0.2f;  // 청크의 최소 20%는 각 바이옴이어야 "충분"으로 인정
        [SerializeField] private int   minPatchBlocks = 8;
        
        [Header("노이즈 부드럽게 (fBm 옥타브)")]
        [SerializeField] private int   octaves     = 4;     // 겹칠 노이즈 층 수 (많을수록 디테일)
        [SerializeField] private float persistence = 0.5f;  // 층마다 진폭 감소율
        [SerializeField] private float lacunarity  = 2f;    // 층마다 주파수 증가율

        // 플레이어 시작 블록 좌표 (Start에서 계산)
        private int _playerBlockX;
        private int _playerBlockZ;

// 마지막으로 숲/광산이 "충분히" 나온 청크와의 간격 추적
        private int _gapSinceForest;  // 숲이 안 나온 연속 청크 수
        private int _gapSinceMine;    // 광산이 안 나온 연속 청크 수

        private float BlockSize => gridPerBlock * gridSize;

        // Z 인덱스 범위: 0 기준 앞뒤 대칭. 15 → -7 ~ +7
        private int ZMin => -(depthBlocks / 2);
        private int ZMax => (depthBlocks - 1) / 2;

        // 정거장은 마지막 청크
        private int StationChunk => _baseChunk + (totalChunks - 1);

        private readonly List<GameObject> _spawned = new();
        private GameObject _wallRoot;
        private int _baseChunk; 

        private void Start()
        {
            if (poolManager == null) poolManager = PoolManager.Instance;
            biomeSeed = Random.Range(0, 99999);
            resourceSeed =  Random.Range(0, 99999);
            
            if (player != null)
            {
                _baseChunk    = Mathf.FloorToInt(player.position.x / (chunkBlocks * BlockSize));
                // 플레이어가 선 블록 인덱스 (자원 제거 중심)
                _playerBlockX = Mathf.FloorToInt(player.position.x / BlockSize);
                _playerBlockZ = Mathf.RoundToInt(player.position.z / BlockSize);
            }
            else { _baseChunk = 0; _playerBlockX = 0; _playerBlockZ = 0; }
            
            GenerateMap();
            BuildBoundaryWalls();
        }

// 여러 옥타브를 겹친 부드러운 펄린 (0~1 정규화)
        private float FBM(int bx, int bz, float scale, int seed)
        {
            float amp = 1f, freq = 1f;
            float sum = 0f, ampSum = 0f;

            for (int o = 0; o < octaves; o++)
            {
                float nx = (bx + seed) * scale * freq;
                float nz = (bz + seed) * scale * freq;
                sum    += Mathf.PerlinNoise(nx, nz) * amp;
                ampSum += amp;
                amp    *= persistence;
                freq   *= lacunarity;
            }
            return Mathf.Clamp01(sum / ampSum); // 진폭 합으로 나눠 0~1 유지
        }

        // ── 맵 전체 1회 생성 ─────────────────────────────────────────────
        private void GenerateMap()
        {
            _gapSinceForest = 0;
            _gapSinceMine   = 0;
            for (int i = 0; i < totalChunks; i++)
                SpawnChunk(_baseChunk + i);
        }

        private void SpawnChunk(int chunkX)
        {
            
            int startBX = chunkX * chunkBlocks;
            bool stationChunk = (chunkX == StationChunk);
            int stationCenterBX = startBX + chunkBlocks / 2;

            // 1) 이 청크의 바이옴 맵을 먼저 계산 (true = 광산/돌, false = 숲/잔디)
            int zCount = ZMax - ZMin + 1;
            bool[,] isMineMap = new bool[chunkBlocks, zCount];
            int mineCount = 0, forestCount = 0;

            for (int lbx = 0; lbx < chunkBlocks; lbx++)
                for (int zi = 0; zi < zCount; zi++)
                {
                    int bx = startBX + lbx;
                    int bz = ZMin + zi;
                    bool mine = FBM(bx, bz, biomeScale, biomeSeed) >= biomeThreshold;
                    isMineMap[lbx, zi] = mine;
                    if (mine) mineCount++; else forestCount++;
                }

            // 2) 균형 보정: 한쪽이 maxGapChunks 넘게 안 나왔으면 강제로 패치 심기
            if (balanceBiomes && !stationChunk)
            {
                int totalCells   = chunkBlocks * zCount;
                int minCells     = Mathf.CeilToInt(totalCells * minBiomeRatio);

                bool forestEnough = forestCount >= minCells;
                bool mineEnough   = mineCount   >= minCells;

                _gapSinceForest = forestEnough ? 0 : _gapSinceForest + 1;
                _gapSinceMine   = mineEnough   ? 0 : _gapSinceMine   + 1;

                // 숲이 부족하고 gap 초과 → 숲을 minCells까지 끌어올림
                if (_gapSinceForest > maxGapChunks)
                {
                    int need = minCells - forestCount;
                    if (need > 0) ForcePatch(isMineMap, zCount, makeMine: false,
                        Mathf.Max(need, minPatchBlocks));
                    _gapSinceForest = 0;
                }
                // 광산이 부족하고 gap 초과 → 광산을 minCells까지
                if (_gapSinceMine > maxGapChunks)
                {
                    int need = minCells - mineCount;
                    if (need > 0) ForcePatch(isMineMap, zCount, makeMine: true,
                        Mathf.Max(need, minPatchBlocks));
                    _gapSinceMine = 0;
                }
            }

            // 3) 확정된 바이옴 맵으로 실제 스폰
            for (int lbx = 0; lbx < chunkBlocks; lbx++)
                for (int zi = 0; zi < zCount; zi++)
                {
                    int bx = startBX + lbx;
                    int bz = ZMin + zi;
                    bool isMine = isMineMap[lbx, zi];

                    Vector3 blockPos = new Vector3(
                        (bx + 0.5f) * BlockSize, groundY, bz * BlockSize);

                    string groundKey =grassKey;
                    GameObject ground = poolManager.Spawn(groundKey, blockPos, Quaternion.identity);
                    if (ground != null) { _spawned.Add(ground); }

                    bool nearStation = stationChunk
                                       && Mathf.Abs(bx - stationCenterBX) <= clearRadiusX
                                       && Mathf.Abs(bz)                   <= clearRadiusZ;
                    // 플레이어 시작 주변 반경 (추가)
                    bool nearPlayer = Mathf.Abs(bx - _playerBlockX) <= playerClearRadiusX
                                      && Mathf.Abs(bz - _playerBlockZ) <= playerClearRadiusZ;

                    if (nearStation || nearPlayer) continue;

                    float resNoise = FBM(bx, bz, resourceScale, resourceSeed);
                    if (resNoise >= resourceThreshold)
                    {
                        Vector3 resPos = new Vector3(blockPos.x, resourceY, blockPos.z);
                        string resKey = isMine ? rockKey : treeKey;
                        GameObject res = poolManager.Spawn(resKey, resPos, Quaternion.identity);
                        if (res != null) _spawned.Add(res);
                    }
                }

            if (stationChunk)
            {
                Vector3 stationPos = new Vector3(
                    (stationCenterBX + 0.5f) * BlockSize, resourceY, Random.Range(-randomZ, randomZ));
                GameObject station = poolManager.Spawn(stationKey, stationPos, Quaternion.identity);
                if (station != null) _spawned.Add(station);
            }
        }

        // 바이옴 맵의 일부를 강제로 뒤집어 최소 minBlocks칸의 패치를 만든다.
        // resourceScale 노이즈가 가장 자원이 잘 뜨는 칸을 골라 자연스럽게 군집되도록.
        private void ForcePatch(bool[,] map, int zCount, bool makeMine, int count)
        {
            // 1) 씨앗: 뒤집어야 할 칸 중 자원 노이즈가 가장 높은 칸 하나
            int seedLbx = -1, seedZi = -1;
            float best = -1f;
            for (int lbx = 0; lbx < chunkBlocks; lbx++)
            for (int zi = 0; zi < zCount; zi++)
                if (map[lbx, zi] != makeMine)
                {
                    float score = FBM(lbx, ZMin + zi, resourceScale, resourceSeed);
                    if (score > best) { best = score; seedLbx = lbx; seedZi = zi; }
                }
            if (seedLbx < 0) return; // 뒤집을 칸 없음

            // 2) 씨앗에서 가까운 칸부터 count개를 뒤집어 한 덩어리로 확장 (BFS 유사)
            var open = new List<(int lbx, int zi)> { (seedLbx, seedZi) };
            var done = new HashSet<(int, int)>();
            int placed = 0;

            while (open.Count > 0 && placed < count)
            {
                // 씨앗에 가까운 순으로 처리 → 둥글게 뭉침
                open.Sort((a, b) =>
                {
                    int da = Mathf.Abs(a.lbx - seedLbx) + Mathf.Abs(a.zi - seedZi);
                    int db = Mathf.Abs(b.lbx - seedLbx) + Mathf.Abs(b.zi - seedZi);
                    return da.CompareTo(db);
                });

                var cur = open[0];
                open.RemoveAt(0);
                if (!done.Add(cur)) continue;

                if (map[cur.lbx, cur.zi] != makeMine)
                {
                    map[cur.lbx, cur.zi] = makeMine;
                    placed++;
                }

                // 상하좌우 이웃 추가
                AddNeighbor(open, done, cur.lbx + 1, cur.zi, zCount);
                AddNeighbor(open, done, cur.lbx - 1, cur.zi, zCount);
                AddNeighbor(open, done, cur.lbx, cur.zi + 1, zCount);
                AddNeighbor(open, done, cur.lbx, cur.zi - 1, zCount);
            }
        }

        private void AddNeighbor(List<(int, int)> open, HashSet<(int, int)> done,
            int lbx, int zi, int zCount)
        {
            if (lbx < 0 || lbx >= chunkBlocks || zi < 0 || zi >= zCount) return;
            if (done.Contains((lbx, zi))) return;
            open.Add((lbx, zi));
        }

        private float Perlin(int bx, int bz, float scale, int seed)
        {
            float nx = (bx + seed) * scale;
            float nz = (bz + seed) * scale;
            return Mathf.Clamp01(Mathf.PerlinNoise(nx, nz));
        }

        // ── 맵 4면 경계 벽 (BoxCollider) ─────────────────────────────────
        private void BuildBoundaryWalls()
        {
            // 맵 월드 범위 계산
            float minX = _baseChunk * chunkBlocks * BlockSize;
            float maxX = (_baseChunk + totalChunks) * chunkBlocks * BlockSize;
            float minZ = (ZMin) * BlockSize - BlockSize * 0.5f;
            float maxZ = (ZMax) * BlockSize + BlockSize * 0.5f;

            float midX = (minX + maxX) * 0.5f;
            float midZ = (minZ + maxZ) * 0.5f;
            float lenX = maxX - minX;
            float lenZ = maxZ - minZ;
            float wallMidY = groundY + wallHeight * 0.5f;

            if (wallParent == null)
            {
                var go = new GameObject("BoundaryWalls");
                go.transform.SetParent(transform, false);
                wallParent = go.transform;
                _wallRoot = go;
            }

            // 앞/뒤(Z 양끝): X방향으로 길게
            CreateWall("Wall_Zmin", new Vector3(midX, wallMidY, minZ - wallThickness * 0.5f),
                new Vector3(lenX + wallThickness * 2f, wallHeight, wallThickness));
            CreateWall("Wall_Zmax", new Vector3(midX, wallMidY, maxZ + wallThickness * 0.5f),
                new Vector3(lenX + wallThickness * 2f, wallHeight, wallThickness));
            // 좌/우(X 양끝): Z방향으로 길게
            CreateWall("Wall_Xmin", new Vector3(minX - wallThickness * 0.5f, wallMidY, midZ),
                new Vector3(wallThickness, wallHeight, lenZ));
            CreateWall("Wall_Xmax", new Vector3(maxX + wallThickness * 0.5f, wallMidY, midZ),
                new Vector3(wallThickness, wallHeight, lenZ));
        }

        private void CreateWall(string name, Vector3 center, Vector3 size)
        {
            var wall = new GameObject(name);
            wall.transform.SetParent(wallParent, false);
            wall.transform.position = center;
            wall.layer = (int)Mathf.Log(wallLayer.value, 2);
            var box = wall.AddComponent<BoxCollider>();
            box.size = size; // 보이지 않는 콜라이더 벽 (렌더러 없음)
        }
        // ── 맵 전체 정리: 풀 오브젝트 반환 + 벽 제거 ──────────────────────
        public void ClearMap()
        {
            // 1) 스폰된 모든 오브젝트를 풀로 반환 (스케일 원복 후)
            foreach (var go in _spawned)
            {
                if (go == null) continue;
                poolManager.Despawn(go);
            }
            _spawned.Clear();

            // 2) 경계 벽 제거 (풀 오브젝트가 아니므로 Destroy)
            if (_wallRoot != null)
            {
                Destroy(_wallRoot);
                _wallRoot   = null;
                wallParent  = null;        // 다음 BuildBoundaryWalls에서 새로 생성되도록
            }
        }

// ── 맵 리셋: 정리 후 재생성 (스테이지 전환 등에서 호출) ────────────
        public void RegenerateMap()
        {
            ClearMap();
            GenerateMap();
            BuildBoundaryWalls();
        }
    }
}