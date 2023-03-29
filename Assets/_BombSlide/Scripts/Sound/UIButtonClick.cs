using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource), typeof(Button))]
public class UIButtonClick : MonoBehaviour
{
    private AudioSource _audioSource;
    private Button _button;

    private Button Button
    {
        get
        {
            if (_button == null)
                _button = GetComponent<Button>();

            return _button;
        }
    }
    
    private AudioSource AudioSource
    {
        get
        {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            return _audioSource;
        }
    }

    private void OnEnable()
    {
        Button.onClick.AddListener(PlaySound);
    }

    private void OnDisable()
    {
        Button.onClick.RemoveListener(PlaySound);
    }

    private void PlaySound()
    {
        AudioSource.loop = false;
        AudioSource.Play();
    }
}
