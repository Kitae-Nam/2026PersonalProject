using UnityEngine;

namespace _01_Script.Map
{
    public class WaterManager : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private Transform   target;       // 카메라 또는 플레이어(기차)
        [SerializeField] private Transform[] waterTiles;   // 물 평면들 (2~3장)
     
        [Header("타일 길이 (0이면 Renderer로 자동 측정)")]
        [SerializeField] private float tileLength = 0f;
     
        [Header("순환 민감도")]
        [Tooltip("맨 뒤 평면이 타겟보다 (tileLength * 이 값)만큼 뒤처지면 앞으로 순환")]
        [SerializeField] private float backThreshold = 1.0f;
     
        private void Start()
        {
            if (waterTiles == null || waterTiles.Length == 0)
            {
                Debug.LogWarning("[WaterScroller] waterTiles 가 비어 있습니다.");
                enabled = false;
                return;
            }
     
            // 평면 한 장의 X 길이 자동 측정
            if (tileLength <= 0f)
            {
                var rend = waterTiles[0].GetComponentInChildren<Renderer>();
                if (rend != null) tileLength = rend.bounds.size.x;
                if (tileLength <= 0f)
                {
                    Debug.LogWarning("[WaterScroller] tileLength 측정 실패. 인스펙터에서 직접 입력하세요.");
                    tileLength = 100f;
                }
            }
     
            SortByX();      // 배열을 X 오름차순(0 = 가장 뒤)으로 정렬
            AlignTiles();   // tileLength 간격으로 빈틈 없이 일렬 배치
        }
     
        private void Update()
        {
            if (target == null) return;
     
            Transform back  = waterTiles[0];                     // 가장 뒤(작은 X)
            Transform front = waterTiles[waterTiles.Length - 1]; // 가장 앞(큰 X)
     
            // 타겟이 충분히 전진해 맨 뒤 평면이 뒤로 벗어났으면 앞으로 보냄
            // while: 한 프레임에 여러 칸 이동(고속 이동)해도 따라잡도록
            while (target.position.x - back.position.x > tileLength * backThreshold)
            {
                Vector3 p = back.position;
                p.x = front.position.x + tileLength;  // 맨 앞 평면 바로 뒤(앞쪽)에 이어 붙임
                back.position = p;
     
                ShiftForward();                        // 배열 순서 갱신
                back  = waterTiles[0];
                front = waterTiles[waterTiles.Length - 1];
            }
            while (front.position.x - target.position.x > tileLength * backThreshold)
            {
                Vector3 p = front.position;
                p.x = back.position.x - tileLength;
                front.position = p;

                ShiftBackward();
                back  = waterTiles[0];
                front = waterTiles[waterTiles.Length - 1];
            }
        }
     
        // ── 보조 ─────────────────────────────────────────────────────────
        private void SortByX()
        {
            System.Array.Sort(waterTiles, (a, b) => a.position.x.CompareTo(b.position.x));
        }
     
        private void AlignTiles()
        {
            float baseX = waterTiles[0].position.x;
            for (int i = 0; i < waterTiles.Length; i++)
            {
                Vector3 p = waterTiles[i].position;
                p.x = baseX + i * tileLength;
                waterTiles[i].position = p;
            }
        }
     
        // waterTiles[0]을 맨 끝으로 회전 (배열 순서 = X 순서 유지)
        private void ShiftForward()
        {
            Transform moved = waterTiles[0];
            for (int i = 0; i < waterTiles.Length - 1; i++)
                waterTiles[i] = waterTiles[i + 1];
            waterTiles[waterTiles.Length - 1] = moved;
        }
        private void ShiftBackward()
        {
            int last = waterTiles.Length - 1;
            Transform moved = waterTiles[last];
            for (int i = last; i > 0; i--)
                waterTiles[i] = waterTiles[i - 1];
            waterTiles[0] = moved;
        }
    }
}