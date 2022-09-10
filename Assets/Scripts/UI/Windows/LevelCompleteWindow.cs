using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteWindow : AbstractWindow
{
    [SerializeField] private Button _continurButton;

    private const string LockKey = "LevelCompleteWindow";

    OutOfUIClickChecker _outOfUIClickChecker;
    private IDisposable _clickObserver;

    public static LevelCompleteWindow Show() =>
                   Game.Windows.ScreenChange<LevelCompleteWindow>(false, w => w.Init());

    private void Init()
    {
        _outOfUIClickChecker ??= gameObject.AddComponent<OutOfUIClickChecker>();
        _clickObserver = _outOfUIClickChecker.CheckMouseOutClick(_content, OnContinueClickButton);

        _continurButton.onClick.AddListener(OnContinueClickButton);
    }

    protected override void OnClose()
    {
        base.OnClose();
        _clickObserver.Dispose();
        Destroy(_outOfUIClickChecker);
    }

    private void OnContinueClickButton()
    {
        _clickObserver.Dispose();

        Game.Locker.Lock(LockKey);

        CloseAnimated();
        ClosePromise
            .Then(() => Game.LevelManager.ExitLevel(AfterExit));

        void AfterExit()
        {
            Game.Locker.UnlockAll(LockKey);
        }
    }
}
