using DG.Tweening;
using RSG;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ColorDataBase;
using static LevelConfigs;

[RequireComponent(typeof(AudioSource))]
public class Ship : MonoBehaviour
{
    [SerializeField] private List<Transform> _containerPlacement;
    [SerializeField] private Transform _containerHolder;
    [SerializeField] private MeshFilter _shipBody;
    [SerializeField] private float _parkingMoveTime;
    [Space]
    [SerializeField] private AnimationCurve _moveSpeedCurve;

    private const int MaxOneColorContainersDefault = 200;
    private int _raws = 2;

    private Container _containerPrefab;
    private ShipConfig _config;

    private float _containersHeight;

    private List<Container> _containers;
    private Dictionary<ContainerColor, int> _availibleCollors;

    public AudioSource AudioSource { get; private set; }

    public MeshFilter ShipBody => _shipBody;
    public int ContainersCount => _containers == null ? 0 : _containers.Count;

    public event Action<Container> ContainerCrushed;
    public event Action ContainersEnded;

    private void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }


    private void OnDestroy()
    {
        Game.LevelLoader.LevelExited -= OnExitLevel;
    }

    public void Init(ShipConfig config, Container containerPrefab)
    {
        _containerPrefab = containerPrefab;
        _containers = new List<Container>();

        Game.LevelLoader.LevelExited += OnExitLevel;

        _containersHeight = _containerPrefab.ContainerHeight;

        _config = config;
        _raws = _config.Raws;

        var configColors = GetConfigAvailibleColor(config.AvailibleColors);

        _availibleCollors = new Dictionary<ContainerColor, int>();

        foreach (var color in configColors)
            _availibleCollors.Add(color.Key, 0);

        InitContainers(configColors, config.StronglyCount());
    }

    public void RemoveContainer(Container container)
    {
        _containers.Remove(container);

        if (_containers.Count == 0)
            ContainersEnded?.Invoke();
    }

    public void RemoveAvailibleColorsCount(ContainerColor color)
    {
        if (_availibleCollors.TryGetValue(color, out int value))
        {
            value--;
            _availibleCollors[color] = value;
        }
    }

    public void OnContainerCrush(Container container)
    {
        RemoveAvailibleColorsCount(container.ContainerColor);
        ContainerCrushed?.Invoke(container);
    }

    public IPromise MoveTo(Transform targetPoint)
    {
        Promise promise = new Promise();

        transform.DOMoveZ(targetPoint.position.z, _parkingMoveTime, false)
            .SetEase(Ease.InOutQuad)
            .SetLink(gameObject)
            .OnComplete(() => promise.Resolve());

        return promise;
    }

    public ContainerColor GetRandomAvailibleContainerColor()
    {
        List<ContainerColor> availibleCollors = new List<ContainerColor>();

        foreach (var color in _availibleCollors)
        {
            if (color.Value > 0)
                availibleCollors.Add(color.Key);
        }

        int count = availibleCollors.Count;
        int random = UnityEngine.Random.Range(0, count);

        ContainerColor randomColor = availibleCollors[random];
        RemoveAvailibleColorsCount(randomColor);

        return randomColor;
    }

    public bool HasAvailibleColor(ContainerColor color) => _availibleCollors[color] >= 0;

    public bool HasAvailibleColor()
    {
        foreach (var color in _availibleCollors)
        {
            if (color.Value > 0)
                return true;
        }

        return false;
    }

    private void InitContainers(Dictionary<ContainerColor, int> availableColors, int stronglyCount)
    {
        var newAvailableColors = availableColors;
        float heightOffset = 0.1f;

        if (stronglyCount > 0)
        {
            int raws = stronglyCount / _containerPlacement.Count;
            raws += stronglyCount % _containerPlacement.Count == 0 ? 0 : 1;

            for (int i = 0; i < raws; i++)
            {
                foreach (var place in _containerPlacement)
                {
                    if (_containers.Count < stronglyCount)
                        CreateContainer(place, i + 1);
                }
            }
        }
        else
        {
            for (int i = 0; i < _raws; i++)
            {
                foreach (var place in _containerPlacement)
                    CreateContainer(place, i + 1);
            }
        }

        foreach (var place in _containerPlacement)
            place.gameObject.SetActive(false);

        void CreateContainer(Transform place, int rawNumber)
        {
            Vector3 position = place.position;
            position.y = place.position.y + (_containersHeight + heightOffset) * (rawNumber);

            var container = Instantiate(_containerPrefab, position, place.rotation, _containerHolder);

            var color = GetColorRandom();
            container.Init(this, color);
            _containers.Add(container);
            AddAvailibleColorsCount(color);
        }

        ContainerColor GetColorRandom()
        {
            List<ContainerColor> keyList = new List<ContainerColor>(newAvailableColors.Keys);
            int count = keyList.Count;

            ContainerColor randomColor = keyList[UnityEngine.Random.Range(0, count)];
            newAvailableColors[randomColor]--;

            if (newAvailableColors[randomColor] <= 0)
                newAvailableColors.Remove(randomColor);

            return randomColor;
        }
    }

    private Dictionary<ContainerColor, int> GetConfigAvailibleColor(IReadOnlyList<AvailableColor> configColors)
    {
        var result = new Dictionary<ContainerColor, int>();

        foreach (var color in configColors)
            result.Add(color.ContainerColor, color.MaxCount == 0 ? MaxOneColorContainersDefault : color.MaxCount);

        return result;
    }

    private void AddAvailibleColorsCount(ContainerColor color)
    {
        if (_availibleCollors.TryGetValue(color, out int value))
        {
            value++;
            _availibleCollors[color] = value;
        }
    }

    private void OnExitLevel()
    {
        Destroy(gameObject);
    }
}
