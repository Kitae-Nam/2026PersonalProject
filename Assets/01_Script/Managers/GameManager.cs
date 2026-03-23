using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoSingleton<GameManager>
{
    public Tilemap _groundTile;
    public GameObject _player;
    public Transform _ItemPileParent;
}
