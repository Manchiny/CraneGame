using DG.Tweening;
using System;
using UniRx;
using UnityEngine;

public class LoadingWindow : AbstractWindow
{
    private const float ROTATION_SPEED = 50f;

    [SerializeField] private Transform _loaderImage;
    private CompositeDisposable _disposeRotate = new CompositeDisposable();
    public static LoadingWindow Of() =>
                    Game.Windows.ScreenChange<LoadingWindow>(true, w => w.Init());

    private void Init()
    {
        Observable.EveryUpdate().Subscribe(_ =>
        {
            RotateLoader();
        }).AddTo(_disposeRotate);
    }


    private void RotateLoader()
    {
        var vector = new Vector3(0, 0, -1);
        _loaderImage.Rotate(vector * Time.deltaTime * ROTATION_SPEED);
    }

    protected override void OnClose() 
    {
        _disposeRotate.Clear();
    }
}
