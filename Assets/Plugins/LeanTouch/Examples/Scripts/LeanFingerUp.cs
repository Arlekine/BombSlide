using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
    // This script calls the OnFingerUp event when a finger stops touching the screen
    public class LeanFingerUp : MonoBehaviour
    {
        [Tooltip("If the finger is over the GUI, ignore it?")]
        public bool IgnoreIfOverGui;

        [Tooltip("If the finger started over the GUI, ignore it?")]
        public bool IgnoreIfStartedOverGui;

        public LeanFingerEvent OnFingerUp;

        protected virtual void OnEnable()
        {
            // Hook events
            LeanTouch.OnFingerUp += FingerUp;
        }

        protected virtual void OnDisable()
        {
            // Unhook events
            LeanTouch.OnFingerUp -= FingerUp;
        }

        private void FingerUp(LeanFinger finger)
        {
            // Ignore?
            if (IgnoreIfOverGui && finger.IsOverGui) return;

            if (IgnoreIfStartedOverGui && finger.StartedOverGui) return;

            // Call event
            OnFingerUp.Invoke(finger);
        }

        // Event signature
        [Serializable]
        public class LeanFingerEvent : UnityEvent<LeanFinger>
        {
        }
    }
}