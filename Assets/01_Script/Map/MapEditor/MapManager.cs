using System.Collections.Generic;
using _01_Script.Pool;
using UnityEngine;

namespace _01_Script.Map.MapEditor
{
    /// <summary>
    /// 페인트로 그린 MapTile 들을 읽어 맵을 생성.
    /// - 타입별 풀: 첫 시작(Start) / 일반 필드(Field) / 역(Station)
    /// - 맵 구성: [시작 타일 1개] + [필드 타일 N개] + [역 타일 1개]
    /// - 시드 기반 랜덤 선택 + 연속 중복 방지 + 좌우 미러링
    /// - 셀: Empty=땅, Tree=나무, Rock=돌, Station=역(자원 없이 역 오브젝트 스폰)
    /// - PoolManager로 블록/자원 풀링
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private PoolManager poolManager;
        [SerializeField] private Transform   player;
        [SerializeField] private Transform   wallParent;

        [Header("타일 풀 (페인트로 그린 MapTile 에셋들)")]
        [SerializeField] private MapTile[] startTiles;    // 첫 시작 타일들
        [SerializeField] private MapTile[] fieldTiles;    // 일반 필드 타일들
        [SerializeField] private MapTile[] stationTiles;  // 역 타일들

        [Header("풀 키")]
        [SerializeField] private string grassKey   = "Grass"; // 땅(Empty)
        [SerializeField] private string stoneKey   = "NoGrass";
        [SerializeField] private string treeKey    = "Tree1";
        [SerializeField] private string rockKey    = "Rock1";
        [SerializeField] private string stationKey = "Station";    // 역 오브젝트

        [Header("그리드 / 블록")]
        [SerializeField] private float gridSize     = 0.5f;
        [SerializeField] private int   gridPerBlock = 4;
        [SerializeField] private float groundY      = 0f;
        [SerializeField] private float resourceY    = 0f;

        [Header("맵 구성")]
        [SerializeField] private int  fieldChunkCount = 4;  // 시작/역 사이 필드 타일 수
        [SerializeField] private int  seed            = 12345;
        [SerializeField] private bool randomSeed      = false;

        [Header("좌우 미러링")]
        [SerializeField] [Range(0f,1f)] private float mirrorChance  = 0.5f;
        [SerializeField] private bool mirrorStart   = false;
        [SerializeField] private bool mirrorStation = false;
        [Header("앞뒤(X) 미러링 — 필드 전용")]
        [SerializeField] [Range(0f,1f)] private float mirrorXChance = 0.5f;

        [Header("연속 중복 방지 (직전 몇 개 제외)")]
        [SerializeField] private int avoidRecent = 1;

        [Header("블록 스케일 보정")]
        [SerializeField] private bool  autoScaleGround = true;
        [SerializeField] private float prefabBaseSize  = 1f;

        [Header("경계 벽")]
        [SerializeField] private float wallHeight    = 10f;
        [SerializeField] private float wallThickness = 1f;

        private float BlockSize => gridPerBlock * gridSize;

        private readonly List<GameObject> _spawned = new();
        private GameObject _wallRoot;

        private System.Random _rng;
        private readonly Queue<int> _recentField = new();

        // 맵 전체 범위 추적(경계 벽용)
        private float _mapMinX, _mapMaxX, _mapMinZ, _mapMaxZ;
        private int   _cursorBX;   // 다음 타일이 시작될 블록 X (타일을 이어 붙이는 커서)
        private int   _baseBX;     // 플레이어 기준 시작 블록 X

        private void Start()
        {
            if (poolManager == null) poolManager = PoolManager.Instance;
            if (randomSeed) seed = System.Environment.TickCount;

            _baseBX = (player != null)
                ? Mathf.FloorToInt(player.position.x / BlockSize)
                : 0;

            GenerateMap();
            BuildBoundaryWalls();
        }

        // ── 맵 전체 생성 ─────────────────────────────────────────────────
        private void GenerateMap()
        {
            _rng = new System.Random(seed);
            _recentField.Clear();
            _cursorBX = _baseBX;
            _mapMinX = float.MaxValue; _mapMaxX = float.MinValue;
            _mapMinZ = float.MaxValue; _mapMaxZ = float.MinValue;
            

            // 1) 시작 타일 (X미러 없음)
            MapTile startTile = PickFrom(startTiles, mirrorStart, out bool m0);
            int startW = (startTile != null) ? startTile.width : 0;
            _cursorBX = _baseBX - startW / 2;
            
            PlaceTile(startTile, false, false);

            // 2) 필드 타일들 (Z미러 + X미러)
            for (int i = 0; i < fieldChunkCount; i++)
            {
                MapTile t = PickField(out bool mf);
                bool mirrorX = _rng.NextDouble() < mirrorXChance;
                PlaceTile(t, mf, mirrorX);
            }

            // 3) 역 타일 (X미러 없음)
            PlaceTile(PickFrom(stationTiles, mirrorStation, out bool ms), ms, false);
        }

        // 배열에서 시드로 하나 뽑기 (+ 미러 여부)
        private MapTile PickFrom(MapTile[] pool, bool allowMirror, out bool mirror)
        {
            mirror = allowMirror && _rng.NextDouble() < mirrorChance;
            if (pool == null || pool.Length == 0) return null;
            return pool[_rng.Next(pool.Length)];
        }

