using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutOfUIClickChecker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool _isOverPanel = false;
    public bool IsOverPanel => _isOverPanel;
    public IDisposable CheckMouseOutClick(RectTransform objectTransform, Action onOutClick)
    {
        return Observable.EveryUpdate().Subscribe(_ =>
        {
            if (!Input.GetMouseButtonUp(0) || !objectTransform || _isOverPanel) return;
            onOutClick?.Invoke();
        }).AddTo(objectTransform);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        _isOverPanel = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        _isOverPanel = false;
    }
}
