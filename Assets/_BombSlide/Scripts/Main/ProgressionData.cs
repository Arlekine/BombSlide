using UnityEngine;

public class ProgressionData : Singletone<ProgressionData>
{
    [SerializeField] private int _startMoney = 50;
    [SerializeField] private int _baseUpgradeCost = 100;
    [SerializeField] private int additionalUpgradeCost = 80;

    [Space]
    [SerializeField] private float _speedForUpgrade = 15;
    [SerializeField] private float _boostForUpgrade = 10;
    [SerializeField] private float _explosionForceForUpgrade = 10000;
    [SerializeField] private float _explosionRadiusForUpgrade = 10;

    [Space] 
    [SerializeField] private float _moneyForDistanceInMeters = 0.1f;
    [SerializeField] private int _moneyForDestraction = 100;
    [SerializeField] private int _baseMoneyForLevelPass = 300;
    [SerializeField] private int _additionalMoneyForLevel = 300;

    public int StartMoney => _startMoney;
    public int BaseUpgradeCost => _baseUpgradeCost;
    public int AdditionalUpgradeCost => additionalUpgradeCost;
    public float SpeedForUpgrade => _speedForUpgrade;
    public float BoostForUpgrade => _boostForUpgrade;
    public float ExplosionForceForUpgrade => _explosionForceForUpgrade;
    public float ExplosionRadiusForUpgrade => _explosionRadiusForUpgrade;
    public float MoneyForDistanceInMeters => _moneyForDistanceInMeters;
    public int MoneyForDestraction => _moneyForDestraction;
    public int BaseMoneyForLevelPass => _baseMoneyForLevelPass;
    public int AdditionalMoneyForLevel => _additionalMoneyForLevel;
}