        // 필드 타일: 직전 avoidRecent개 제외하고 뽑기
        private MapTile PickField(out bool mirror)
        {
            mirror = _rng.NextDouble() < mirrorChance;
            if (fieldTiles == null || fieldTiles.Length == 0) return null;
            if (fieldTiles.Length == 1) return fieldTiles[0];

            var candidates = new List<int>();
            for (int i = 0; i < fieldTiles.Length; i++)
                if (!_recentField.Contains(i)) candidates.Add(i);
            if (candidates.Count == 0)
                for (int i = 0; i < fieldTiles.Length; i++) candidates.Add(i);

            int pick = candidates[_rng.Next(candidates.Count)];
            _recentField.Enqueue(pick);
            while (_recentField.Count > Mathf.Clamp(avoidRecent, 0, fieldTiles.Length - 1))
                _recentField.Dequeue();

            return fieldTiles[pick];
        }

        // ── 타일 한 장을 커서 위치에 배치하고 커서를 전진 ────────────────
        private void PlaceTile(MapTile tile, bool mirrorZ, bool mirrorX)
        {
            if (tile == null) return;

            int w = tile.width;
            int h = tile.height;
            int zMin = -(h / 2);

            for (int lx = 0; lx < w; lx++)
            {
                for (int lz = 0; lz < h; lz++)
                {
                    int readLx = mirrorX ? (w - 1 - lx) : lx; // 앞뒤(X) 미러
                    int readLz = mirrorZ ? (h - 1 - lz) : lz; // 좌우(Z) 미러
                    CellType c = tile.Get(readLx, readLz);

                    int bx = _cursorBX + lx;
                    int bz = zMin + lz;

                    Vector3 blockPos = new Vector3(
                        (bx + 0.5f) * BlockSize, groundY, bz * BlockSize);

                    // 돌 칸 아래는 stoneKey, 그 외는 grassKey
                    string groundKey = (c == CellType.Rock) ? stoneKey : grassKey;
                    GameObject ground = poolManager.Spawn(groundKey, blockPos, Quaternion.identity);
                    if (ground != null) { if (autoScaleGround) ApplyBlockScale(ground); _spawned.Add(ground); }

                    Vector3 topPos = new Vector3(blockPos.x, resourceY, blockPos.z);
                    switch (c)
                    {
                        case CellType.Tree:    SpawnTop(treeKey, topPos);    break;
                        case CellType.Rock:    SpawnTop(rockKey, topPos);    break;
                        case CellType.Station: SpawnTop(stationKey, topPos); break;
                        default: break;
                    }

                    UpdateBounds(blockPos);
                }
            }

            _cursorBX += w;
        }

        private void SpawnTop(string key, Vector3 pos)
        {
            GameObject go = poolManager.Spawn(key, pos, Quaternion.identity);
            if (go != null) _spawned.Add(go);
        }

        private void ApplyBlockScale(GameObject ground)
        {
            float factor = BlockSize / prefabBaseSize;
            Vector3 s = ground.transform.localScale;
            ground.transform.localScale = new Vector3(factor, s.y, factor);
        }

        private void UpdateBounds(Vector3 p)
        {
            float half = BlockSize * 0.5f;
            _mapMinX = Mathf.Min(_mapMinX, p.x - half);
            _mapMaxX = Mathf.Max(_mapMaxX, p.x + half);
            _mapMinZ = Mathf.Min(_mapMinZ, p.z - half);
            _mapMaxZ = Mathf.Max(_mapMaxZ, p.z + half);
        }

        // ── 경계 벽 ──────────────────────────────────────────────────────
        private void BuildBoundaryWalls()
        {
            if (_mapMinX > _mapMaxX) return; // 빈 맵 방어

            float midX = (_mapMinX + _mapMaxX) * 0.5f, midZ = (_mapMinZ + _mapMaxZ) * 0.5f;
            float lenX = _mapMaxX - _mapMinX, lenZ = _mapMaxZ - _mapMinZ;
            float wy = groundY + wallHeight * 0.5f;

            if (wallParent == null)
            {
                var go = new GameObject("BoundaryWalls");
                go.transform.SetParent(transform, false);
                wallParent = go.transform; _wallRoot = go;
            }
            CreateWall("Wall_Zmin", new Vector3(midX, wy, _mapMinZ - wallThickness*0.5f), new Vector3(lenX + wallThickness*2, wallHeight, wallThickness));
            CreateWall("Wall_Zmax", new Vector3(midX, wy, _mapMaxZ + wallThickness*0.5f), new Vector3(lenX + wallThickness*2, wallHeight, wallThickness));
            CreateWall("Wall_Xmin", new Vector3(_mapMinX - wallThickness*0.5f, wy, midZ), new Vector3(wallThickness, wallHeight, lenZ));
            CreateWall("Wall_Xmax", new Vector3(_mapMaxX + wallThickness*0.5f, wy, midZ), new Vector3(wallThickness, wallHeight, lenZ));
        }

        private void CreateWall(string name, Vector3 center, Vector3 size)
        {
            var w = new GameObject(name);
            w.transform.SetParent(wallParent, false);
            w.transform.position = center;
            w.AddComponent<BoxCollider>().size = size;
        }

        // ── 정리 / 재생성 ────────────────────────────────────────────────
        public void ClearMap()
        {
            foreach (var go in _spawned)
            {
                if (go == null) continue;
                if (autoScaleGround)
                {
                    var s = go.transform.localScale;
                    go.transform.localScale = new Vector3(prefabBaseSize, s.y, prefabBaseSize);
                }
                poolManager.Despawn(go);
            }
            _spawned.Clear();
            if (_wallRoot != null) { Destroy(_wallRoot); _wallRoot = null; wallParent = null; }
        }

        public void RegenerateMap()
        {
            ClearMap();
            if (randomSeed) seed = System.Environment.TickCount;
            GenerateMap();
            BuildBoundaryWalls();
        }
    }
}
