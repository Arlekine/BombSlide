using UnityEngine;

namespace Lean.Touch
{
    public class LeanCameraMoveSmooth : LeanCameraMove
    {
        [Tooltip("How quickly the zoom reaches the target value")]
        public float Dampening = 10.0f;

        public bool _screenBorders;

        [SerializeField] private float _screenBorderX;

        [SerializeField] private float _screenBorderY;

        public Vector3 remainingDelta;

        [SerializeField] private LeanCameraZoomSmooth _leanCameraZoomSmooth;

        [SerializeField] private LeanMultiTap _leanMultiTap;

        private Transform _currentTarget;

        protected override void LateUpdate()
        {
            var oldPosition = transform.localPosition;
            float factor = Time.deltaTime * 4f;
            
            if (_currentTarget == null)
            {
                if (_leanMultiTap.HighestFingerCount <= 1)
                {
                    base.LateUpdate();
                    remainingDelta += transform.localPosition - oldPosition;
                    factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);
                }
            }
            else
            {
                remainingDelta += _currentTarget.position - oldPosition;
            }
            
            var newDelta = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

            var newPosition = oldPosition + remainingDelta - newDelta;

            if (_screenBorders)
            {
                var borders = GetBorders();

                newPosition.x = Mathf.Clamp(newPosition.x, -borders.x, borders.x);

                newPosition.y = Mathf.Clamp(newPosition.y, -borders.y, borders.y);
            }

            transform.localPosition = newPosition;

            // Update remainingDelta with the dampened value
            remainingDelta = newDelta;
        }

        private Vector2 GetBorders()
        {
            var sizeProcent = _leanCameraZoomSmooth.GetSizeProcent();

            var borders = new Vector2(_screenBorderX * sizeProcent, _screenBorderY * sizeProcent);

            return borders;
        }

        [EditorButton]
        public void SetTarget(Transform target)
        {
            _currentTarget = target;
        }

        public void Free()
        {
            _currentTarget = null;
        }
    }
}