#region Input ����
//-----------------Input ����---------------start
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// ����input��������
/// </summary>
public class InputMethod : MonoBehaviour
{
    static InputMethod _instance;

    static InputMethod instance
    { 
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("InputMethod").AddComponent<InputMethod>();
            } 
            return _instance;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void StartInit()
    {
        Debug.Log("ʵ������" + instance);
    }

    public static int m_PlayerID = 0;

    public static List<int>        playerIDs   = new List<int>();
    public static List<InputState>   keyInputs = new List<InputState>();
    public static List<KeyCode>       keyCodes = new List<KeyCode>();

    public static Dictionary<int, InputData> intInputs = new Dictionary<int, InputData>();
    public static Dictionary<KeyCode, List<int>> keyCodeInputs = new Dictionary<KeyCode, List<int>>();


    /// <summary>
    /// ����ͬ��KEY�����������¼�
    /// </summary>
    /// <param name="keyCode">����</param>
    public static void InputEnter(KeyCode keyCode, int playerID = 0)
    {
        if (keyCodeInputs.TryGetValue(keyCode,  out var list))
        {
            foreach (var item in list)
            {
                if (intInputs.TryGetValue(item, out var data))
                {
                    if (data.playerID == playerID)
                    {
                        data.TriggerInfos(keyCode);
                    }
                }
            }
        }
    }
    /// <summary>
    /// ����ͬ��KEY�����������¼�
    /// </summary>
    /// <param name="keyCode">����</param>
    public static void InputEnter<T>(KeyCode keyCode, T value, int playerID = 0) 
    {
        if (keyCodeInputs.TryGetValue(keyCode, out var list))
        {
            foreach (var item in list)
            {
                if (intInputs.TryGetValue(item, out var data))
                {
                    if (data.playerID == playerID)
                    {
                        data.TriggerInfos(keyCode, value);
                    }
                }
            }
        }
    }


    /// <summary>
    /// ����ͬ��KEY�����������¼�
    /// </summary>
    /// <param name="keyCode">����</param>
    public static void InputExit(KeyCode keyCode, int playerID = 0)
    {
        if (keyCodeInputs.TryGetValue(keyCode, out var list))
        {
            foreach (var item in list)
            {
                if (intInputs.TryGetValue(item, out var data))
                {
                    if (data.playerID == playerID)
                    {
                        data.ExitInfos(keyCode);
                    }
                }
            }
        }
    }

    public static void InputExit<T>(KeyCode keyCode,T value,  int playerID = 0)
    {
        if (keyCodeInputs.TryGetValue(keyCode, out var list))
        {
            foreach (var item in list)
            {
                if (intInputs.TryGetValue(item, out var data))
                {
                    if (data.playerID == playerID)
                    {
                        data.ExitInfos(keyCode, value);
                    }
                }
            }
        }
    }


    /// <summary>
    /// �������¼�
    /// </summary>
    public void Update()
    {
        foreach (var item in keyInputs)
        {
            item.Update();
        }
    }
}

public enum KeyState
{
    Down,
    Stay,
    Up
}

public enum StateMode
{ 
   Void,
   State,
   Value
}


public interface IAction
{
    void Call();
}



public class InputState 
{
    protected bool inDown;
    protected bool inStay;
    protected bool inExit;

    protected bool isPass = false;

    /// <summary>
    /// info�¼���Ӧ������
    /// </summary>
    KeyCode   code;
    StateMode mode;
    int     playerID;

    protected dynamic value;
    protected Delegate voidMethod;
    protected Delegate paterMethod;


    public InputState(KeyCode key, int playerID)
    {
        this.code     = key;
        this.playerID = playerID;
    }

    public void SetData<TValue>(TValue value)
    {
        this.value = value;
    }


    bool CheckCondition()
    {
        return Input.GetKey(code) && playerID == InputMethod.m_PlayerID;
    }

    bool Listance()
    {
        if (CheckCondition()) return true;
        return false;
    }

    public void Trigger()
    {
        isPass = true;
    }
    public void Exit()
    {
        isPass = false;
    }



