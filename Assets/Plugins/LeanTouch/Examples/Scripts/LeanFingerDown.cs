using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
    // This script calls the OnFingerDown event when a finger touches the screen
    public class LeanFingerDown : MonoBehaviour
    {
        [Tooltip("If the finger is over the GUI, ignore it?")]
        public bool IgnoreIfOverGui;

        public LeanFingerEvent OnFingerDown;

        protected virtual void OnEnable()
        {
            // Hook events
            LeanTouch.OnFingerDown += FingerDown;
        }

        protected virtual void OnDisable()
        {
            // Unhook events
            LeanTouch.OnFingerDown -= FingerDown;
        }

        private void FingerDown(LeanFinger finger)
        {
            // Ignore?
            if (IgnoreIfOverGui && finger.IsOverGui) return;

            // Call event
            OnFingerDown.Invoke(finger);
        }

        // Event signature
        [Serializable]
        public class LeanFingerEvent : UnityEvent<LeanFinger>
        {
        }
    }
}