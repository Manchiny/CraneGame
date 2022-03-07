using UnityEngine;

public class ExcessContainerChecker : MonoBehaviour
{
    private Container _container;
    private bool _isInited;
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void Init(Container container)
    {
        gameObject.SetActive(true);
        _container = container;
        _isInited = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isInited)
            return;

        if (other.GetComponent<Container>() == true)
        {
            var excessContainer = other.GetComponent<Container>();
            _container.AddContainerAsWrong(excessContainer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isInited)
            return;

        if (other.GetComponent<Container>() == true)
        {
            var excessContainer = other.GetComponent<Container>();
            _container.RemoveContainerAsWrong(excessContainer);
        }

    }
}
