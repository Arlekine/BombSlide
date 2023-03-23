using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
    // This script fires events if a finger has been held for a certain amount of time without moving
    public class LeanFingerSwipe : MonoBehaviour
    {
        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreGuiFingers = true;

        [Tooltip("Must the swipe be in a specific direction?")]
        public bool CheckAngle;

        [Tooltip("The required angle of the swipe in degrees, where 0 is up, and 90 is right")]
        public float Angle;

        [Tooltip("The left/right tolerance of the swipe angle in degrees")]
        public float AngleThreshold = 90.0f;

        // Called on the first frame the conditions are met
        public FingerEvent OnFingerSwipe;

        protected virtual void OnEnable()
        {
            // Hook events
            LeanTouch.OnFingerSwipe += FingerSwipe;
        }

        protected virtual void OnDisable()
        {
            // Unhook events
            LeanTouch.OnFingerSwipe -= FingerSwipe;
        }

        protected void CheckSwipe(LeanFinger finger, Vector2 delta)
        {
            if (CheckAngle)
            {
                var angle = Mathf.Atan2(delta.x, delta.y) * Mathf.Rad2Deg;

                if (Mathf.Abs(Mathf.DeltaAngle(angle, Angle)) >= AngleThreshold * 0.5f) return;
            }

            // Call event
            OnFingerSwipe.Invoke(finger);
        }

        private void FingerSwipe(LeanFinger finger)
        {
            // Ignore this finger?
            if (IgnoreGuiFingers && finger.StartedOverGui) return;

            CheckSwipe(finger, finger.SwipeScreenDelta);
        }

        // Event signature
        [Serializable]
        public class FingerEvent : UnityEvent<LeanFinger>
        {
        }
    }
}