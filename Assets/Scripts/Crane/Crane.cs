using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crane : MonoBehaviour
{
    [SerializeField] private ConfigurableJoint _joint;
    [SerializeField] private Magnit _magnit;
    [SerializeField] private CableRender _cable;

    public bool IsDownMoveFreeze = false;

    private void Awake()
    {
        _magnit.crane = this;
    }

    public ConfigurableJoint Joint { get => _joint; }
    public Magnit Magnit { get => _magnit; }

}
