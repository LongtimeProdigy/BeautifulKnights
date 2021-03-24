using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NGUIExtension
{
    public static void SetTriggerEventOnClick(this GameObject _gameObject, MonoBehaviour _target, string _methodName, object _param = null)
    {
        UIEventTrigger trigger = _gameObject.GetComponent<UIEventTrigger>();
        EventDelegate del = new EventDelegate();
        del.target = _target;
        del.methodName = _methodName;
        if(_param != null)
        {
            EventDelegate.Parameter param = new EventDelegate.Parameter(_param);
            del.parameters[0] = param;
        }

        trigger.onClick.Add(del);
    }

    public static void SetTriggerEventOnClick(this GameObject _gameObject, MonoBehaviour _target, string _methodName, params object[] _params)
    {
        UIEventTrigger trigger = _gameObject.GetComponent<UIEventTrigger>();
        EventDelegate del = new EventDelegate();
        del.target = _target;
        del.methodName = _methodName;
        for(int i = 0; i < _params.Length; i++)
        {
            EventDelegate.Parameter param = new EventDelegate.Parameter(_params[i]);
            del.parameters[i] = param;
        }

        trigger.onClick.Add(del);
    }
}
