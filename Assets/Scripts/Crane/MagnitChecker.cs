using UnityEngine;

public class MagnitChecker : MonoBehaviour
{
    public Container Container { get; private set; }

    private BoxCollider _collider;

    private void Awake()
    {
        Container = GetComponentInParent<Container>();
        _collider = GetComponent<BoxCollider>();
    }
    public void SetStatus(bool isFree)
    {
        if (_collider != null)
            _collider.enabled = isFree;
    }
}
