using System;
using _01_Script.Event;
using _01_Script.Managers;
using _01_Script.Map.MapEditor;
using _01_Script.Train;
using _01_Script.UI.Carouse;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private TrainInfoSo trainInfoSo;
    [SerializeField] private TrainInfoChangeSo trainInfoChangeSo;
    
    public MapManager MapManager;
    public StoreManager StoreManager;
    
    public Tilemap groundTile;
    public GameObject player;
    public Transform ItemPileParent;
    public GameObject engineTrain;
    public GameObject station;
    
    private int pastTotalDistance;
    [SerializeField] private float fadeDuration;
    [SerializeField] private CanvasGroup canvasGroup;

    protected override void Awake()
    {
        base.Awake();
        if (trainInfoChangeSo != null)
        {
            trainInfoChangeSo.OnStationChange += StationChange;
        }

        TrainSplineMove.OnRailStation += () =>//맵 재생성, 상점, 속도, 자원 획득, 맵 길이 늘어남
        {
            float totalDistance = TrainManager.Instance.TotalDistance;
            int total = (int)Math.Truncate(totalDistance);
            CostManager.Instance.Add((total - pastTotalDistance) / 10);
            trainInfoChangeSo.OnSpeedChangeInvoke(trainInfoSo.speed +1);
            MapManager.fieldChunkCount += 1;
            MapManager.RegenerateMap();
            StoreManager.Open();
            pastTotalDistance = total;
        };
        TrainSplineMove.OnRailBroken += () =>
        {
            canvasGroup.DOFade(1f, fadeDuration);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        };
    }

    private void StationChange(Transform obj)
    {
        station = obj.gameObject;
    }
}
