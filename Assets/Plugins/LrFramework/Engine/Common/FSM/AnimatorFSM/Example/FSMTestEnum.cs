using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sailfish;

public class FsmEnem_Run : FSMTransfer<TestEnum>
{
    public override void Enter()
    {
        Debug.Log("进入Run");
    }
}

public class FsmEnem_Wark : FSMTransfer<TestEnum>
{
    public override void Enter()
    {
        Debug.Log("进入Wark");
    }
}


public class FsmEnem_Idle : FSMTransfer<TestEnum>
{
    public override void Enter()
    {
        Debug.Log("进入Idle");
    }
}


public enum TestEnum
{
    Idle,
    Walk,
    Run
}

public class FSMTestEnum : MonoBehaviour
{
    public TestEnum test;
    FSMMonitor<TestEnum> monitor;

    // Start is called before the first frame update
    void Start()
    {
        monitor = new FSMMonitor<TestEnum>();

        FsmEnem_Run state1  = new FsmEnem_Run();
        FsmEnem_Wark state2 = new FsmEnem_Wark();
        FsmEnem_Idle state3 = new FsmEnem_Idle();

        monitor.ConcatsTransfer(TestEnum.Run,  state1);
        monitor.ConcatsTransfer(TestEnum.Walk, state2);
        monitor.ConcatsTransfer(TestEnum.Idle, state3);

        monitor.Default(state3);


        state3.Connect(TestEnum.Walk, state2);
        state3.Connect(TestEnum.Run,  state1);

        state2.Connect(TestEnum.Idle, state3);
        state2.Connect(TestEnum.Run,  state1);

        state1.Connect(TestEnum.Idle, state3);

        monitor.InputMsg("TestEnum");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            monitor.ConditionsTransfer(test);
        }
    }
}
