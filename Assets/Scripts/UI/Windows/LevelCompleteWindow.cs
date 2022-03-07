using RSG;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteWindow : AbstractWindow
{
    private const string LOCK_KEY = "LevelCompleteWindow";

    [SerializeField] private Button _continurButton;

    OutOfUIClickChecker _outOfUIClickChecker;
    private IDisposable _clickObserver;
    public static LevelCompleteWindow Of() =>
                   Game.Windows.ScreenChange<LevelCompleteWindow>(false, w => w.Init());

    private void Init()
    {
        _outOfUIClickChecker ??= gameObject.AddComponent<OutOfUIClickChecker>();
        _clickObserver = _outOfUIClickChecker.CheckMouseOutClick(_content, OnContinueClickButton);

        _continurButton.onClick.AddListener(OnContinueClickButton);
    }

    private void OnContinueClickButton()
    {
        _clickObserver.Dispose();

        Game.Locker.Lock(LOCK_KEY);

        CloseAnimated();
        ClosePromise
            .Then(() => Game.LevelManager.ExitLevel(AfterExit));

        void AfterExit()
        {
            Game.Locker.UnlockAll(LOCK_KEY);
        }
    }

    protected override void OnClose()
    {
        base.OnClose();
        _clickObserver.Dispose();
        Destroy(_outOfUIClickChecker);
    }
}
