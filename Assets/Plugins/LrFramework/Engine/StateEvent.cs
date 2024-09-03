using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEvent 
{
    public delegate void StateEventDelegate();

    public event StateEventDelegate OnStateEnter;
    public event StateEventDelegate OnStateExit;
    public event StateEventDelegate OnStateUpdate;

    public System.Func<bool> Containing;

    protected bool inEnter;
    protected bool inStay;
    protected bool inExit;

    public StateEvent()
    {
        CallUnit.updateCall.AddListener(Update);
    }

    protected virtual bool IsNotHas()
    {
        if (OnStateExit == null && OnStateUpdate == null)
            return true;
        return false;
    }

    public void Update()
    {
        try
        {
            var isNotHas = IsNotHas();

            if (Containing())
            {
                if (!isNotHas && inStay)
                {
                    OnStateUpdate?.Invoke();
                }

                if (!inEnter && !inStay)
                {
                    inEnter = true;
                    inStay  = true;

                    OnStateEnter?.Invoke();
                }
            }
            else
            {
                if (inEnter && !inExit)
                {
                    if (!isNotHas)
                    {
                        OnStateExit?.Invoke();
                    }
                    Replace();
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("报错！跳过:" + ex);
            Replace();
        }
    }

    public void Replace()
    {
        inEnter = false;
        inStay  = false;
        inExit  = false;
    }

    public void Clear()
    {
        OnStateEnter  = null;
        OnStateExit   = null;
        OnStateUpdate = null;
        CallUnit.updateCall.RemoveListener(Update);
    }
}
