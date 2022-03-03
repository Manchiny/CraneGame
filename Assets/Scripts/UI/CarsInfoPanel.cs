using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarsInfoPanel : MonoBehaviour
{
    [SerializeField] private CarSpawner _carSpawner;
    [Space]
    [SerializeField] private NeedColorIndicatorView _needColorViewPrefab;
    [SerializeField] private RectTransform _content;

    private List<GameObject> _indicators;

    private void Awake()
    {
        _carSpawner.OnNewCarCreate += OnNewCarCreated;
    }
    public void AddColorIndicator()
    {

    }

    private void SetIndicatorColor()
    {

    }

    private void OnCarSetColor()
    {

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
