using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
    // This script calls the OnFingerSet event while a finger is touching the screen
    public class LeanFingerSet : MonoBehaviour
    {
        [Tooltip("If the finger is over the GUI, ignore it?")]
        public bool IgnoreIfOverGui;

        [Tooltip("If the finger started over the GUI, ignore it?")]
        public bool IgnoreIfStartedOverGui;

        public LeanFingerEvent OnFingerSet;

        protected virtual void OnEnable()
        {
            // Hook events
            LeanTouch.OnFingerSet += FingerSet;
        }

        protected virtual void OnDisable()
        {
            // Unhook events
            LeanTouch.OnFingerSet -= FingerSet;
        }

        private void FingerSet(LeanFinger finger)
        {
            // Ignore?
            if (IgnoreIfOverGui && finger.IsOverGui) return;

            if (IgnoreIfStartedOverGui && finger.StartedOverGui) return;

            // Call event
            OnFingerSet.Invoke(finger);
        }

        // Event signature
        [Serializable]
        public class LeanFingerEvent : UnityEvent<LeanFinger>
        {
        }
    }
}