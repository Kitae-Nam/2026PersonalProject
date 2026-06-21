using System;
using System.Collections.Generic;
using _01_Script.Event;
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
        [SerializeField] private TrainInfoChangeSo eventSo;
        [SerializeField] private PoolManager poolManager;
        [SerializeField] private Transform   player;
        [SerializeField] private Transform   wallParent;
 
        [Header("타일 풀 (페인트로 그린 MapTile 에셋들)")]
        [SerializeField] private MapTile[] startTiles;
        [SerializeField] private MapTile[] fieldTiles;
        [SerializeField] private MapTile[] stationTiles;
 
        [Header("풀 키")]
        [SerializeField] private string grassKey   = "Grass";
        [SerializeField] private string stoneKey   = "NoGrass";
        [SerializeField] private string treeKey    = "Tree1";
        [SerializeField] private string rockKey    = "Rock1";
        [SerializeField] private string stationKey = "Station";
 
        [Header("그리드 / 블록")]
        [SerializeField] private float gridSize     = 0.5f;
        [SerializeField] private int   gridPerBlock = 4;
        [SerializeField] private float groundY      = 0f;
        [SerializeField] private float resourceY    = 0f;
 
        [Header("맵 구성")]
        [SerializeField] private int  fieldChunkCount = 4;
        [SerializeField] private int  seed            = 12345;
        [SerializeField] private bool randomSeed      = false;
 
        [Header("뒤처진 타일 정리 기준 (블록 단위)")]
        [Tooltip("플레이어보다 이 블록 수 이상 뒤(작은 X)인 타일은 풀로 반환")]
        [SerializeField] private int despawnBehindBlocks = 32;
 
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
        
        [SerializeField] private Vector3 resourceOffset = Vector3.zero;
 
        private float BlockSize => gridPerBlock * gridSize;
 
        // 배치된 타일 1장의 기록
        private class PlacedTile
        {
            public int startBX;
            public int endBX;
            public bool isStation;
            public Vector3 stationPos;
            public GameObject stationGo;
            public readonly List<GameObject> objects = new();
        }
 
        private readonly List<PlacedTile> _tiles = new();
        private GameObject _wallRoot;
 
        private System.Random _rng;
        private readonly Queue<int> _recentField = new();
 
        private int _cursorBX;
        private int _baseBX;
 
        private void Start()
        {
            if (poolManager == null) poolManager = PoolManager.Instance;
            if (randomSeed) seed = System.Environment.TickCount;
 
            _baseBX = (player != null)
                ? Mathf.FloorToInt(player.position.x / BlockSize)
                : 0;
 
            GenerateInitial();
            BuildBoundaryWalls();
        }
 
        // 최초 생성: [시작] + [필드 N] + [역]
        private void GenerateInitial()
        {
            _rng = new System.Random(seed);
            _recentField.Clear();
            _tiles.Clear();
 
            MapTile startTile = PickFrom(startTiles, mirrorStart, out _);
            int startW = (startTile != null) ? startTile.width : 0;
            _cursorBX = _baseBX - startW / 2;
 
            PlaceTile(startTile, false, false, isStation: false);
 
            AppendSegment();
        }
 
        // 역 도착 시 외부 이벤트에서 호출
        public void OnReachStation()
        {
            AppendSegment();
            CleanupBehind();
            RebuildWalls();
        }
 
        // 필드 N개 + 역 1개 이어 붙이기
        private void AppendSegment()
        {
            for (int i = 0; i < fieldChunkCount; i++)
            {
                MapTile t = PickField(out bool mf);
                bool mirrorX = _rng.NextDouble() < mirrorXChance;
                PlaceTile(t, mf, mirrorX, isStation: false);
            }
 
            MapTile station = PickFrom(stationTiles, mirrorStation, out bool ms);
            PlaceTile(station, ms, false, isStation: true);
        }
 
        // 플레이어보다 despawnBehindBlocks 이상 뒤인 타일 정리.
        // 플레이어가 위에 서 있는 타일은 절대 지우지 않는다.
        private void CleanupBehind()
        {
            if (player == null) return;
            int playerBX = Mathf.FloorToInt(player.position.x / BlockSize);
            int cutoff = playerBX - despawnBehindBlocks;
 
            for (int i = _tiles.Count - 1; i >= 0; i--)
            {
                PlacedTile t = _tiles[i];
 
                bool playerOnTile = playerBX >= t.startBX && playerBX < t.endBX;
                if (playerOnTile) continue;
 
                if (t.endBX <= cutoff)
                {
                    DespawnTile(t);
                    _tiles.RemoveAt(i);
                }
            }
        }
 
        private MapTile PickFrom(MapTile[] pool, bool allowMirror, out bool mirror)
        {
            mirror = allowMirror && _rng.NextDouble() < mirrorChance;
            if (pool == null || pool.Length == 0) return null;
            return pool[_rng.Next(pool.Length)];
        }
 
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
 
        // 타일 한 장 배치 + 커서 전진
        private void PlaceTile(MapTile tile, bool mirrorZ, bool mirrorX, bool isStation)
        {
            if (tile == null) return;
 
            int w = tile.width;
            int h = tile.height;
            int zMin = -(h / 2);
 
            var placed = new PlacedTile
            {
                startBX   = _cursorBX,
                endBX     = _cursorBX + w,
                isStation = isStation
            };
 
            for (int lx = 0; lx < w; lx++)
            {
                for (int lz = 0; lz < h; lz++)
                {
                    int readLx = mirrorX ? (w - 1 - lx) : lx;
                    int readLz = mirrorZ ? (h - 1 - lz) : lz;
                    CellType c = tile.Get(readLx, readLz);
 
                    int bx = _cursorBX + lx;
                    int bz = zMin + lz;
 
                    Vector3 blockPos = new Vector3(
                        bx * BlockSize, groundY, bz * BlockSize) + resourceOffset;
 
                    string groundKey = (c == CellType.Rock) ? stoneKey : grassKey;
                    GameObject ground = poolManager.Spawn(groundKey, blockPos, Quaternion.identity);
                    if (ground != null)
                    {
                        if (autoScaleGround) ApplyBlockScale(ground);
                        placed.objects.Add(ground);
                    }
 
                    Vector3 topPos = new Vector3(blockPos.x, resourceY, blockPos.z);
                    switch (c)
                    {
                        case CellType.Tree:
                            SpawnTop(treeKey, topPos, placed); break;
                        case CellType.Rock:
                            SpawnTop(rockKey, topPos, placed); break;
                        case CellType.Station:
                            var st = SpawnTop(stationKey, topPos, placed);
                            if (st != null) { placed.stationPos = topPos; placed.stationGo = st; }
                            break;
                        default: break;
                    }
                }
            }
 
            _tiles.Add(placed);
            _cursorBX += w;

            if (isStation && placed.stationGo != null)
            {
                Debug.Log("cc");
                eventSo.OnStationChangeInvoke(placed.stationGo.transform);
            }
        }
 
        private GameObject SpawnTop(string key, Vector3 pos, PlacedTile placed)
        {
            GameObject go = poolManager.Spawn(key, pos, Quaternion.identity);
            if (go != null) placed.objects.Add(go);
            return go;
        }
 
        private void DespawnTile(PlacedTile t)
        {
            foreach (var go in t.objects)
            {
                if (go == null) continue;
                if (autoScaleGround)
                {
                    var s = go.transform.localScale;
                    go.transform.localScale = new Vector3(prefabBaseSize, s.y, prefabBaseSize);
                }
                poolManager.Despawn(go);
            }
            t.objects.Clear();
        }
 
        private void ApplyBlockScale(GameObject ground)
        {
            float factor = BlockSize / prefabBaseSize;
            Vector3 s = ground.transform.localScale;
            ground.transform.localScale = new Vector3(factor, s.y, factor);
        }
 
        // 경계 벽 (현재 살아있는 타일 범위)
        private void BuildBoundaryWalls()
        {
            if (_tiles.Count == 0) return;
 
            int minBX = int.MaxValue, maxBX = int.MinValue;
            int anyH = 16;
            foreach (var t in _tiles)
            {
                if (t.startBX < minBX) minBX = t.startBX;
                if (t.endBX   > maxBX) maxBX = t.endBX;
            }
 
            float minX = minBX * BlockSize - BlockSize * 0.5f;
            float maxX = maxBX * BlockSize - BlockSize * 0.5f;
            float halfZ = (anyH / 2f) * BlockSize + BlockSize * 0.5f;
            float minZ = -halfZ;
            float maxZ =  halfZ;
 
            float midX = (minX + maxX) * 0.5f, midZ = (minZ + maxZ) * 0.5f;
            float lenX = maxX - minX, lenZ = maxZ - minZ;
            float wy = groundY + wallHeight * 0.5f;
 
            if (wallParent == null)
            {
                var go = new GameObject("BoundaryWalls");
                go.transform.SetParent(transform, false);
                wallParent = go.transform; _wallRoot = go;
            }
            CreateWall("Wall_Zmin", new Vector3(midX, wy, minZ - wallThickness*0.5f), new Vector3(lenX + wallThickness*2, wallHeight, wallThickness));
            CreateWall("Wall_Zmax", new Vector3(midX, wy, maxZ + wallThickness*0.5f), new Vector3(lenX + wallThickness*2, wallHeight, wallThickness));
            CreateWall("Wall_Xmin", new Vector3(minX - wallThickness*0.5f, wy, midZ), new Vector3(wallThickness, wallHeight, lenZ));
            CreateWall("Wall_Xmax", new Vector3(maxX + wallThickness*0.5f, wy, midZ), new Vector3(wallThickness, wallHeight, lenZ));
        }
 
        private void RebuildWalls()
        {
            if (_wallRoot != null) { Destroy(_wallRoot); _wallRoot = null; wallParent = null; }
            BuildBoundaryWalls();
        }
 
        private void CreateWall(string name, Vector3 center, Vector3 size)
        {
            var w = new GameObject(name);
            w.transform.SetParent(wallParent, false);
            w.layer = LayerMask.NameToLayer("Ground");
            w.transform.position = center;
            w.AddComponent<BoxCollider>().size = size;
        }
 
        // 정리 / 재생성
        public void ClearMap()
        {
            foreach (var t in _tiles) DespawnTile(t);
            _tiles.Clear();
            if (_wallRoot != null) { Destroy(_wallRoot); _wallRoot = null; wallParent = null; }
        }
 
        public void RegenerateMap()
        {
            ClearMap();
            if (randomSeed) seed = System.Environment.TickCount;
            GenerateInitial();
            BuildBoundaryWalls();
        }
    }
}
