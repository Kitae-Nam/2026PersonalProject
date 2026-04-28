using UnityEngine;

[CreateAssetMenu(fileName = "RailInfoSO", menuName = "Scriptable Objects/RailInfoSO")]
public class RailInfoSO : ScriptableObject
{
    public Mesh frontToBackMesh;
    public Mesh frontToRightMesh;
    public Mesh frontToLeftMesh;
    public Mesh backToRightMesh;
    public Mesh backToLeftMesh;
    public Mesh rightToLeftMesh;

    public Mesh MeshChange(byte railType)
    {
        switch (railType)// ╬у ╣з ©Л аб = 0001 0010 0100 1000
        {
            case 0b1100: return frontToBackMesh;
            case 0b1010: return frontToRightMesh;
            case 0b1001: return frontToLeftMesh;
            case 0b0110: return backToRightMesh;
            case 0b0101: return backToLeftMesh;
            case 0b0011: return rightToLeftMesh;
            default: return null;
        }
    }
}
