using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CableRender : MonoBehaviour
{
    [SerializeField] Transform _target;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        UpdateRender();    
    }

    private void UpdateRender()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _target.position);
    }

    public void SetTarget(Transform targetTransform)
    {
        _target = targetTransform;
    }
}
