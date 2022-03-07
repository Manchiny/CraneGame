using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelFailedWindow : AbstractWindow
{
    private const string LOCK_KEY = "LevelFailedWindow";

    [SerializeField] private Button _continurButton;

    public static LevelFailedWindow Of() =>
                   Game.Windows.ScreenChange<LevelFailedWindow>(false, w => w.Init());

    private void Init()
    {
        _continurButton.onClick.AddListener(OnContinueClickButton);
    }

    private void OnContinueClickButton()
    {
        Game.Locker.Lock(LOCK_KEY);
        _continurButton.onClick.RemoveAllListeners();

        CloseAnimated();
        ClosePromise
            .Then(() => Game.LevelManager.ExitLevel(AfterExit));

        void AfterExit()
        {
            Game.Locker.UnlockAll(LOCK_KEY);
        }
    }
}
