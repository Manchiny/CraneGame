using RSG;
using System.Collections;
using UnityEngine;
using static LevelConfigs;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private LevelConfigs _levelConfigs;
    private LevelConfig[] _levels => _levelConfigs.Configs;
    private LevelConfig _levelConfig;
    public void Init()
    {
        var loader = LoadingWindow.Of();
        _levelConfig = GetLevel();

        LevelManager.Instance.StartLevel(_levelConfig)
            .Then(() => AwaitingPromise(1f))
            .Then(() => loader.Close());
    }

    private LevelConfig GetLevel()
    {
        return _levels[0];
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

    public void ExitLevel()
    {

    }
    
}
