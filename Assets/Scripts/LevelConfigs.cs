using System;
using System.Collections.Generic;
using UnityEngine;
using static ColorManager;

[CreateAssetMenu]
public class LevelConfigs : ScriptableObject
{
    [Header("������")]
    [SerializeField] private LevelConfig[] _levelConfigs;
    public LevelConfig[] Configs => _levelConfigs;

    [Serializable]
    public class LevelConfig
    {
        [Header("���������� �����������, ������� ����� ��������")]
        [SerializeField] private int _maxContainersCrushes;
        [Header("������������ �������� � ������")]
        [SerializeField] private ShipConfig[] _shipConfig;
        public ShipConfig[] ShipConfigs => _shipConfig;
        public int MaxContainerCrushes => _maxContainersCrushes;
    }

    [Serializable]
    public class ShipConfig
    {
        [Range(1,5)]
        [Header("���������� ����� � ������ (���� ��� ����������� �� ���������� �����������)")]
        [SerializeField] private int _raws;
        [Header("����� ����������� �� �������, ��������� ���� �� 1. �� �����������!")]
        [SerializeField] private AvailableColor[] _availibleColors;
        [SerializeField] private Ship _shipPrefab;
        public int Raws => _raws;
        public IReadOnlyList<AvailableColor> AvailibleColors => _availibleColors;
        public Ship ShipPrefab => _shipPrefab;

        /// <summary>
        /// ���� ��� ��� ����� ������ ������ ������ ���������� - ������ ����������. ����� - 0;
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
        [Header("����������� �� ����������. ���� 0 - �� ��������")]
        [SerializeField] private int _maxCount; // ���� 0 - �� ��������. ���� ������ � ����� ����� ���������� - ����� ���  � �� �����
        public ContainerColor ContainerColor => _color;
        public int MaxCount => _maxCount;
    }

}


