using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomGameObjectBrush", menuName = "Brushes/Custom GameObject Brush")]
[CustomGridBrush(false, true, false, "Prefab Brush")]
public class CustomGameObjectBrush : GameObjectBrush
{
    public GameObject[] _gameObjectsToPaint;

    public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        if(_gameObjectsToPaint == null || _gameObjectsToPaint.Length == 0)
            return;

        GameObject randomObject = _gameObjectsToPaint[Random.Range(0, _gameObjectsToPaint.Length)];

        Vector3 test = gridLayout.CellToWorld(position) + (Vector3)(gridLayout.cellSize * 0.5f);
        test.y = brushTarget.transform.position.y;

        if (HasObjectInCell(gridLayout, brushTarget.transform, test))
            return;

        GameObject.Instantiate(randomObject, test, GetQuaternion(Random.Range(0, 5)), brushTarget.transform);
    }
    public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        Vector3 worldPosition = gridLayout.CellToWorld(position) + (Vector3)(gridLayout.cellSize * 0.5f);
        worldPosition.y = brushTarget.transform.position.y;
        for (int i = 0; i < brushTarget.transform.childCount; i++)
        {
            Transform child = brushTarget.transform.GetChild(i);
            if (child.position == worldPosition)
            {
                GameObject.DestroyImmediate(child.gameObject);
                return;
            }
        }
    }
    private static bool HasObjectInCell(GridLayout gridLayout, Transform parent, Vector3 position)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Vector3 cell = child.position;
            if (cell == position) return true;
        }
        return false;
    }
    private static Quaternion GetQuaternion(int x)
    {
        switch (x)
        {
            case 0:
                return Quaternion.Euler(0, 0, 0);
            case 1:
                return Quaternion.Euler(0, 90, 0);
            case 2:
                return Quaternion.Euler(0, 180, 0);
            case 3:
                return Quaternion.Euler(0, 270, 0);
            default:
                return Quaternion.Euler(0, 0, 0);
        }
    }
}
