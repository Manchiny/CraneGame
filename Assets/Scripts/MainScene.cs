using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScene : MonoBehaviour
{
    private const int GAME_SCENE_ID = 1;

    [SerializeField] private LoaderWindow _window;

    private void Start()
    {
        _window.Init();
        StartCoroutine(LoadGameScene());
    }
    private IEnumerator LoadGameScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(GAME_SCENE_ID);
        operation.allowSceneActivation = false;
        while(!operation.isDone)
        {
            float progress = operation.progress/0.9f;
            _window.SetProgress(progress);
            if(_window.GetProgessBarValue() >= 0.999f)
            {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}

