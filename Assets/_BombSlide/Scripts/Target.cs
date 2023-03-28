using System.Collections;
using System.Collections.Generic;
using ShatterToolkit;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private int _cost;

    public int Cost => _cost;
    public ShatterTool ShatterTool => GetComponent<ShatterTool>();
}
