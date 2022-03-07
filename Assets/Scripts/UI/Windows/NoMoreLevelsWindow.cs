using System;
using UnityEngine;
using UnityEngine.UI;

public class NoMoreLevelsWindow : AbstractWindow
{
    [SerializeField] private Button _buttonReset;
    [SerializeField] private Button _buttonCancel;

    OutOfUIClickChecker _outOfUIClickChecker;
    private IDisposable _clickObserver;
    public static NoMoreLevelsWindow Of() =>
                   Game.Windows.ScreenChange<NoMoreLevelsWindow>(false, w => w.Init());

    private void Init()
    {
        _outOfUIClickChecker ??= gameObject.AddComponent<OutOfUIClickChecker>();
        _clickObserver = _outOfUIClickChecker.CheckMouseOutClick(_content, OnButtonCancelClick);

        _buttonReset.onClick.AddListener(OnButtonYesClick);
        _buttonCancel.onClick.AddListener(OnButtonCancelClick);
    }

    private void OnButtonYesClick()
    {
        CloseAnimated();
        ClosePromise
            .Then(() => ConfirmWindow.Of(() => Game.User.ResetLevel()));
    }

    private void OnButtonCancelClick()
    {
        RemoveButtonsListeners();
        CloseAnimated();
    }

    private void RemoveButtonsListeners()
    {
        _buttonReset.onClick.RemoveAllListeners();
        _buttonCancel.onClick.RemoveAllListeners();
    }
    protected override void OnClose()
    {
        base.OnClose();
        _clickObserver.Dispose();
        Destroy(_outOfUIClickChecker);
    }
}
