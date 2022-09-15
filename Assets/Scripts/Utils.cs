using RSG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static MonoBehaviour _mb = null;

    public static void SetMainContainer(MonoBehaviour mainContainer)
    {
        _mb = mainContainer;
    }

    public static Promise WaitSeconds(float time)
    {
        var promise = new Promise();

        if (time == 0f)
            promise.ResolveOnce();
        else
        {
            if (_mb != null)
                _mb.StartCoroutine(timer());
            else
                Debug.LogError("_mb is null");
        }

        return promise;

        IEnumerator timer()
        {
            yield return new WaitForSeconds(time);
            promise.ResolveOnce();
        }
    }

    public static Promise WaitForFixedUpdate()
    {
        var promise = new Promise();


        if (_mb != null)
            _mb.StartCoroutine(timer());
        else
            Debug.LogError("_mb is null");

        return promise;

        IEnumerator timer()
        {
            yield return new WaitForFixedUpdate();
            promise.ResolveOnce();
        }
    }

    public static Promise WaitForEndOfFrame()
    {
        var promise = new Promise();

        if (_mb != null)
            _mb.StartCoroutine(timer());
        else
            Debug.LogError("_mb is null");

        return promise;

        IEnumerator timer()
        {
            yield return new WaitForEndOfFrame();
            promise.ResolveOnce();
        }
    }

    public static void ResolveOnce(this Promise p)
    {
        if (p.CurState == PromiseState.Pending)
            p.Resolve();
    }
}
