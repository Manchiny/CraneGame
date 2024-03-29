using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmWindow : AbstractWindow
{
    [SerializeField] private Button _buttonYes;
    [SerializeField] private Button _buttonNo;

    private const string LockKey = "ExitLevelWindow";

    OutOfUIClickChecker _outOfUIClickChecker;
    private IDisposable _clickObserver;

    public static ConfirmWindow Show(Action confirmAction) =>
                 Game.Windows.ScreenChange<ConfirmWindow>(false, w => w.Init(confirmAction));

    private void Init(Action action)
    {
        _outOfUIClickChecker ??= gameObject.AddComponent<OutOfUIClickChecker>();
        _clickObserver = _outOfUIClickChecker.CheckMouseOutClick(_content, OnButtonNoClick);

        _buttonYes.onClick.AddListener(() => OnButtonYesClick(action));
        _buttonNo.onClick.AddListener(OnButtonNoClick);
    }

    private void OnButtonYesClick(Action action)
    {
        Game.Locker.Lock(LockKey);
        action.Invoke();
        CloseAnimated();
        ClosePromise
            .Then(() => Game.Locker.Unlock(LockKey));
    }

    private void OnButtonNoClick()
    {
        RemoveButtonsListeners();
        CloseAnimated();
    }

    private void RemoveButtonsListeners()
    {
        _buttonYes.onClick.RemoveAllListeners();
        _buttonNo.onClick.RemoveAllListeners();
    }
    protected override void OnClose()
    {
        base.OnClose();
        _clickObserver.Dispose();
        Destroy(_outOfUIClickChecker);
    }
}
