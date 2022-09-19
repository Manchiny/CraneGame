using System;
using UnityEngine;

public class Crane : MonoBehaviour
{
    [SerializeField] private ConfigurableJoint _joint;
    [SerializeField] private Magnit _magnit;
    [SerializeField] private CableRender _cable;
    [SerializeField] private CraneController _craneController;

    public CraneController Controller => _craneController;
    public ConfigurableJoint Joint => _joint;
    public Magnit Magnit => _magnit;
    
    public void Init()
    {
        _magnit.Init(this);
        _craneController.Init(this);
    }
}
