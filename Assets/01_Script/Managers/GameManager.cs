using System;
using _01_Script.Event;
using _01_Script.Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private TrainInfoChangeSo trainInfoChangeSo;
    
    public Tilemap groundTile;
    public GameObject player;
    public Transform ItemPileParent;
    public GameObject engineTrain;
    public GameObject station;

    private void Start()
    {
        if (trainInfoChangeSo != null)
        {
            trainInfoChangeSo.OnStationChange += StationChange;
        }
    }

    private void StationChange(Transform obj)
    {
        station = obj.gameObject;
    }
}
