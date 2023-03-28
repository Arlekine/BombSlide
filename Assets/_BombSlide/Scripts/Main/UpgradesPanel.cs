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

    [Space] 
    [SerializeField] private float _speedForUpgrade;
    [SerializeField] private float _boostForUpgrade;
    [SerializeField] private float _explosionForceForUpgrade;
    [SerializeField] private float _explosionRadiusForUpgrade;

    private void Awake()
    {
        _speedUpgrade.Clicked.AddListener(UpgradeSpeed);
        _boostUpgrade.Clicked.AddListener(UpgradeBoost);
        _explosionUpgrade.Clicked.AddListener(UpgradeExplosion);
    }

    private void UpgradeSpeed(int cost, int costInteration)
    {
        SpeedUpgraded?.Invoke(_speedForUpgrade, cost, costInteration);
    }

    private void UpgradeBoost(int cost, int costInteration)
    {
        BoostUpgraded?.Invoke(_boostForUpgrade, cost, costInteration);
    }
    
    private void UpgradeExplosion(int cost, int costInteration)
    {
        ExplosionUpgraded?.Invoke(_explosionRadiusForUpgrade, _explosionForceForUpgrade, cost, costInteration);
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