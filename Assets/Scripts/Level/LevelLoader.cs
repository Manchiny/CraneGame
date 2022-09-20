using RSG;
using System;
using UnityEngine;
using static LevelConfigs;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private LevelConfigs _levelConfigs;
    private LevelConfig[] _levels => _levelConfigs.Configs;
    private LevelConfig _levelConfig;

    private LoadingWindow _loader;

    public event Action LevelLoaded;
    public event Action LevelExited;

    public IPromise StartGame()
    {
        Promise promise = new Promise();
        _levelConfig = GetLevel();

        if (_levelConfig != null)
        {         
            _loader = LoadingWindow.Show();

            Crane crane = FindObjectOfType<Crane>();
            crane.Init();

            var window = GameHUDWindow.Show(crane, _levelConfig);

            Game.Level.StartLevel(_levelConfig, this, window);

            Utils.WaitSeconds(1f)
                .Then(() => _loader?.Close())
                .Then(() => LevelLoaded?.Invoke())
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

        Utils.WaitSeconds(1f)
            .Then(() => _loader?.Close())
            .Then(() => MainMenuWindow.Show())
            .Then(() => onComplete?.Invoke());
    }

    private LevelConfig GetLevel()
    {
        int levelId = Game.User.CurrentLevel;

        if (levelId < _levels.Length)
            return _levels[levelId];
        else
            return null;
    }
}
