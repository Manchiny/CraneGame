using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class OptionsPanel : MonoBehaviour
{
    private const float FadeDuration = 0.35f;

    private RectTransform _rectTransform;
    private Sequence _sequence;
    private Vector2 _defaultSize;
    private CanvasGroup _canvasGroup;

    private IDisposable _clickObserver;
    private OutOfUIClickChecker _outOfUIClickChecker;

    private bool _isActive;
    public bool IsActive => _isActive;

    private void Awake()
    {
        _rectTransform = (RectTransform)transform;
        _defaultSize = _rectTransform.sizeDelta;
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide(true);
    }

    public void Show()
    {
        _isActive = true;

        _outOfUIClickChecker ??= gameObject.AddComponent<OutOfUIClickChecker>();
        _clickObserver = _outOfUIClickChecker.CheckMouseOutClick((RectTransform)transform, () => Hide()); 

        _sequence?.Complete();
        _sequence = DOTween.Sequence()
            .Append(_rectTransform.DOSizeDelta(_defaultSize, FadeDuration))
            .Join(_canvasGroup.DOFade(1, FadeDuration))
            .SetLink(gameObject);
    }

    public void Hide(bool force = false)
    {
        _clickObserver?.Dispose();
        _isActive = false;
        _outOfUIClickChecker = null;

        var sizeVector = _defaultSize;
        sizeVector.x = 0;

        _sequence?.Complete();
        _sequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(_rectTransform.DOSizeDelta(sizeVector, force ? 0 : FadeDuration))
            .Join(_canvasGroup.DOFade(0, FadeDuration));

    }
}

