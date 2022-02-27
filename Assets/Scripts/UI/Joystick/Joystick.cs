using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] protected Image _joystickBg;
    [SerializeField] protected Image _joystick;
    [SerializeField] protected CraneController _controller;
    protected Vector2 inputVector;
  
    private CompositeDisposable _dispose = new CompositeDisposable();

    protected void Start()
    {
        _joystick.transform.localPosition = Vector3.zero;
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
 
        //Observable.EveryUpdate().Subscribe(_ =>
        //{
        //    CameraLookAtMagnit();
        //}).AddTo(_dispose);

        OnDrag(eventData);
    }
    public abstract void OnDrag(PointerEventData eventData);
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        _joystick.rectTransform.anchoredPosition = Vector2.zero;
        _dispose.Clear();
    }

    public float Horizontal()
    {
        if (inputVector.x != 0)
            return inputVector.x;
        else
            return Input.GetAxisRaw("Horizontal");
    }
    public float Vertical()
    {
        if (inputVector.y != 0)
            return inputVector.y;
        else
            return Input.GetAxisRaw("Vertical");
    }
}

