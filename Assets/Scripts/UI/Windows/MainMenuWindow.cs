using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : AbstractWindow
{
    private const string LOCK_KEY = "MenuStartGame";

    [SerializeField] private Button _playButton;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Button _closeAppButton;
    public static MainMenuWindow Of() =>
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
        Game.Locker.Lock(LOCK_KEY);
        Game.LevelLoader.StartGame(OnLoadingComplete);

        void OnLoadingComplete()
        {
            Close();
            Game.Locker.Unlock(LOCK_KEY);
        }
    }

    private void OnButtonCloseClick()
    {
        Application.Quit();
    }
}
