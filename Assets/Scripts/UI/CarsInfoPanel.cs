using System.Collections.Generic;
using UnityEngine;

public class CarsInfoPanel : MonoBehaviour
{
    [SerializeField] private NeedColorIndicatorView _needColorViewPrefab;
    [SerializeField] private RectTransform _content;

    private CarSpawner _carSpawner;
    private List<GameObject> _indicators;

    public void Init()
    {
        if (_carSpawner != null)
            _carSpawner.OnNewCarCreate -= OnNewCarCreated;

        _carSpawner = FindObjectOfType<CarSpawner>();
        _carSpawner.OnNewCarCreate += OnNewCarCreated;
    }
    private void OnNewCarCreated(Car car)
    {
        var indicator = Instantiate(_needColorViewPrefab, _content);
        indicator.Init(car);
    }
    private void OnDestroy()
    {
        _carSpawner.OnNewCarCreate -= OnNewCarCreated;
    }
}
