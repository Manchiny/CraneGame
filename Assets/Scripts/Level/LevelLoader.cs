using RSG;
using System;
using System.Collections;
using UnityEngine;
using static LevelConfigs;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private LevelConfigs _levelConfigs;
    private LevelConfig[] _levels => _levelConfigs.Configs;
    private LevelConfig _levelConfig;

    private LoadingWindow _loader;

    public event Action LoadingCompleted;
    public event Action LevelExited;

    public IPromise StartGame(Action onComplete)
    {
        Promise promise = new Promise();
        _levelConfig = GetLevel();

        if (_levelConfig != null)
        {         
            _loader = LoadingWindow.Show();

            Crane crane = FindObjectOfType<Crane>();
            crane.Init();

            var window = GameMainWindow.Show(crane, _levelConfig);

            Game.LevelManager.StartLevel(_levelConfig, this, window);

            AwaitingPromise(1f)
            .Then(() => _loader?.Close())
            .Then(() =>
            {
                LoadingCompleted?.Invoke();
                onComplete?.Invoke();
            })
            .Then(() => promise.Resolve());
        }
        else
        {
            Game.Locker.ClearAllLocks();
            NoMoreLevelsWindow.Show();

            return Promise.Resolved();
        }

        return promise;
    }

    public void ExitLevel(Action onComplete)
    {
        _loader = LoadingWindow.Show();
        LevelExited?.Invoke();

        AwaitingPromise(1f)
        .Then(() => _loader?.Close())
        .Then(() => MainMenuWindow.Show())
        .Then(() =>
        {
            LoadingCompleted?.Invoke();
            onComplete?.Invoke();
        });
    }

    private LevelConfig GetLevel()
    {
        int levelId = Game.User.CurrentLevel;

        if (levelId < _levels.Length)
            return _levels[levelId];
        else
            return null;
    }

    private IPromise AwaitingPromise(float seconds)
    {
        Promise promise = new Promise();
        StartCoroutine(Timer());

        IEnumerator Timer()
        {
            yield return new WaitForSeconds(seconds);
            promise.Resolve();
        }

        return promise;
    }
}
