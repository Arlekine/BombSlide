using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyPart : DestractablePart
{
    protected override void OnHitted(Vector3 hitPoint)
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
    }

}