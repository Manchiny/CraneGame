using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScene : MonoBehaviour
{
    [SerializeField] private LoaderWindow _window;

    private const int GameSceneId = 1;

    private const float MaxProgressSliderValue = 0.999f;
    private const float ProgressBarSlowdownFactor = 0.9f;
    private const float SecondsDelayAfterSceneLoaded = 0.5f;

    private void Start()
    {
        _window.Init();
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(GameSceneId);
        operation.allowSceneActivation = false;

        while (operation.isDone == false)
        {
            float progress = operation.progress / ProgressBarSlowdownFactor;
            _window.SetProgress(progress);

            if (_window.GetProgessBarValue() >= MaxProgressSliderValue)
            {
                yield return new WaitForSeconds(SecondsDelayAfterSceneLoaded);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}

