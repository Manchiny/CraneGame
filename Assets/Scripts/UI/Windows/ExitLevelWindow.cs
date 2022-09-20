using System;
using UnityEngine;
using UnityEngine.UI;

public class ExitLevelWindow : AbstractWindow
{
    [SerializeField] private Button _yes;
    [SerializeField] private Button _no;

    private const string LockKey = "ExitLevelWindow";

    OutOfUIClickChecker _outOfUIClickChecker;
    private IDisposable _clickObserver;

    public static ExitLevelWindow Show() =>
                   Game.Windows.ScreenChange<ExitLevelWindow>(false, w => w.Init());

    private void Init()
    {
        _outOfUIClickChecker ??= gameObject.AddComponent<OutOfUIClickChecker>();
        _clickObserver = _outOfUIClickChecker.CheckMouseOutClick(_content, OnButtonNoClick);

        _yes.onClick.AddListener(OnButtonYesClick);
        _no.onClick.AddListener(OnButtonNoClick);
    }

    protected override void OnClose()
    {
        base.OnClose();
        _clickObserver.Dispose();
        Destroy(_outOfUIClickChecker);
    }

    private void OnButtonYesClick()
    {
        Game.Locker.Lock(LockKey);
        CloseAnimated();
        ClosePromise
            .Then(() => Game.Level.ExitLevel(AfterExit));   
        
        void AfterExit()
        {
            Game.Locker.Unlock(LockKey);
        }
    }

    private void OnButtonNoClick()
    {
        RemoveButtonsListeners();
        CloseAnimated();
    }

    private void RemoveButtonsListeners()
    {
        _yes.onClick.RemoveAllListeners();
        _no.onClick.RemoveAllListeners();
    }
}
