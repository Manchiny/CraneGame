using System;
using System.Collections.Generic;
using UnityEngine;
using static ColorManager;

[CreateAssetMenu]
public class LevelConfigs : ScriptableObject
{
    [Header("Уровни")]
    [SerializeField] private LevelConfig[] _levelConfigs;
    public LevelConfig[] Configs => _levelConfigs;

    [Serializable]
    public class LevelConfig
    {
        [Header("Количество контейнеров, которые можно утратить")]
        [SerializeField] private int _maxContainersCrushes;
        [Header("Конфигурации кораблей в уровне")]
        [SerializeField] private ShipConfig[] _shipConfig;
        public ShipConfig[] ShipConfigs => _shipConfig;
        public int MaxContainerCrushes => _maxContainersCrushes;
    }

    [Serializable]
    public class ShipConfig
    {
        [Range(1,5)]
        [Header("Количество рядов в высоту (если нет ограничений по количеству контейнеров)")]
        [SerializeField] private int _raws;
        [Header("Цвета контейнеров на корабле, необходим хотя бы 1. НЕ ДУБЛИРОВАТЬ!")]
        [SerializeField] private AvailableColor[] _availibleColors;
        [SerializeField] private Ship _shipPrefab;
        public int Raws => _raws;
        public IReadOnlyList<AvailableColor> AvailibleColors => _availibleColors;
        public Ship ShipPrefab => _shipPrefab;

        /// <summary>
        /// Если все для ввсех цветов строго задано количество - вернет количество. Иначе - 0;
        /// </summary>
        /// <returns></returns>
        public int StronglyCount()
        {
            int stronglyCount = 0;
            foreach (var item in _availibleColors)
            {
                stronglyCount += item.MaxCount;
                if (item.MaxCount == 0)
                {
                    return 0;
                }
            }

            return stronglyCount;
        }
    }

    [Serializable]
    public class AvailableColor
    {

        [SerializeField] private ContainerColor _color;
        [Header("Ограничение по количеству. Если 0 - то безлимит")]
        [SerializeField] private int _maxCount; // если 0 - то безлимит. Если запись о цвете вооще отсутсвует - тогда его  и не будет
        public ContainerColor ContainerColor => _color;
        public int MaxCount => _maxCount;
    }

}


