using RSG;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class AbstractWindow : MonoBehaviour
{
    protected CanvasGroup canvasGroup;
    public Promise ClosePromise { get; } = new Promise();

    protected bool _isOpening = false;
    public bool IsOpening => _isOpening;

    protected bool _isClosing = false;
    public bool IsClosing => _isClosing;

	public virtual void Close()
	{
		if (_isClosing)
			return;

		OnClose();
		Destroy(gameObject);
		ClosePromise.Resolve();
	}


	protected void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		OnAwake();
	}

	protected virtual void Start()
	{
		OnStart();
		_isOpening = true;
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
