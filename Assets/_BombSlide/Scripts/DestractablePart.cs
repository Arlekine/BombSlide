using UnityEngine;

public abstract class DestractablePart : MonoBehaviour
{
    private Target _parent;

    public void SetParentTarget(Target target)
    {
        _parent = target;
    }

    public Target Hit(Vector3 hitPoint)
    {
        OnHitted(hitPoint);

        if (_parent != null)
            _parent.InvokeTargetHit();

        return _parent;
    }

    protected abstract void OnHitted(Vector3 hitPoint);
}