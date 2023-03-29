using ShatterToolkit;
using UnityEngine;

[RequireComponent(typeof(ShatterTool))]
public class ShatterPart : DestractablePart
{
    private ShatterTool _shatterTool;

    protected override void OnHitted(Vector3 hitPoint)
    {
        _shatterTool = GetComponent<ShatterTool>();
        _shatterTool.Shatter(hitPoint);
    }
}