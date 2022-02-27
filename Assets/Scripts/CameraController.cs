using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _magnit;
    private float speed = 3;
    private void Start()
    {
        LookAtMagnit();
    }
    void LateUpdate()
    {
        RotateToTarget(_magnit.position);
    }

    private void LookAtMagnit()
    {
        transform.LookAt(_magnit.transform);
    }

    private void RotateToTarget(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speed * Time.deltaTime);
    }
}
