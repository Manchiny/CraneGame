using UnityEngine;
using DG.Tweening;

public class LoadingWindow : AbstractWindow
{
    [SerializeField] private Transform _loaderImage;

    private const float RotationDuration = 3f;

    public static LoadingWindow Show() =>
                    Game.Windows.ScreenChange<LoadingWindow>(true, w => w.Init());

    private void Init()
    {
        if (_loaderImage != null)
            _loaderImage.DOLocalRotate(new Vector3(0, 0, -360), RotationDuration, RotateMode.FastBeyond360)
                .SetRelative(true)
                .SetEase(Ease.Linear)
                .SetLink(gameObject)
                .SetLoops(-1, LoopType.Restart);
    }
}
