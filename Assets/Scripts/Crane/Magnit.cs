using DG.Tweening;
using RSG;
using System;
using System.Collections;
using UniRx;
using UnityEngine;

public class Magnit : MonoBehaviour
{
    private const float MAGNETYZE_ANIMATION_TIME = 0.1f;
    private const float CONTAINERS_WIDTH = 3f;
    private const float ADDITIONAl_HEIGHT_ON_MAGNETYZE = 0.35f;

    private Crane _crane;
    private Container _conteiner;

    private Sequence _sequence;
    public bool IsFreezed { get; private set; }
    private bool _inProcess;
    public Action<bool, float> OnRotationSet;
    public void Init(Crane cran)
    {
        _crane = cran;
        Free();
        _conteiner = null;
        IsFreezed = false;
        _inProcess = false;
    }
    public void Free()
    {
        if (_conteiner != null && _inProcess == false)
        {
            IsFreezed = true;
            _sequence?.Kill();
            _inProcess = true;

            OnRotationSet?.Invoke(false, 0);

            _conteiner.transform.parent = null;
            _conteiner.AddRigidbody();
            _conteiner.SetMagnitizeStatus(false);

            _crane.Controller.StartForceMagnitUp(0.4f, OnComplete);

            void OnComplete()
            {
                Unfreeze();
            }
        }
    }

    private void Unfreeze()
    {
        IsFreezed = false;
        _conteiner = null;
        _inProcess = false;
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
        _crane.IsDownMoveFreeze = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        _crane.IsDownMoveFreeze = false;
    }

    private void Magnetize(GameObject container, MagnitChecker checker)
    {
        if (IsFreezed)
            return;

        _inProcess = false;

        _crane.IsDownMoveFreeze = true;
        _conteiner = container.GetComponent<Container>();
        _conteiner.SetMagnitizeStatus(true);

        var contRigidbody = container.GetComponent<Rigidbody>();
        if (contRigidbody != null)
        {
            Destroy(contRigidbody);
        }

        _crane.Joint.connectedBody = null;
        var magnitRiggidbody = GetComponent<Rigidbody>();
        magnitRiggidbody.isKinematic = true;
        var collider = GetComponent<Collider>();
        transform.rotation = Quaternion.identity;
        collider.isTrigger = true;
        ContainerAttachAnim(_conteiner)
            .Then(() =>
            {
                collider.isTrigger = false;
                transform.position = container.transform.position;

                _crane.Joint.autoConfigureConnectedAnchor = true;
                _crane.Joint.connectedBody = magnitRiggidbody;

                var anchors = _crane.Joint.connectedAnchor;
                anchors.x = 0;
                anchors.z = 0;

                container.transform.parent = this.transform;
                magnitRiggidbody.isKinematic = false;
                _crane.Joint.autoConfigureConnectedAnchor = false;
                _crane.Joint.connectedAnchor = anchors;

                _crane.IsDownMoveFreeze = false;

                float y = _conteiner.transform.localRotation.eulerAngles.y;
                OnRotationSet?.Invoke(true, y);
            }
            );
    }

    public void Rotate(float speed)
    {
        if (_conteiner == null)
            return;

        _conteiner.transform.Rotate(Vector3.up * speed * Time.deltaTime);
        float y = _conteiner.transform.localRotation.eulerAngles.y;
        OnRotationSet?.Invoke(true, y);
    }

    private IPromise ContainerAttachAnim(Container container)
    {
        Promise promise = new Promise();

        _sequence?.Kill();
        _sequence.SetLink(container.gameObject);
        _sequence = DOTween.Sequence();
        var contTransform = container.transform;

        var addHeight = ADDITIONAl_HEIGHT_ON_MAGNETYZE;
        var contRotation = contTransform.rotation.eulerAngles;
        if (contRotation.x > 45 && contRotation.x < 315 || contRotation.x < -45 && contRotation.x > -225)
            addHeight += CONTAINERS_WIDTH / 2;

        contRotation.x = 0;
        contRotation.z = 0;

        _sequence.Append(contTransform.DOMoveY(contTransform.position.y + addHeight, MAGNETYZE_ANIMATION_TIME));
        _sequence.Join(contTransform.DORotate(contRotation, MAGNETYZE_ANIMATION_TIME));

        _sequence.Play()
            .OnComplete(() => promise.Resolve());

        return promise;
    }
}

