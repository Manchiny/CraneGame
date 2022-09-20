using DG.Tweening;
using RSG;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Magnit : MonoBehaviour
{
    private const float MagnetyzeAnimationTime = 0.1f;
    private const float ContainersWidth = 3f;
    private const float MagnetizeHeight = 0.35f;

    private const float MinRotationXToAddHeight = 45f;
    private const float MaxRotationXToAddHeight = 315f;

    private const float ForceLiftMagniteDistance = 0.4f;

    private Crane _crane;
    private Container _conteiner;

    private Sequence _moveSequence;
    private bool _inProcess;

    private Rigidbody _rigidbody;
    private Collider _collider;

    private HashSet<GameObject> _obstacles = new HashSet<GameObject>();

    public bool IsDownMoveFreeze { get; private set; }
    public bool HasObstacles => _obstacles.Count > 0;
    public bool CanMoveDown => IsDownMoveFreeze == false && HasObstacles == false;

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
        if(collision.gameObject.GetComponent<Checker>() == null)
            _obstacles.Add(collision.gameObject);

        if (_conteiner != null)
            _conteiner.Sound.PlayHitSound();
    }

    private void OnCollisionExit(Collision collision)
    {
        _obstacles.Remove(collision.gameObject);
    }

    public void Init(Crane cran)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _crane = cran;

        _obstacles.Clear();

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

            _conteiner.SetFree();

            _crane.Controller.StartForceMagnitUp(ForceLiftMagniteDistance, OnLifted);

            void OnLifted()
            {
                Unfreeze();
                _obstacles.Clear();
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

        _inProcess = true;
        IsDownMoveFreeze = true;

        _conteiner = container;
        _conteiner.OnMagniteze();

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

                container.transform.parent = transform;
                _rigidbody.isKinematic = false;
                _crane.Joint.autoConfigureConnectedAnchor = false;
                _crane.Joint.connectedAnchor = anchors;

                IsDownMoveFreeze = false;

                float y = _conteiner.transform.localRotation.eulerAngles.y;
                _inProcess = false;
                RotationChanged?.Invoke(true, y);
            });
    }

    private IPromise ContainerAttachAnimation(Container container)
    {
        Promise promise = new Promise();

        _moveSequence?.Kill();
        _moveSequence = DOTween.Sequence().SetLink(container.gameObject);

        var contTransform = container.transform;

        var additionalHeight = MagnetizeHeight;
        var contRotation = contTransform.rotation.eulerAngles;

        if (contRotation.x > MinRotationXToAddHeight && contRotation.x < MaxRotationXToAddHeight || contRotation.x < -MinRotationXToAddHeight && contRotation.x > -MaxRotationXToAddHeight)
            additionalHeight += ContainersWidth / 2;

        contRotation.x = 0;
        contRotation.z = 0;

        _moveSequence.Append(contTransform.DOMoveY(contTransform.position.y + additionalHeight, MagnetyzeAnimationTime));
        _moveSequence.Join(contTransform.DORotate(contRotation, MagnetyzeAnimationTime));

        _moveSequence.Play()
            .OnComplete(() => promise.Resolve());

        return promise;
    }
}

