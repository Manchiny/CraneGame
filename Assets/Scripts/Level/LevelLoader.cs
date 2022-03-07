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

    // private CompositeDisposable _dispose = new CompositeDisposable();
    private LoadingWindow _loader;

    public Action OnLoadingComplete;
    public Action OnExitLevel;
    public IPromise StartGame(Action onComplete)
    {
        Promise promise = new Promise();
        _levelConfig = GetLevel();

        if (_levelConfig != null)
        {         
            _loader = LoadingWindow.Of();

            Crane crane = FindObjectOfType<Crane>();
            crane.Init();

            var window = GameMainWindow.Of(crane, _levelConfig);

            Game.LevelManager.StartLevel(_levelConfig, this, window)
            .Then(() => AwaitingPromise(1f))
            .Then(() => _loader?.Close())
            .Then(() =>
            {
                OnLoadingComplete?.Invoke();
                onComplete?.Invoke();
            })
            .Then(() => promise.Resolve());
        }
        else
        {
            Game.Locker.ClearAllLocks();
            NoMoreLevelsWindow.Of();
            return Promise.Resolved();
        }

        return promise;
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

    public void ExitLevel(Action onComplete)
    {
        _loader = LoadingWindow.Of();
        OnExitLevel?.Invoke();

        AwaitingPromise(1f)
        .Then(() => _loader?.Close())
        .Then(() => MainMenuWindow.Of())
        .Then(() =>
        {
            OnLoadingComplete?.Invoke();
            onComplete?.Invoke();
        });
    }
}
