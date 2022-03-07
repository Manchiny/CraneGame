using System;
using UnityEngine;
using UnityEngine.UI;

public class ResetedProgressWindow : AbstractWindow
{
    [SerializeField] private Button _continueButton;

    OutOfUIClickChecker _outOfUIClickChecker;
    private IDisposable _clickObserver;

    public static ResetedProgressWindow Of() =>
                Game.Windows.ScreenChange<ResetedProgressWindow>(false, w => w.Init());

    private void Init()
    {
        _outOfUIClickChecker ??= gameObject.AddComponent<OutOfUIClickChecker>();
        _clickObserver = _outOfUIClickChecker.CheckMouseOutClick(_content, OnButtonClick);

        _continueButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        CloseAnimated();
    }
}
