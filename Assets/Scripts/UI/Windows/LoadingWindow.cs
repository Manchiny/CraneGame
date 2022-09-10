using UniRx;
using UnityEngine;

public class LoadingWindow : AbstractWindow
{
    [SerializeField] private Transform _loaderImage;

    private const float RotationSpeed = 50f;

    private CompositeDisposable _disposeRotate = new CompositeDisposable();

    public static LoadingWindow Show() =>
                    Game.Windows.ScreenChange<LoadingWindow>(true, w => w.Init());

    private void Init()
    {
        Observable.EveryUpdate().Subscribe(_ =>
            {
                RotateLoader();
            }).AddTo(_disposeRotate);
    }

    protected override void OnClose()
    {
        _disposeRotate.Clear();
    }

    private void RotateLoader()
    {
        var vector = new Vector3(0, 0, -1);
        _loaderImage.Rotate(vector * Time.deltaTime * RotationSpeed);
    }


}
