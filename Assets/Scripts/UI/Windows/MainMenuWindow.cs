using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : AbstractWindow
{
	[SerializeField] private Button _playButton;
	[SerializeField] private Button _closeAppButton;
	public static MainMenuWindow Of() =>
					Game.Windows.ScreenChange<MainMenuWindow>(false, w => w.Init());

	protected void Init()
	{
		_playButton.onClick.RemoveAllListeners();
		_playButton.onClick.AddListener(StartGame);

		_closeAppButton.onClick.AddListener(OnButtonCloseClick);
	}

	private void StartGame()
    {
		Game.LevelLoader.Init();
		Close();
    }

	private void OnButtonCloseClick()
    {
		Debug.Log("On close click");
		Application.Quit();
    }
}
