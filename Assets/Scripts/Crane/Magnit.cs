using DG.Tweening;
using RSG;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Magnit : MonoBehaviour
{
    private const float MagnetyzeAnimationTime = 0.1f;
    private const float ContainersWidth = 3f;
    private const float MagnetizeHeight = 0.35f;

    private Crane _crane;
    private Container _conteiner;

    private Sequence _moveSequence;
    private bool _inProcess;

    private Rigidbody _rigidbody;
    private Collider _collider;

    public bool IsFreezed { get; private set; }

    public event Action<bool, float> RotationChanged;

    private void OnTriggerEnter(Collider other)
    {
        if (_conteiner == null)
        {
            if (other.gameObject.TryGetComponent(out MagnitChecker checker))
            {
                Magnetize(checker.Container);
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

    public void Init(Crane cran)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
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
            _moveSequence?.Kill();
            _inProcess = true;

            RotationChanged?.Invoke(false, 0);

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

    public void Rotate(float speed)
    {
        if (_conteiner == null)
            return;

        _conteiner.transform.Rotate(Vector3.up * speed * Time.deltaTime);
        float y = _conteiner.transform.localRotation.eulerAngles.y;
        RotationChanged?.Invoke(true, y);
    }

    private void Unfreeze()
    {
        IsFreezed = false;
        _conteiner = null;
        _inProcess = false;
    }

    private void Magnetize(Container container)
    {
        if (IsFreezed)
            return;

        _inProcess = false;

        _crane.IsDownMoveFreeze = true;
        _conteiner = container;
        _conteiner.SetMagnitizeStatus(true);

        var contRigidbody = container.GetComponent<Rigidbody>();

        if (contRigidbody != null)
            Destroy(contRigidbody);

        _crane.Joint.connectedBody = null;

        _rigidbody.isKinematic = true;
        transform.rotation = Quaternion.identity;
        _collider.isTrigger = true;

        ContainerAttachAnimation(_conteiner)
            .Then(() =>
            {
                _collider.isTrigger = false;
                transform.position = container.transform.position;

                _crane.Joint.autoConfigureConnectedAnchor = true;
                _crane.Joint.connectedBody = _rigidbody;

                var anchors = _crane.Joint.connectedAnchor;
                anchors.x = 0;
                anchors.z = 0;

                container.transform.parent = this.transform;
                _rigidbody.isKinematic = false;
                _crane.Joint.autoConfigureConnectedAnchor = false;
                _crane.Joint.connectedAnchor = anchors;

                _crane.IsDownMoveFreeze = false;

                float y = _conteiner.transform.localRotation.eulerAngles.y;
                RotationChanged?.Invoke(true, y);
            });
    }

    private IPromise ContainerAttachAnimation(Container container)
    {
        Promise promise = new Promise();

        _moveSequence?.Kill();
        _moveSequence = DOTween.Sequence().SetLink(container.gameObject);

        var contTransform = container.transform;

        var addHeight = MagnetizeHeight;
        var contRotation = contTransform.rotation.eulerAngles;

        if (contRotation.x > 45 && contRotation.x < 315 || contRotation.x < -45 && contRotation.x > -225)
            addHeight += ContainersWidth / 2;

        contRotation.x = 0;
        contRotation.z = 0;

        _moveSequence.Append(contTransform.DOMoveY(contTransform.position.y + addHeight, MagnetyzeAnimationTime));
        _moveSequence.Join(contTransform.DORotate(contRotation, MagnetyzeAnimationTime));

        _moveSequence.Play()
            .OnComplete(() => promise.Resolve());

        return promise;
    }
}

