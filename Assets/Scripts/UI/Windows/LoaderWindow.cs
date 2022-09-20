using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoaderWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _versionText;
    [SerializeField] private Slider _progressBar;

    public void Init()
    {
        _progressBar.value = 0;
        _versionText.text = "VER.: " + Application.version;
    }

    public void SetProgress(float progress)
    {
        _progressBar.value = Mathf.Lerp(_progressBar.value, progress, 2 * Time.deltaTime);
    }

    public float GetProgessBarValue()
    {
        return _progressBar.value;
    }
}
