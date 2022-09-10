using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : AbstractWindow
{
    [SerializeField] private Button _playButton;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Button _closeAppButton;

    private const string LockKey = "MenuStartGame";

    public static MainMenuWindow Show() =>
                    Game.Windows.ScreenChange<MainMenuWindow>(false, w => w.Init());

    protected void Init()
    {
        _playButton.onClick.RemoveAllListeners();
        _playButton.onClick.AddListener(StartGame);
        _levelText.text = $"Уровень {Game.User.CurrentLevel + 1}";

        _closeAppButton.onClick.AddListener(OnButtonCloseClick);
    }

    private void StartGame()
    {
        Game.Locker.Lock(LockKey);
        Game.LevelLoader.StartGame(OnLoadingComplete);

        void OnLoadingComplete()
        {
            Close();
            Game.Locker.Unlock(LockKey);
        }
    }

    private void OnButtonCloseClick()
    {
        Application.Quit();
    }
}
