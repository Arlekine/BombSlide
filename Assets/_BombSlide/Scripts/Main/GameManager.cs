using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameManager : Singletone<GameManager>
{
    private const string GameDataPlayerPrefs = "GameData";

    [SerializeField] private CameraBahaviour _camera;
    [SerializeField] private Level[] _levels;
    [SerializeField] private MoneyTracker _moneyTracker;
    [SerializeField] private UpgradesPanel _upgrades;

    [Header("UI")]
    [SerializeField] private Fade _fade;
    [SerializeField] private LevelTarget _levelTarget;
    [SerializeField] private StartScreen _startScreen;
    [SerializeField] private BoostButton _boostButton;
    [SerializeField] private CurrentEnergyView _energyView;
    [SerializeField] private TextMeshProUGUI _currentMoney;
    [SerializeField] private TextMeshProUGUI _levelPassText;
    [SerializeField] private SwitchButton _hapticButton;
    [SerializeField] private SwitchButton _soundButton;

    [Header("Tutorial")] 
    [SerializeField] private FlightTutorial _flightTutorial;

    [Space] 
    [SerializeField] private float _timeBeforeLevelRestart;

    private GameData _gameData;
    private Level _currentLevel;
    private Sequence _levelPassAnimation;

    public bool HapticOn => _gameData.HapticOn;
    public int CompletedLevels => _gameData.CompletedLevels;
    public bool IsTutorial => _gameData.IsTutorial;
    public bool IsUpgradesTutorial;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(GameDataPlayerPrefs))
        {
            _gameData = JsonUtility.FromJson<GameData>(PlayerPrefs.GetString(GameDataPlayerPrefs));
        }
        else
        {
            _gameData = new GameData();
            _gameData.CurrentMoney = ProgressionData.Instance.StartMoney;
            SaveData();
        }

        if (_levels.Length < _gameData.CurrentLevel)
        {
            _gameData.CurrentLevel = 0;
            SaveData();
        }

        OpenLevel(_levels[_gameData.CurrentLevel]);

        _startScreen.Clicked += StartLevel;
        _moneyTracker.GotMoney += UpdateCurrentMoney;

        _currentMoney.text = $"{_gameData.CurrentMoney}";

        _upgrades.SetData(_gameData.SpeedUpgradeInteration, _gameData.BoostUpgradeInteration, _gameData.ExplosionUpgradeInteration);
        _upgrades.UpdateButtonsInteractivity(_gameData.CurrentMoney);

        _upgrades.SpeedUpgraded.AddListener(UpgradeSpeed);
        _upgrades.BoostUpgraded.AddListener(UpgradeBoost);
        _upgrades.ExplosionUpgraded.AddListener(UpgradeExplosion);

        AudioListener.volume = _gameData.SoundOn ? 1f : 0f;

        _hapticButton.SetState(_gameData.HapticOn);
        _soundButton.SetState(_gameData.SoundOn);

        _soundButton.OnSwitch += HapticSwitch;
        _soundButton.OnSwitch += SoundSwitch;

        _startScreen.gameObject.SetActive(true);
    }

    public void EndTutorial()
    {
        IsUpgradesTutorial = false;
        _gameData.IsTutorial = false;
        SaveData();
    }

    [EditorButton]
    public void Add1000Money()
    {
        UpdateCurrentMoney(1000);
        _upgrades.UpdateButtonsInteractivity(_gameData.CurrentMoney);
    }

    public bool IsMainTarget(Target target)
    {
        return target == _currentLevel.TargetToPass;
    }

    public void HapticSwitch(bool isActive)
    {
        _gameData.HapticOn = isActive;
        SaveData();
    }

    public void SoundSwitch(bool isActive)
    {
        _gameData.SoundOn = isActive;
        AudioListener.volume = _gameData.SoundOn ? 1f : 0f;
        SaveData();
    }

    private void UpdateCurrentMoney(int newMoney)
    {
        _gameData.CurrentMoney += newMoney;
        SaveData();
        _currentMoney.text = $"{_gameData.CurrentMoney}";
    }

    private void UpgradeSpeed(float speed, int cost, int interations)
    {
        _gameData.SpeedUpgradeInteration = interations;
        _gameData.CurrentMoney -= cost;
        _gameData.AdditionalSpeed += speed;

        _currentMoney.text = $"{_gameData.CurrentMoney}";

        SaveData();
        
        if (_currentLevel != null)
            _currentLevel.RocketControl.AddSpeed(speed);

        _upgrades.UpdateButtonsInteractivity(_gameData.CurrentMoney);
    }

    private void UpgradeBoost(float boost, int cost, int interations)
    {
        _gameData.BoostUpgradeInteration = interations;
        _gameData.CurrentMoney -= cost;
        _gameData.AdditionalBoost += boost;

        _currentMoney.text = $"{_gameData.CurrentMoney}";

        SaveData();

        if (_currentLevel != null)
            _currentLevel.RocketControl.AddBoost(boost);

        _upgrades.UpdateButtonsInteractivity(_gameData.CurrentMoney);
    }

    private void UpgradeExplosion(float explosionRadius, float explosionForce, int cost, int interations)
    {
        _gameData.BoostUpgradeInteration = interations;
        _gameData.CurrentMoney -= cost;
        _gameData.AdditionalExplosionForce += explosionForce;
        _gameData.AdditionalExplosionRadius += explosionRadius;

        _currentMoney.text = $"{_gameData.CurrentMoney}";

        SaveData();

        if (_currentLevel != null)
            _currentLevel.RocketControl.AddExplosion(explosionRadius, explosionForce);

        _upgrades.UpdateButtonsInteractivity(_gameData.CurrentMoney);
    }

    private void OpenLevel(Level prefab)
    {
        if (_currentLevel != null)
        {
            _currentLevel.RocketControl.ObstacleHitted.RemoveAllListeners();
            Destroy(_currentLevel.gameObject);
        }

        _startScreen.gameObject.SetActive(true);
        _currentLevel = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        _levelTarget.SetData(_currentLevel.TargetToPass, _currentLevel.RocketControl);

        _upgrades.UpdateButtonsInteractivity(_gameData.CurrentMoney);

        _currentLevel.RocketControl.AddSpeed(_gameData.AdditionalSpeed);
        _currentLevel.RocketControl.AddBoost(_gameData.AdditionalBoost);
        _currentLevel.RocketControl.AddExplosion(_gameData.AdditionalExplosionRadius, _gameData.AdditionalExplosionForce);

        _currentLevel.CameraControl.SetCamera(_camera);
        _currentLevel.RocketControl.ObstacleHitted.AddListener(EndLevel);
        _moneyTracker.SetRocket(_currentLevel.RocketControl);
        _energyView.SetRocket(_currentLevel.RocketControl); 
        _boostButton.SetRocket(_currentLevel.RocketControl);

        if (IsTutorial)
        {
            if (IsUpgradesTutorial)
                _upgrades.StartTutorial();
            else
                _flightTutorial.SetRocket(_currentLevel.RocketControl);
        }
    }

    private void SaveData()
    {
        PlayerPrefs.SetString(GameDataPlayerPrefs, JsonUtility.ToJson(_gameData));
    }

    private void StartLevel()
    {
        _currentLevel.RocketControl.StartMoving();
    }

    private void EndLevel(Target target)
    {
        if (target == _currentLevel.TargetToPass)
        {
            _gameData.CurrentLevel++;

            _gameData.AdditionalSpeed = 0;
            _gameData.AdditionalBoost = 0;
            _gameData.AdditionalExplosionForce = 0;
            _gameData.AdditionalExplosionRadius = 0;
            _gameData.CompletedLevels++;

            if (_gameData.CurrentLevel >= _levels.Length)
                _gameData.CurrentLevel = 0;
            
            _levelPassAnimation = DOTween.Sequence();

            _levelPassAnimation.Append(_levelPassText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
            _levelPassAnimation.Join(_levelPassText.DOFade(1f, 0.5f));

            SaveData();
        }

        StartCoroutine(LevelEndRoutine());
    }

    private IEnumerator LevelEndRoutine()
    {
        yield return new WaitForSeconds(_timeBeforeLevelRestart);

        _fade.FadeIn(() =>
        {
            _levelPassText.DOFade(0f, 0f);
            _camera.Camera.fieldOfView = 40f;
            _camera.SpeedEffect.gameObject.SetActive(false);
            OpenLevel(_levels[_gameData.CurrentLevel]);
            _fade.FadeOut();
        });

    }
}

public class GameData
{
    public int CurrentLevel;
    public int CompletedLevels;
    public int CurrentMoney;
    public bool IsTutorial = true;

    public bool SoundOn = true;
    public bool HapticOn = true;

    public int SpeedUpgradeInteration;
    public int BoostUpgradeInteration;
    public int ExplosionUpgradeInteration;

    public float AdditionalSpeed;
    public float AdditionalBoost;
    public float AdditionalExplosionForce;
    public float AdditionalExplosionRadius;
}