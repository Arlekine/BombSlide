using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string GameDataPlayerPrefs = "GameData";

    [SerializeField] private CameraBahaviour _camera;
    [SerializeField] private Level[] _levels;
    [SerializeField] private MoneyTracker _moneyTracker;
    [SerializeField] private UpgradesPanel _upgrades;

    [Header("UI")]
    [SerializeField] private Fade _fade;
    [SerializeField] private StartScreen _startScreen;
    [SerializeField] private BoostButton _boostButton;
    [SerializeField] private CurrentEnergyView _energyView;
    [SerializeField] private TextMeshProUGUI _currentMoney;

    [Space] 
    [SerializeField] private float _timeBeforeLevelRestart;

    private GameData _gameData;
    private Level _currentLevel;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(GameDataPlayerPrefs))
        {
            _gameData = JsonUtility.FromJson<GameData>(PlayerPrefs.GetString(GameDataPlayerPrefs));
        }
        else
        {
            _gameData = new GameData();
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

        _currentMoney.text = $"{_gameData.CurrentMoney} $";

        _upgrades.SetData(_gameData.SpeedUpgradeInteration, _gameData.BoostUpgradeInteration, _gameData.ExplosionUpgradeInteration);
        _upgrades.UpdateButtonsInteractivity(_gameData.CurrentMoney);

        _upgrades.SpeedUpgraded.AddListener(UpgradeSpeed);
        _upgrades.BoostUpgraded.AddListener(UpgradeBoost);
        _upgrades.ExplosionUpgraded.AddListener(UpgradeExplosion);
    }

    private void UpdateCurrentMoney(int newMoney)
    {
        _gameData.CurrentMoney += newMoney;
        SaveData();
        _currentMoney.text = $"{_gameData.CurrentMoney} $";
    }

    private void UpgradeSpeed(float speed, int cost, int interations)
    {
        _gameData.SpeedUpgradeInteration = interations;
        _gameData.CurrentMoney -= cost;
        _gameData.AdditionalSpeed += speed;

        _currentMoney.text = $"{_gameData.CurrentMoney} $";

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

        _currentMoney.text = $"{_gameData.CurrentMoney} $";

        SaveData();

        if (_currentLevel != null)
            _currentLevel.RocketControl.AddBoost(boost);

        _upgrades.UpdateButtonsInteractivity(_gameData.CurrentMoney);
    }

    private void UpgradeExplosion(float explosionRadius, float explosionForce, int cost, int interations)
    {
        _gameData.BoostUpgradeInteration = interations;
        _gameData.CurrentMoney -= cost;
        _gameData.AdditionalExplosionForce += explosionRadius;
        _gameData.AdditionalExplosionRadius += explosionForce;

        _currentMoney.text = $"{_gameData.CurrentMoney} $";

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

        _upgrades.UpdateButtonsInteractivity(_gameData.CurrentMoney);

        _currentLevel.RocketControl.AddSpeed(_gameData.AdditionalSpeed);
        _currentLevel.RocketControl.AddBoost(_gameData.AdditionalBoost);
        _currentLevel.RocketControl.AddExplosion(_gameData.AdditionalExplosionRadius, _gameData.AdditionalExplosionForce);

        _currentLevel.CameraControl.SetCamera(_camera);
        _currentLevel.RocketControl.ObstacleHitted.AddListener(EndLevel);
        _moneyTracker.SetRocket(_currentLevel.RocketControl);
        _energyView.SetRocket(_currentLevel.RocketControl); 
        _boostButton.SetRocket(_currentLevel.RocketControl);
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

            if (_gameData.CurrentLevel >= _levels.Length)
                _gameData.CurrentLevel = 0;

            SaveData();
        }

        StartCoroutine(LevelEndRoutine());
    }

    private IEnumerator LevelEndRoutine()
    {
        yield return new WaitForSeconds(_timeBeforeLevelRestart);

        _fade.FadeIn(() =>
        {
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
    public int CurrentMoney;

    public int SpeedUpgradeInteration;
    public int BoostUpgradeInteration;
    public int ExplosionUpgradeInteration;

    public float AdditionalSpeed;
    public float AdditionalBoost;
    public float AdditionalExplosionForce;
    public float AdditionalExplosionRadius;
}