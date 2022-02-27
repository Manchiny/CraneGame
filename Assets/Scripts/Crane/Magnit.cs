using System;
using System.Collections;
using UnityEngine;

public class Magnit : MonoBehaviour
{
    public Crane crane;
    private Container _conteiner; //{ get; private set; }
    private MagnitChecker checker;

    public Action OnMagnityze;
    public Action OnFree;
    public void Free()
    {
        if (_conteiner != null)
        {
            OnFree?.Invoke();
            InfoScreenView.Instance.OnCargoAngelSet(false);

            checker = _conteiner.GetComponentInChildren<MagnitChecker>();
            checker.SetStatus(false);

            _conteiner.transform.parent = null;
            var rigidbody = _conteiner.gameObject.AddComponent<Rigidbody>();
            rigidbody.mass = 500;
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            _conteiner.SetMagnitizeStatus(false);

            StartCoroutine(UnfreezeChecker());
        }
    }

    IEnumerator UnfreezeChecker()
    {
        yield return new WaitForSeconds(2f);
        checker.SetStatus(true);
        _conteiner = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_conteiner == null)
        {
            if (other.gameObject.GetComponent<MagnitChecker>() == true)
            {
                var container = other.gameObject.GetComponent<MagnitChecker>().Container;
                MagnitChecker checker = other.gameObject.GetComponent<MagnitChecker>();
                Magnetize(container.gameObject, checker);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        crane.IsDownMoveFreeze = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        crane.IsDownMoveFreeze = false;
    }

    private void Magnetize(GameObject container, MagnitChecker checker)
    {
        OnMagnityze?.Invoke();

        crane.IsDownMoveFreeze = true;
        _conteiner = container.GetComponent<Container>();
        _conteiner.SetMagnitizeStatus(true);

        var contRigidbody = container.GetComponent<Rigidbody>();
        if (contRigidbody != null)
        {
            Destroy(contRigidbody);
        }

        crane.Joint.connectedBody = null;
        var magnitRiggidbody = GetComponent<Rigidbody>();
        magnitRiggidbody.isKinematic = true;

        container.transform.position += new Vector3(0, 0.25f, 0);
        var contRotation = container.transform.rotation;
        contRotation.x = 0;
        container.transform.rotation = contRotation; // выравниваем вращение контейнера по горизонту

        transform.rotation = Quaternion.identity;
        transform.position = container.transform.position;

        crane.Joint.autoConfigureConnectedAnchor = true;
        crane.Joint.connectedBody = magnitRiggidbody;

        var anchors = crane.Joint.connectedAnchor;
        anchors.x = 0;
        anchors.z = 0;

        checker.gameObject.GetComponent<MagnitChecker>().SetStatus(false);

        container.transform.parent = this.transform;
        magnitRiggidbody.isKinematic = false;
        crane.Joint.autoConfigureConnectedAnchor = false;
        crane.Joint.connectedAnchor = anchors;

        crane.IsDownMoveFreeze = false;

        float y = _conteiner.transform.localRotation.eulerAngles.y;
        InfoScreenView.Instance.OnCargoAngelSet(angel: y);
    }

    public void Rotate(float speed)
    {
        if (_conteiner == null)
            return;

        _conteiner.transform.Rotate(Vector3.up * speed * Time.deltaTime);
        float y = _conteiner.transform.localRotation.eulerAngles.y;
        InfoScreenView.Instance.OnCargoAngelSet(angel: y);
    }
}

