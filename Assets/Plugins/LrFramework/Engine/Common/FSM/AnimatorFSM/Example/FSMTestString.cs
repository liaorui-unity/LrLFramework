using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sailfish;


public class FsmState_Run : FSMTransfer<string>
{
    public override void Enter()
    {
        Debug.Log("进入Run");
    }
}

public class FsmState_Wark : FSMTransfer<string>
{
    public override void Enter()
    {
        Debug.Log("进入Wark");
    }
}


public class FsmState_Idle : FSMTransfer<string>
{
    public override void Enter()
    {
        Debug.Log("进入Idle");
    }
}


public class FSMTestString : MonoBehaviour
{
    FSMMonitor<string> monitor;
    public string currentKey;

    // Start is called before the first frame update
    void Start()
    {
        monitor = new FSMMonitor<string>();

        FsmState_Run  state1 = new FsmState_Run();
        FsmState_Wark state2 = new FsmState_Wark();
        FsmState_Idle state3 = new FsmState_Idle();

        monitor.ConcatsTransfer("Run",  state1);
        monitor.ConcatsTransfer("Walk", state2);
        monitor.ConcatsTransfer("Idle", state3);

        monitor.Default(state3);


        state3.Connect("Walk", state2);
        state3.Connect("Run",  state1);

        state2.Connect("Idle", state3);
        state2.Connect("Run",  state1);

        state1.Connect("Idle", state3);

        state1.Disconnect(state3);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            monitor.ConditionsTransfer(currentKey);
        }

        if (Input.GetKeyUp(KeyCode.G))
        {

        }
    }
}
