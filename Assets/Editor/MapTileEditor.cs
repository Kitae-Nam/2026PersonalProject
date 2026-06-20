#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// MapTile을 인스펙터에서 그림판처럼 칠하는 커스텀 에디터.
/// - width/height 인트값을 입력하면 그 크기의 정사각 격자가 그려짐
/// - 팔레트에서 색(셀 타입)을 고르고 격자를 클릭/드래그하면 칠해짐
/// - 색 규칙: 빈칸=땅, 갈색=나무, 흰색=돌, 하늘색=역
/// </summary>
[CustomEditor(typeof(MapTile))]
public class MapTileEditor : Editor
{
    private CellType _brush = CellType.Tree; // 현재 선택된 브러쉬
    private const float CellSize = 22f;      // 격자 한 칸의 픽셀 크기

    // 셀 타입별 표시 색
    private static Color ColorOf(CellType t) => t switch
    {
        CellType.Empty   => new Color(0.55f, 0.78f, 0.35f), // 땅(연두) — 시각적 구분용
        CellType.Tree    => new Color(0.55f, 0.35f, 0.15f), // 갈색
        CellType.Rock    => Color.white,                    // 흰색
        CellType.Station => new Color(0.45f, 0.78f, 0.95f), // 하늘색
        _                => Color.gray
    };

    private static string NameOf(CellType t) => t switch
    {
        CellType.Empty   => "땅",
        CellType.Tree    => "나무",
        CellType.Rock    => "돌",
        CellType.Station => "역",
        _ => "?"
    };

    public override void OnInspectorGUI()
    {
        var tile = (MapTile)target;

        // ── 기본 필드 ──
        EditorGUI.BeginChangeCheck();
        var kind = (TileKind)EditorGUILayout.EnumPopup("타일 종류", tile.kind);

        EditorGUILayout.BeginHorizontal();
        int w = EditorGUILayout.IntField("가로(Width)",  tile.width);
        int h = EditorGUILayout.IntField("세로(Height)", tile.height);
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(tile, "Edit MapTile");
            tile.kind = kind;
            if (w != tile.width || h != tile.height)
                tile.Resize(Mathf.Max(1, w), Mathf.Max(1, h));
            EditorUtility.SetDirty(tile);
        }

        EditorGUILayout.Space(8);

        // ── 팔레트 (브러쉬 선택) ──
        EditorGUILayout.LabelField("브러쉬", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        foreach (CellType t in System.Enum.GetValues(typeof(CellType)))
        {
            bool selected = (_brush == t);
            var bg = GUI.backgroundColor;
            GUI.backgroundColor = selected ? Color.yellow : ColorOf(t);
            if (GUILayout.Button(NameOf(t), GUILayout.Height(26)))
                _brush = t;
            GUI.backgroundColor = bg;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(8);

        // ── 격자 페인트 영역 ──
        EditorGUILayout.LabelField($"격자 ({tile.width} x {tile.height}) — 클릭/드래그로 칠하기",
                                   EditorStyles.miniBoldLabel);

        // 격자 전체가 들어갈 사각 영역 확보
        Rect area = GUILayoutUtility.GetRect(
            tile.width * CellSize, tile.height * CellSize,
            GUILayout.ExpandWidth(false));

        DrawGrid(tile, area);
        HandlePaint(tile, area);

        EditorGUILayout.Space(8);

        // ── 유틸 버튼 ──
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("전체 지우기(땅)"))
        {
            Undo.RecordObject(tile, "Clear MapTile");
            for (int i = 0; i < tile.cells.Length; i++) tile.cells[i] = CellType.Empty;
            EditorUtility.SetDirty(tile);
        }
        if (GUILayout.Button("좌우 미리보기 뒤집기"))
        {
            Undo.RecordObject(tile, "Flip MapTile");
            FlipHorizontal(tile);
            EditorUtility.SetDirty(tile);
        }
        EditorGUILayout.EndHorizontal();

        if (GUI.changed) EditorUtility.SetDirty(tile);
    }

    // 격자 그리기 (위쪽이 z=height-1, 즉 화면 위 = 맵 안쪽/먼쪽으로 보이게)
    private void DrawGrid(MapTile tile, Rect area)
    {
        for (int z = 0; z < tile.height; z++)
        {
            for (int x = 0; x < tile.width; x++)
            {
                // 화면 위쪽이 z가 큰 쪽이 되도록 뒤집어 그림
                int drawRow = tile.height - 1 - z;
                Rect cell = new Rect(
                    area.x + x * CellSize,
                    area.y + drawRow * CellSize,
                    CellSize - 1, CellSize - 1);

                EditorGUI.DrawRect(cell, ColorOf(tile.Get(x, z)));
            }
        }

        // 격자 외곽선
        Handles.color = new Color(0, 0, 0, 0.4f);
        for (int x = 0; x <= tile.width; x++)
            Handles.DrawLine(new Vector3(area.x + x * CellSize, area.y),
                             new Vector3(area.x + x * CellSize, area.y + tile.height * CellSize));
        for (int z = 0; z <= tile.height; z++)
            Handles.DrawLine(new Vector3(area.x, area.y + z * CellSize),
                             new Vector3(area.x + tile.width * CellSize, area.y + z * CellSize));
    }

    // 마우스 클릭/드래그로 칠하기
    private void HandlePaint(MapTile tile, Rect area)
    {
        Event e = Event.current;
        if (e.type != EventType.MouseDown && e.type != EventType.MouseDrag) return;
        if (!area.Contains(e.mousePosition)) return;

        int x = Mathf.FloorToInt((e.mousePosition.x - area.x) / CellSize);
        int drawRow = Mathf.FloorToInt((e.mousePosition.y - area.y) / CellSize);
        int z = tile.height - 1 - drawRow; // 화면 좌표 → z 인덱스 복원

        if (x < 0 || x >= tile.width || z < 0 || z >= tile.height) return;

        // 좌클릭=브러쉬 칠, 우클릭=땅으로 지우기
        CellType paint = (e.button == 1) ? CellType.Empty : _brush;
        if (tile.Get(x, z) != paint)
        {
            Undo.RecordObject(tile, "Paint MapTile");
            tile.Set(x, z, paint);
            EditorUtility.SetDirty(tile);
        }
        e.Use();
        Repaint();
    }

    private void FlipHorizontal(MapTile tile)
    {
        for (int z = 0; z < tile.height; z++)
            for (int x = 0; x < tile.width / 2; x++)
            {
                int mx = tile.width - 1 - x;
                var a = tile.Get(x, z);
                var b = tile.Get(mx, z);
                tile.Set(x, z, b);
                tile.Set(mx, z, a);
            }
    }
}
#endif
