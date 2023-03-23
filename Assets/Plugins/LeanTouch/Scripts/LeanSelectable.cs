using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
    // This component allows you to select this GameObject via another component
    public class LeanSelectable : MonoBehaviour
    {
        public static List<LeanSelectable> Instances = new List<LeanSelectable>();

        [Tooltip("Should IsSelected temporarily return false if the selecting finger is still being held?")]
        public bool HideWithFinger;

        // Called when selection begins (finger = the finger that selected this)
        public LeanFingerEvent OnSelect;

        // Called when the selecting finger goes up (finger = the finger that selected this)
        public LeanFingerEvent OnSelectUp;

        // Called when this is deselected, if OnSelectUp hasn't been called yet, it will get called first
        public UnityEvent OnDeselect;

        // Is this selectable selected?
        [SerializeField] private bool isSelected;

        // This stores the finger that began selection of this LeanSelectable
        // This will become null as soon as that finger releases, which you can detect via OnSelectUp
        [NonSerialized] public LeanFinger SelectingFinger;

        public bool IsSelected
        {
            get
            {
                // Hide IsSelected?
                if (HideWithFinger && isSelected && SelectingFinger != null) return false;

                return isSelected;
            }
        }

        protected virtual void LateUpdate()
        {
            // Null the selecting finger?
            // NOTE: This is done in LateUpdate so certain OnFingerUp actions that require checking SelectingFinger can still work properly
            if (SelectingFinger != null)
                if (SelectingFinger.Set == false || isSelected == false)
                    SelectingFinger = null;
        }

        protected virtual void OnEnable()
        {
            // Register instance
            Instances.Add(this);
        }

        protected virtual void OnDisable()
        {
            // Unregister instance
            Instances.Remove(this);

            if (isSelected) Deselect();
        }

        [ContextMenu("Select")]
        public void Select()
        {
            Select(null);
        }

        // NOTE: Multiple selection is allowed
        public void Select(LeanFinger finger)
        {
            isSelected = true;
            SelectingFinger = finger;

            if (OnSelect != null) OnSelect.Invoke(finger);
        }

        [ContextMenu("Deselect")]
        public void Deselect()
        {
            // Make sure we don't deselect multiple times
            if (isSelected)
            {
                isSelected = false;

                if (SelectingFinger != null && OnSelectUp != null) OnSelectUp.Invoke(SelectingFinger);

                if (OnDeselect != null) OnDeselect.Invoke();

                // NOTE: SelectingFinger is set to null in LateUpdate
            }
        }

        // Event signature
        [Serializable]
        public class LeanFingerEvent : UnityEvent<LeanFinger>
        {
        }
    }
}