    public void Update()
    {
        try
        {
            if (isPass || Listance())
            {
                if (IsNotHas() == false && inStay)
                {
                    ActionEvents(KeyState.Stay);
                }

                if (!inDown && !inStay)
                {
                    inDown = true;
                    inStay = true;

                    ActionEvents(KeyState.Down);

                    if (IsNotHas())
                    {
                        isPass = false;
                    }
                }
            }
            else
            {
                if (inDown && !inExit)
                {
                    if (IsNotHas() == false)
                    {
                        ActionEvents(KeyState.Up);
                    }
                    Replace();
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("��������:" + ex);
            Replace();
        }
    }



    protected virtual bool IsNotHas()
    {
        if (mode == StateMode.Value)
            return true;
        return false;
    }

    public virtual void AddMethodInfo(Delegate info)
    {
        paterMethod = info;
    }

    public virtual void SetMode(ParameterInfo[] parameters, Delegate info)
    {
        if (parameters.Length <= 0)
        {
            mode = StateMode.Void;
            voidMethod = info;
        }
        else 
        {
            bool isMode = false;

            if     (parameters.Length == 1)
            {
                if (parameters[0].ParameterType == typeof(KeyState))
                {
                    mode   = StateMode.State;
                    isMode = true;
                }
            }
            else if (parameters.Length == 2)
            {
                if (parameters[0].ParameterType == typeof(KeyState))
                {
                    mode   = StateMode.Value;
                    isMode = true;
                }
            }

            paterMethod = info;

            if (isMode == false)
            {
                Debug.LogError($"[{code}] KeyCode is not rule");
            }
        }
    }


    protected virtual void ActionEvents(KeyState state)
    {
        if (mode == StateMode.Void)
        {
            voidMethod?.DynamicInvoke();
        }
        else if (mode == StateMode.State)
        {
            paterMethod?.DynamicInvoke(state);
        }
        else if(mode == StateMode.Value)
        {
            paterMethod?.DynamicInvoke(state, value);
        }
    }


    public void Replace()
    {
        inDown = false;
        inStay = false;
        inExit = false;

        isPass = false;
    }


    public void RemoveAll()
    {
        paterMethod = null;
    }
}





/// <summary>
/// Input����
/// </summary>
public class InputData
{
    public Dictionary<KeyCode, InputState> keyInfos = new Dictionary<KeyCode, InputState>();

    public int playerID;// �ű���������ID

    /// <summary>
    /// ����ű�ʵ��
    /// </summary>
    public InputData(int playerID)
    {
        this.playerID = playerID;
    }



    /// <summary>
    /// ���input�����¼�
    /// </summary>
    public void AddInfo(KeyCode key, MethodInfo info , object instance)
    {
        if (keyInfos.ContainsKey(key) == false)
        {
            var state = new InputState(key, playerID);
            InputMethod.keyInputs.Add(state);
            keyInfos[key] = state;
        }

        keyInfos[key].SetMode(info.GetParameters(), InputDelegate.CreateDelegate(info, instance));
    }

    /// <summary>
    /// ���input�����¼�
    /// </summary>
    public void AddInfo(KeyCode key, MethodInfo info, object instance, object value)
    {
        if (keyInfos.ContainsKey(key) == false)
        {
            var state = new InputState(key, playerID);
            InputMethod.keyInputs.Add(state);
            keyInfos[key] = state;
        }

        keyInfos[key].SetData(value);
        keyInfos[key].SetMode(info.GetParameters(), InputDelegate.CreateDelegate(info, instance));
    }

    /// <summary>
    /// ������Ӧkey�ĺ���
    /// </summary>
    /// <param name="key"></param>
    public void TriggerInfos<T>(KeyCode key, T value) 
    {
        if (keyInfos.TryGetValue(key, out var state))
        {
            state.SetData(value);
            state.Trigger();
        }
    }

    /// <summary>
    /// ������Ӧkey�ĺ���
    /// </summary>
    /// <param name="key"></param>
    public void TriggerInfos(KeyCode key)
    {
        if (keyInfos.ContainsKey(key))
        {
            keyInfos[key].Trigger();
        }
    }



    /// <summary>
    /// ������Ӧkey�ĺ���
    /// </summary>
    /// <param name="key"></param>
    public void ExitInfos(KeyCode key)
    {
        if (keyInfos.ContainsKey(key))
        {
            keyInfos[key].Exit();
        }
    }

    public void ExitInfos<T>(KeyCode key, T value)
    {
        if (keyInfos.TryGetValue(key, out var state))
        {
            state.SetData(value);
            state.Exit();
        }
    }


    /// <summary>
    /// ɾ�����е�info����
    /// </summary>
    public void RemoveAllInfos()
    {
        foreach (var item in keyInfos.Values)
        {
            item.RemoveAll();
        }
        keyInfos.Clear();
    }
}



public static class InputDelegate
{
    public static Delegate CreateDelegate(MethodInfo methodInfo, object target)
    {
        var paras = new List<ParameterExpression>();


        for (int i = 0; i < methodInfo.GetParameters().Length; i++)
        {
            var item = methodInfo.GetParameters()[i];
            paras.Add(Expression.Parameter(item.ParameterType, "arg"));
        }
           
        
        var meters = (IEnumerable<ParameterExpression>)paras;

        var instance   = Expression.Constant(target);
        var methodCall = Expression.Call(instance, methodInfo, meters);
        var lambda     = Expression.Lambda(methodCall, meters);

        return lambda.Compile();
    }
}


/// <summary>
/// ������뺯������
/// </summary>
public class InputAttribute : System.Attribute
{
    /// <summary>
    /// ������KEY
    /// </summary>
    public KeyCode  keyCode;

    public object   value;

    #region ���캯��
    /// <summary>
    /// ���캯����ͬ���÷�
    /// </summary>
    public InputAttribute()
    {

    }

    public InputAttribute(KeyCode keyCode)
    {
        this.keyCode = keyCode;
    }


    public InputAttribute(KeyCode keyCode,object value)
    {
        this.keyCode  = keyCode;
        this.value    = value;
    }


    #endregion
}

/// <summary>
/// �������ݴ���
/// </summary>
public static class InputAttributeExtenders
{
    /// <summary>
    /// ������Ҫ��ⰴť�¼��Ľű�
    /// </summary>
    /// <param name="target"></param>
    /// <param name="playerID"></param>
    public static void AddInputEvents(this object target, int playerID = 0)
    {
        var fields    = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        var inputData = new InputData(playerID);

        for (int i = 0; i < fields.Length; i++)
        {
            InputAttribute[] attrs = fields[i].GetCustomAttributes(typeof(InputAttribute), false) as InputAttribute[];
            if (attrs != null && attrs.Length > 0)
            {
                var inputValue = attrs[0];

                if (inputValue.value != null)
                {
                    inputData.AddInfo(inputValue.keyCode, fields[i], target, inputValue.value);
                }
                else
                {
                    inputData.AddInfo(inputValue.keyCode, fields[i], target);
                }

                if (InputMethod.keyCodes.Contains(inputValue.keyCode) == false)
                {
                    InputMethod.keyCodes.Add(inputValue.keyCode);
                }


                if (InputMethod.keyCodeInputs.TryGetValue(inputValue.keyCode, out var inputs) == false)
                {
                    inputs = new List<int>();
                    inputs.Add(target.GetHashCode());
                    InputMethod.keyCodeInputs[inputValue.keyCode] = inputs;
                }
                else
                {
                    inputs.Add(target.GetHashCode());
                }
            }
        }

        if (!InputMethod.playerIDs.Contains(playerID))
             InputMethod.playerIDs.Add(playerID);

        InputMethod.intInputs.Add(target.GetHashCode(), inputData);
    }


    /// <summary>
    /// �Ƴ����Ľű�
    /// </summary>
    /// <param name="target"></param>
    public static void RemoveInputEvents(this object target)
    {
        if (InputMethod.intInputs.ContainsKey(target.GetHashCode()))
        {
            InputMethod.intInputs[target.GetHashCode()].RemoveAllInfos();
            InputMethod.intInputs.Remove(target.GetHashCode());
        }

        foreach (var item in InputMethod.keyCodeInputs.Values)
        {
            if (item.Contains(target.GetHashCode()))
            {
                item.Remove(target.GetHashCode());
            }
        }
    }
}
//-----------------Input ����---------------end
#endregion

