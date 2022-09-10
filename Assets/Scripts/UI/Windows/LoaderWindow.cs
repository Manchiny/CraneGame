using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoaderWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Slider _progressBar;

    public void Init()
    {
        _progressBar.value = 0;
        _title.text = "Портовый\nкран";
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
