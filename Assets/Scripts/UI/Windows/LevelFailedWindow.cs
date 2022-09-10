using UnityEngine;
using UnityEngine.UI;

public class LevelFailedWindow : AbstractWindow
{
    [SerializeField] private Button _continurButton;

    private const string LockKey = "LevelFailedWindow";

    public static LevelFailedWindow Show() =>
                   Game.Windows.ScreenChange<LevelFailedWindow>(false, w => w.Init());

    private void Init()
    {
        _continurButton.onClick.AddListener(OnContinueClickButton);
    }

    private void OnContinueClickButton()
    {
        Game.Locker.Lock(LockKey);
        _continurButton.onClick.RemoveAllListeners();

        CloseAnimated();
        ClosePromise
            .Then(() => Game.LevelManager.ExitLevel(AfterExit));

        void AfterExit()
        {
            Game.Locker.UnlockAll(LockKey);
        }
    }
}
