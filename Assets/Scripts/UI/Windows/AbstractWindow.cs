using DG.Tweening;
using RSG;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class AbstractWindow : MonoBehaviour
{
    private const float UnfideDuration = 2f;
    private const float HideDuration = 1f;

    private const float CloseAnimationMoveDownDuration = 0.1f;
    private const float CloseAnimationMoveUpDuration = 0.3f;
    private const float CloseAnimationMoveDownDistance = 100f;

    protected CanvasGroup _canvasGroup;
    protected RectTransform _content;

    protected bool _isOpening = false;
    protected bool _isClosing = false;

    protected Sequence _animationSequence;

    public Promise ClosePromise { get; } = new Promise();
    public bool IsOpening => _isOpening;
    public bool IsClosing => _isClosing;


    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _content = transform.Find("Content").transform as RectTransform;
        OnAwake();
    }

    public virtual void Close()
    {
        if (_isClosing)
            return;

        OnClose();
        Destroy(gameObject);
        ClosePromise.Resolve();
    }

    protected virtual void Start()
    {
        OnStart();
        _isOpening = true;
    }

    protected void Unhide()
    {
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        _canvasGroup.DOFade(1, UnfideDuration).SetLink(gameObject);
    }

    protected void Hide()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        _canvasGroup.DOFade(0, HideDuration).SetLink(gameObject);
    }

    protected void ForceHide()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0;
    }

    protected void CloseAnimated()
    {
        if (_isClosing)
            return;

        RectTransform _rectTransform = _content;

        _animationSequence?.Complete();

        _animationSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(_rectTransform.DOMoveY(_rectTransform.position.y - CloseAnimationMoveDownDistance, CloseAnimationMoveDownDuration))
            .Append(_rectTransform.DOMoveY(Screen.safeArea.height * 2f, CloseAnimationMoveUpDuration));

        _animationSequence.SetEase(Ease.Linear);

        _animationSequence.Play()
            .OnComplete(() =>
            {
                OnClose();
                Destroy(gameObject);
                ClosePromise.Resolve();
            });
    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnClose() { }

}

