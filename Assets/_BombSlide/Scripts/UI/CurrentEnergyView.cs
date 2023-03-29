using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class  CurrentEnergyView : MonoBehaviour
{
    [SerializeField] private Image _slider;

    private RocketControl _rocketControl;

    public void SetRocket(RocketControl rocket)
    {
        _rocketControl = rocket;
    }

    private void Update()
    {
        if (_rocketControl != null)
            _slider.fillAmount = _rocketControl.CurrentBoostNormalized;
    }
}
