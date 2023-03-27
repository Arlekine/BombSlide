using System.Collections;
using System.Collections.Generic;
using ShatterToolkit;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private int _cost;
    [SerializeField] private ShatterTool _shatterTool;

    public int Cost => _cost;
    public ShatterTool ShatterTool => _shatterTool;
}
