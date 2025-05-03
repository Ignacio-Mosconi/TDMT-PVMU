using UnityEngine;
using UnityEngine.EventSystems;

namespace Class3
{
    public class CameraTracker : MonoBehaviour
    {
        [Header("Available Actions")]
        [SerializeField] private bool orbit = true;
        [SerializeField] private bool zoom = true;

        [Header("Tracking Settings")]
        [SerializeField] private Vector3 baseFollowOffset = new Vector3(0f, 4f, -6f);
        [SerializeField, Range(0f, 180f)] private float orbitSpeed = 90f;
        [SerializeField, Range(0f, 100f)] private float zoomSpeed = 10f;
        [SerializeField, Range(0f, 1f)] private float minZoom = 0.5f;
        [SerializeField, Range(1f, 5f)] private float maxZoom = 2f;

        private Transform followTarget;
        private Vector3 currentOffset;
        private float currentZoom;


        void LateUpdate ()
        {
            if (!followTarget || EventSystem.current.IsPointerOverGameObject())
                return;

            if (orbit)
            {
                float inputValue = Input.GetAxis("Mouse X");
                currentOffset = Quaternion.AngleAxis(inputValue * orbitSpeed * Time.deltaTime, Vector3.up) * currentOffset;
            }

            if (zoom)
            {
                float inputValue = Input.GetAxis("Mouse ScrollWheel");
                currentZoom = Mathf.Clamp(currentZoom - inputValue * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
            }

            transform.position = followTarget.position + currentOffset * currentZoom;

            transform.LookAt(followTarget.position);
        }


        public void SetFollowTarget (Transform followTarget)
        {
            this.followTarget = followTarget;
            currentOffset = baseFollowOffset;
            currentZoom = 1f;
        }
    }
}