using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentEnergyView : MonoBehaviour
{
    [SerializeField] private RocketControl _rocketControl;
    [SerializeField] private Slider _slider;

    private void Update()
    {
        _slider.normalizedValue = _rocketControl.CurrentBoostNormalized;
    }
}
