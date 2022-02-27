using UnityEngine;
using static LevelConfigs;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private LevelConfigs _levelConfigs;
    private LevelConfig[] _levels => _levelConfigs.Configs;
    private LevelConfig _levelConfig; 
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _levelConfig = GetLevel();

        LevelManager.Instance.Init(_levelConfig);
    }

    private LevelConfig GetLevel()
    {
        return _levels[0];
    }
}
