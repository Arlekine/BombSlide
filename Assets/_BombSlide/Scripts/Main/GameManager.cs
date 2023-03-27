using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string GameDataPlayerPrefs = "GameData";

    [SerializeField] private Camera _camera;
    [SerializeField] private Level[] _levels;
    [SerializeField] private MoneyTracker _moneyTracker;

    [Header("UI")]
    [SerializeField] private Fade _fade;
    [SerializeField] private StartScreen _startScreen;
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
    }

    private void UpdateCurrentMoney(int newMoney)
    {
        _gameData.CurrentMoney += newMoney;
        SaveData();
        _currentMoney.text = $"{_gameData.CurrentMoney} $";
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

        _currentLevel.CameraControl.SetCamera(_camera.transform);
        _currentLevel.RocketControl.ObstacleHitted.AddListener(EndLevel);
        _moneyTracker.SetRocket(_currentLevel.RocketControl);
        _energyView.SetRocket(_currentLevel.RocketControl);
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
            OpenLevel(_levels[_gameData.CurrentLevel]);
            _fade.FadeOut();
        });

    }
}

public class GameData
{
    public int CurrentLevel;
    public int CurrentMoney;
}