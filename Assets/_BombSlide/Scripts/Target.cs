using System;
using System.Collections;
using System.Collections.Generic;
using ShatterToolkit;
using UnityEngine;
using UnityEngine.Animations;

public class Target : MonoBehaviour
{
    [Serializable]
    public class DestractionReaction
    {
        public ShatterTool ShatterTool;
        public Transform Point;

        public void Destroy()
        {
            ShatterTool.Shatter(Point.position);
        }
    }

    [SerializeField] private int _cost;
    [SerializeField] private DestractionReaction[] _destractionReactions;
    [SerializeField] private RigidbodyPart[] _rigidbodies;
    [SerializeField] private ParentConstraint[] _constraintsToDelete;
    [SerializeField] private PositionConstraint[] _positionsToDelete;

    private DestractablePart[] _destractableParts;
    private bool _isDestroyed;

    public int Cost => _cost;

    private void Start()
    {
        _destractableParts = GetComponentsInChildren<DestractablePart>();

        foreach (var destractablePart in _destractableParts)
        {
            if (destractablePart.gameObject != gameObject)
            {
                destractablePart.transform.parent = transform.parent;
            }

            destractablePart.SetParentTarget(this);
        }
    }

    public void InvokeTargetHit()
    {
        if (_isDestroyed)
            return;

        _isDestroyed = true;

        foreach (var destractionReaction in _destractionReactions)
        {
            destractionReaction.Destroy();
        }

        foreach (var rigidbodyPart in _rigidbodies)
        {
            rigidbodyPart.Hit(rigidbodyPart.transform.position);
        }

        foreach (var component in _constraintsToDelete)
        {
            Destroy(component);
        }

        foreach (var component in _positionsToDelete)
        {
            Destroy(component);
        }
    }
}