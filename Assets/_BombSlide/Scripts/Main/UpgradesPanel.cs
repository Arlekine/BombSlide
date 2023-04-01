using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradesPanel : MonoBehaviour
{
    public UnityEvent<float, int, int> SpeedUpgraded;
    public UnityEvent<float, int, int> BoostUpgraded;
    public UnityEvent<float, float, int, int> ExplosionUpgraded;

    [SerializeField] private UpgradeButton _speedUpgrade;
    [SerializeField] private UpgradeButton _boostUpgrade;
    [SerializeField] private UpgradeButton _explosionUpgrade;

    [Header("Tutorial")] 
    [SerializeField] private GameObject _speedUpgradeTutorial;
    [SerializeField] private GameObject _boostUpgradeTutorial;
    [SerializeField] private GameObject _explosionUpgradeTutorial;

    private void Awake()
    {
        _speedUpgrade.Clicked.AddListener(UpgradeSpeed);
        _boostUpgrade.Clicked.AddListener(UpgradeBoost);
        _explosionUpgrade.Clicked.AddListener(UpgradeExplosion);
    }

    public void StartTutorial()
    {
        _speedUpgradeTutorial.SetActive(true);
    }

    private void UpgradeSpeed(int cost, int costInteration)
    {
        if (GameManager.Instance.IsTutorial)
        {
            _speedUpgradeTutorial.SetActive(false);
        _boostUpgradeTutorial.SetActive(true);
        }

        SpeedUpgraded?.Invoke(ProgressionData.Instance.SpeedForUpgrade, cost, costInteration);
    }

    private void UpgradeBoost(int cost, int costInteration)
    {
        if (GameManager.Instance.IsTutorial)
        {
            _boostUpgradeTutorial.SetActive(false);
            _explosionUpgradeTutorial.SetActive(true);
        }

        BoostUpgraded?.Invoke(ProgressionData.Instance.BoostForUpgrade, cost, costInteration);
    }
    
    private void UpgradeExplosion(int cost, int costInteration)
    {
        if (GameManager.Instance.IsTutorial)
            _explosionUpgradeTutorial.SetActive(false);
        ExplosionUpgraded?.Invoke(ProgressionData.Instance.ExplosionRadiusForUpgrade, ProgressionData.Instance.ExplosionForceForUpgrade, cost, costInteration);

        GameManager.Instance.EndTutorial();
    }

    public void SetData(int speedInterations, int boostInterations, int explosionInterations)
    {
        _speedUpgrade.SetCurrentCostIteration(speedInterations);
        _boostUpgrade.SetCurrentCostIteration(boostInterations);
        _explosionUpgrade.SetCurrentCostIteration(explosionInterations);
    }

    public void UpdateButtonsInteractivity(int currentMoney)
    {
        _speedUpgrade.SetCurrentMoney(currentMoney);
        _boostUpgrade.SetCurrentMoney(currentMoney);
        _explosionUpgrade.SetCurrentMoney(currentMoney);
    }
}