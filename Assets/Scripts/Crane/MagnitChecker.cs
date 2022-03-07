using UnityEngine;

public class MagnitChecker : MonoBehaviour
{
    public Container Container { get; private set; }

    private void Awake()
    {
        Container = GetComponentInParent<Container>();
    }
   
}
