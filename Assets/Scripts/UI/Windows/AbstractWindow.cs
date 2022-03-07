using DG.Tweening;
using RSG;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class AbstractWindow : MonoBehaviour
{
    private const float FADE_DURATION = 2f;
    protected CanvasGroup _canvasGroup;
    protected RectTransform _content;
    public Promise ClosePromise { get; } = new Promise();

    protected bool _isOpening = false;
    public bool IsOpening => _isOpening;

    protected bool _isClosing = false;
    public bool IsClosing => _isClosing;

    protected Sequence _sequence;
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

        _canvasGroup.DOFade(1, FADE_DURATION).SetLink(gameObject);
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

        RectTransform _rectTransform = (RectTransform)_content.transform;

        _sequence?.Complete();

        _sequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(_rectTransform.DOMoveY(_rectTransform.position.y - 100f, 0.1f))
            .Append(_rectTransform.DOMoveY(_rectTransform.position.y + 800f, 0.3f));

        _sequence.SetEase(Ease.Linear);

        _sequence.Play()
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
public enum WindowState
{
    Open,
    Process,
    Close
}
