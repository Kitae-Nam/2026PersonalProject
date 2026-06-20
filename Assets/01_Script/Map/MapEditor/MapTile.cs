using UnityEngine;

/// <summary>
/// 한 칸이 무엇인지. 빈칸=땅, 나무, 돌, 역.
/// 색 규칙: Empty=칠 안함(땅), Tree=갈색, Rock=흰색, Station=하늘색
/// </summary>
public enum CellType
{
    Empty   = 0,  // 땅 (자원 없음)
    Tree    = 1,  // 나무 (갈색)
    Rock    = 2,  // 돌   (흰색)
    Station = 3,  // 역   (하늘색)
}

/// <summary>
/// 타일의 용도 분류. 맵 생성 시 타입별 풀에서 뽑는다.
/// </summary>
public enum TileKind
{
    Start = 0,  // 첫 시작 타일
    Field = 1,  // 일반 필드 타일
    Station = 2 // 역 필드 타일
}

/// <summary>
/// 페인트로 그린 한 장의 타일 데이터.
/// width x height 격자를 cells[ ] 1차원 배열로 직렬화 저장.
/// (Unity는 2차원 배열 직렬화를 지원하지 않으므로 1차원 + 인덱스 변환 사용)
/// </summary>
[CreateAssetMenu(menuName = "Map/Map Tile", fileName = "MapTile")]
public class MapTile : ScriptableObject
{
    [Tooltip("타일 용도 분류")]
    public TileKind kind = TileKind.Field;

    [Min(1)] public int width  = 16;  // 가로 칸 수 (X)
    [Min(1)] public int height = 16;  // 세로 칸 수 (Z)

    [HideInInspector] public CellType[] cells = new CellType[16 * 16];

    // (x, z) → 1차원 인덱스
    public int Index(int x, int z) => z * width + x;

    public CellType Get(int x, int z)
    {
        if (x < 0 || x >= width || z < 0 || z >= height) return CellType.Empty;
        return cells[Index(x, z)];
    }

    public void Set(int x, int z, CellType t)
    {
        if (x < 0 || x >= width || z < 0 || z >= height) return;
        cells[Index(x, z)] = t;
    }

    /// <summary>
    /// width/height 변경 시 기존 그림을 최대한 보존하며 배열 크기를 재조정.
    /// </summary>
    public void Resize(int newW, int newH)
    {
        newW = Mathf.Max(1, newW);
        newH = Mathf.Max(1, newH);
        var newCells = new CellType[newW * newH];
        for (int z = 0; z < newH; z++)
            for (int x = 0; x < newW; x++)
                if (x < width && z < height)
                    newCells[z * newW + x] = cells[z * width + x];
        cells  = newCells;
        width  = newW;
        height = newH;
    }

    private void OnValidate()
    {
        // 인스펙터에서 width/height를 직접 바꿨을 때 배열 길이 동기화
        if (cells == null || cells.Length != width * height)
            Resize(width, height);
    }
}
