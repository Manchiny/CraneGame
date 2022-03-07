using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Locker : MonoBehaviour
{
    private readonly List<string> _lockedByKey = new List<string>();

    public BoolReactiveProperty IsLocked = new BoolReactiveProperty(false);

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void Lock(string key)
    {
        _lockedByKey.Add(key);
        UpdateActive();
        Debug.Log("Lock: " + key);
    }

    public void Unlock(string key)
    {
        _lockedByKey.Remove(key);
        UpdateActive();
        Debug.Log("Unlock: " + key);
    }

    private void UpdateActive()
    {
        var newValue = _lockedByKey.Count > 0;
        if (IsLocked.Value != newValue)
        {
            gameObject.SetActive(newValue);
            IsLocked.Value = newValue;
        }
    }

    public void UnlockAll(string key)
    {
        _lockedByKey.RemoveAll(x => x == key);
        UpdateActive();
        Debug.Log("UnlockAll: " + key);
    }
    public void ClearAllLocks()
    {
        _lockedByKey.Clear();
        UpdateActive();
        Debug.Log("ClearAllLocks!" );
    }
    public bool IsLockedByKey(string key)
    {
        if (string.IsNullOrEmpty(key))
            return false;
        return _lockedByKey.Contains(key);
    }
}
