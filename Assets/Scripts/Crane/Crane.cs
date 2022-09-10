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

    private bool _isHeightFreeze;
    public bool IsDownMoveFreeze
    {
        get => _isHeightFreeze;
        set
        {
            _isHeightFreeze = value;

            if (value == true)
                DawnMoveFreezed?.Invoke();
        }
    }

    public event Action DawnMoveFreezed;
    
    public void Init()
    {
        _magnit.Init(this);
        _craneController.Init(this);
        IsDownMoveFreeze = false;
    }


}
