using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateCheckInfo
{
    public UnityAction OnStateEnter;
    public UnityAction OnStateExit;
    public UnityAction<float> OnStateProgress;
}


public class AnimaorStateCheck 
{
    public static Dictionary<Animator, StateData> animatorChecks = new Dictionary<Animator, StateData>();

    public class StateData
    {
        public Dictionary<string, StateCheck> checks = new Dictionary<string, StateCheck>();

        public void Add(string stateName, StateCheck check)
        {
            if (checks.ContainsKey(stateName) == false)
            {
                checks.Add(stateName, check);
            }
        }

        public void Remove(string stateName)
        {
            if(checks.TryGetValue(stateName,out var check))
            {
                check.Release();
                checks.Remove(stateName);
            }
        }

        public void RemoveAll()
        {
            foreach (var item in checks.Values)
            {
                item.Release(); 
            }
            checks.Clear();
        }
    }


    public class StateCheck
    {
        public Animator _animator;
        string _stateName = "";
        float _timeout = 10;
        Int32 _layerID = 0;
        AnimatorStateInfo info;

        bool isEnter = false;
        bool isExit  = false;

        public StateCheckInfo stateInfo;

        public StateCheck(Animator animator)
        {
            _animator  = animator;
            stateInfo  = new StateCheckInfo();
        }

        public void Set(string stateName, float time = 30, int layerID = 0)
        {
            _timeout = time;
            _layerID = layerID;
            _stateName = stateName;

            isEnter = false;
            isExit  = false;

            CallUnit.updateCall.AddListener(Check);
        }

        public bool IsState(string stateName, ref AnimatorStateInfo info)
        {
            info = _animator.GetCurrentAnimatorStateInfo(_layerID);
            return info.IsName(stateName);
        }

        public void Check()
        {
            if (_animator == null)
            {
                StopCheck();
                return;
            }

            if (IsState(_stateName, ref info))
            {
                if (isEnter == false)
                {
                    stateInfo.OnStateEnter?.Invoke();
                    isEnter  = true;
                }
                if (isEnter)
                {
                    var radio = (info.normalizedTime % info.length) / info.length;
                    stateInfo.OnStateProgress?.Invoke(radio);
                    if (info .loop == false)
                    {
                        if (radio >= 0.99f)
                        {
                            stateInfo.OnStateExit?.Invoke();
                            StopCheck();
                            isExit = true;
                        }
                    }
                }
            }
            else
            {
                if (isExit == false && isEnter)
                {
                    stateInfo.OnStateExit?.Invoke();
                    StopCheck();
                    isExit = true;
                }
                if (_timeout <= 0)
                {
                    Debug.LogError("Check Animator State Timeout");
                    StopCheck();
                    return;
                }
                _timeout -= Time.deltaTime;
            }
        }

        public void StopCheck()
        { 
           Remove(_animator, _stateName);
        }

        public void Release()
        {
            CallUnit.updateCall.RemoveListener(Check);
            _animator  = null;
            _timeout   = float.MaxValue;
            stateInfo = null;
        }
     }


    


    public static void Remove(Animator animator)
    { 
        if (animatorChecks.TryGetValue(animator, out var data))
        {
            data.RemoveAll();
            data = null;
            animatorChecks.Remove(animator);
        }
    }

    public static void Remove(Animator animator, string stateName)
    {
        if (animatorChecks.TryGetValue(animator, out var data))
        {
            data.Remove(stateName);
        }
    }

    public static StateCheck Add(Animator animator, string stateName)
    {
        if (animatorChecks.TryGetValue(animator, out var data))
        {
            StateCheck check = new StateCheck(animator);
            data.Add(stateName, check);
        }
        else
        {
            StateCheck check    = new StateCheck(animator);
            StateData stateData = new StateData();
            stateData.Add(stateName, check);
            animatorChecks.Add(animator, stateData);
        }

        return animatorChecks[animator].checks[stateName];
    }

    internal static StateCheck Create(Animator animator)
    {
        return new StateCheck(animator);
    }
}

public static class AnimatorStateCheckExtend
{ 

    public static StateCheckInfo AddStateCheck(this Animator animator, string stateName, float time = 30, int layerID = 0)
    {
        var check = AnimaorStateCheck.Add(animator, stateName);
        check.Set(stateName, time, layerID);
        return check.stateInfo;
    }

    public static StateCheckInfo AddStateCheck(this Animator animator, string stateName)
    {
        var check = AnimaorStateCheck.Add(animator, stateName);
        check.Set(stateName);
        return check.stateInfo;
    }

    public static void RemoveStateCheck(this Animator animator)
    {
        AnimaorStateCheck.Remove(animator);
    }

    public static void RemoveStateCheck(this Animator animator, string stateName)
    {
        AnimaorStateCheck.Remove(animator, stateName);
    }


}
