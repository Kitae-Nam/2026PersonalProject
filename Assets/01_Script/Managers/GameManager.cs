using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoSingleton<GameManager>
{
    public Tilemap groundTile;
    public GameObject player;
    public Transform ItemPileParent;
}
