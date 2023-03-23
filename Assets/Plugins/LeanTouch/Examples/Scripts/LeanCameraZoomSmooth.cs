using UnityEngine;

namespace Lean.Touch
{
    public class LeanCameraZoomSmooth : LeanCameraZoom
    {
        [Tooltip("How quickly the zoom reaches the target value")]
        public float Dampening = 10.0f;

        //[SerializeField] private LeanMultiTap _leanMultiTap;

        private float currentZoom;

        protected override void LateUpdate()
        {
            base.LateUpdate();

            //if (_leanMultiTap.HighestFingerCount != 1) return;

            var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

            currentZoom = Mathf.Lerp(currentZoom, Zoom, factor);

            SetZoom(currentZoom);

            if (Input.GetKey(KeyCode.Space)) SetStandardZoom();
        }

        protected virtual void OnEnable()
        {
            currentZoom = Zoom;
        }

        public float GetSizeProcent()
        {
            return 1f - (currentZoom - ZoomMin) / (ZoomMax - ZoomMin);
        }

        public void SetStandardZoom()
        {
            Zoom = ZoomMax;
        }

        public void SetNewZoomBorders(float minZoom, float maxZoom)
        {
            ZoomMin = minZoom;
            ZoomMax = maxZoom;
        }

        public void SetTargetZoom(float zoom)
        {
            Zoom = Mathf.Clamp(zoom, ZoomMin, ZoomMax);
        }
    }
}