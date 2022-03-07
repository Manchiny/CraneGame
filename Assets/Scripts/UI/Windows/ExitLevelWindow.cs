using System;
using UnityEngine;
using UnityEngine.UI;

public class ExitLevelWindow : AbstractWindow
{
    private const string LOCK_KEY = "ExitLevelWindow";

    [SerializeField] private Button _yes;
    [SerializeField] private Button _no;

    OutOfUIClickChecker _outOfUIClickChecker;
    private IDisposable _clickObserver;
    public static ExitLevelWindow Of() =>
                   Game.Windows.ScreenChange<ExitLevelWindow>(false, w => w.Init());

    private void Init()
    {
        _outOfUIClickChecker ??= gameObject.AddComponent<OutOfUIClickChecker>();
        _clickObserver = _outOfUIClickChecker.CheckMouseOutClick(_content, OnButtonNoClick);

        _yes.onClick.AddListener(OnButtonYesClick);
        _no.onClick.AddListener(OnButtonNoClick);
    }

    private void OnButtonYesClick()
    {
        Game.Locker.Lock(LOCK_KEY);
        CloseAnimated();
        ClosePromise
            .Then(() => Game.LevelManager.ExitLevel(AfterExit));   
        
        void AfterExit()
        {
            Game.Locker.Unlock(LOCK_KEY);
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
    protected override void OnClose()
    {
        base.OnClose();
        _clickObserver.Dispose();
        Destroy(_outOfUIClickChecker);
    }
}
