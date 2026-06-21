using UnityEngine;
using UnityEngine.EventSystems;

namespace _01_Script.UI.Carouse
{
    public class UIPreviewRotator : MonoBehaviour
    {
        [Header("회전 대상 (3D 오브젝트)")] [SerializeField]
        private Transform target;

        [Header("자동 회전")] [SerializeField] private float autoSpeed = 30f; // 초당 각도
        [SerializeField] private Vector3 autoAxis = Vector3.up;
        [SerializeField] private float resumeDelay = 1.5f; // 드래그 후 자동 재개까지 대기(초)
        private float _idleTimer; // 마지막 드래그 이후 경과 시간

        private void Update()
        {
            if (target == null) return;


            _idleTimer += Time.deltaTime;
            if (_idleTimer >= resumeDelay)
                target.Rotate(autoAxis, autoSpeed * Time.deltaTime, Space.World);
        }
    }
}