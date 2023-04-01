using System.Collections;
using Lean.Touch;
using UnityEngine;

public class FlightTutorial : MonoBehaviour
{
    [SerializeField] private GameObject _moveTutorial;
    [SerializeField] private float _moveTutorialDelay = 1f;
    [SerializeField] private GameObject _boostTutorial;
    [SerializeField] private float _boostTutorialDelay = 1f;

    private RocketControl _rocket;

    public void SetRocket(RocketControl rocket)
    {
        _rocket = rocket;
        _rocket.FreeFlightStarted.AddListener(StartMoveTutorial);
    }

    private void StartMoveTutorial()
    {
        _rocket.FreeFlightStarted.RemoveListener(StartMoveTutorial);
        StartCoroutine(MoveTutorialRoutine());
    }

    private IEnumerator MoveTutorialRoutine()
    {
        yield return new WaitForSeconds(_moveTutorialDelay);

        Time.timeScale = 0.01f;
        _moveTutorial.SetActive(true);

        while (true)
        {
            var fingers = LeanTouch.GetFingers(true);

            if (fingers.Count > 0)
            {
                Time.timeScale = 1f;
                _moveTutorial.SetActive(false);
                StartCoroutine(BoostTutorialRoutine());

                break;
            }

            yield return null;
        }
    }

    private IEnumerator BoostTutorialRoutine()
    {
        yield return new WaitForSeconds(_moveTutorialDelay);

        Time.timeScale = 0.01f;
        _boostTutorial.SetActive(true);
        _rocket.BoostStart.AddListener(EndBoostTutorial);
    }

    private void EndBoostTutorial()
    {
        _rocket.BoostStart.RemoveListener(EndBoostTutorial);
        Time.timeScale = 1f;
        _boostTutorial.SetActive(false);
    }
}