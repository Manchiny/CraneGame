using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ColorManager;
using static LevelConfigs;

public class Ship : MonoBehaviour
{
    [SerializeField] private List<Transform> _containerPlacement;
    [SerializeField] private Transform _containerHolder;
    [SerializeField] private GameObject _containerPrefab;
    [SerializeField] private MeshFilter _shipBody;
    [SerializeField] private float _parkingMoveTime;

    private ShipConfig _config;
    private int _raws = 2;
    private const float CONTAINER_HEIGHT = 3f;
    public List<Container> Containers { get; private set; } = new List<Container>();
    public MeshFilter ShipBody => _shipBody;
    public Dictionary<ContainerColor, int> AvailibleCollors { get; private set; }
    public Action<Container> OnCrush;
    public Action OnContainersEnded;
    public void Init(ShipConfig config)
    {
        _config = config;
        _raws = _config.Raws;
        var configColors = GetConfigAvailibleColor(config.AvailibleColors);
        AvailibleCollors = new Dictionary<ContainerColor, int>();
        foreach (var item in configColors)
        {
            AvailibleCollors.Add(item.Key, 0);
        }

        InitContainers(configColors, config.StronglyCount());
    }

    private void InitContainers(Dictionary<ContainerColor, int> availableColors, int stronglyCount)
    {
        var newAvailableColors = availableColors;
        float offset = 0.1f;

        if (stronglyCount > 0)
        {
            int raws = stronglyCount / _containerPlacement.Count;
            raws += stronglyCount % _containerPlacement.Count == 0 ? 0 : 1;

            for (int i = 0; i < raws; i++)
            {
                foreach (var place in _containerPlacement)
                {
                    if (Containers.Count < stronglyCount)
                    {
                        CreateContainer(place, i);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < _raws; i++)
            {
                foreach (var place in _containerPlacement)
                {
                    CreateContainer(place, i);
                }
            }
        }

        foreach (var place in _containerPlacement)
        {
            place.gameObject.SetActive(false);
        }

        void CreateContainer(Transform place, int i)
        {
            Vector3 position = place.position;
            position.y = place.position.y + (CONTAINER_HEIGHT + offset) * (1 + i);
            var container = Instantiate(_containerPrefab, position, place.rotation, _containerHolder).GetComponent<Container>();

            var color = GetColorRandom();
            container.Init(this, color);
            Containers.Add(container);
            AddAvailibleColorsCount(color);
        }

        ContainerColor GetColorRandom()
        {
            List<ContainerColor> keyList = new List<ContainerColor>(newAvailableColors.Keys);
            int count = keyList.Count;
            ContainerColor randomColor = keyList[UnityEngine.Random.Range(0, count)];
            newAvailableColors[randomColor] -= 1;
            if (newAvailableColors[randomColor] <= 0)
                newAvailableColors.Remove(randomColor);

            return randomColor;
        }
    }

    private Dictionary<ContainerColor, int> GetConfigAvailibleColor(AvailableColor[] configColors)
    {
        var result = new Dictionary<ContainerColor, int>();

        foreach (var item in configColors)
        {
            result.Add(item.ContainerColor, item.MaxCount == 0 ? 200 : item.MaxCount);
        }

        return result;
    }

    public void RemoveContainer(Container container)
    {
        Containers.Remove(container);
        if (Containers.Count == 0)
            OnContainersEnded?.Invoke();
    }

    private void AddAvailibleColorsCount(ContainerColor color)
    {
        if (AvailibleCollors.TryGetValue(color, out int value))
        {
            value++;
            AvailibleCollors[color] = value;
        }
    }

    public void RemoveAvailibleColorsCount(ContainerColor color)
    {
        if (AvailibleCollors.TryGetValue(color, out int value))
        {
            value--;
            AvailibleCollors[color] = value;
        }
    }

    public void OnContainerCrush(Container container)
    {
        RemoveAvailibleColorsCount(container.ContainerColor);
        OnCrush?.Invoke(container);
    }

    public void MoveTo(Transform targetPoint)
    {
        var position = targetPoint.position;
        transform.DOMoveZ(targetPoint.position.z, _parkingMoveTime, false);
    }
}
