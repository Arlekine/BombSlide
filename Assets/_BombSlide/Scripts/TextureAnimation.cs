using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAnimation : MonoBehaviour
{
    [SerializeField] private Vector2 _animationVector;
    [SerializeField] private Renderer _renderer;

    private void Update()
    {
        _renderer.material.mainTextureOffset += _animationVector * Time.deltaTime;
    }
}
