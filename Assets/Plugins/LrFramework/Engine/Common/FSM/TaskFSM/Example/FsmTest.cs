using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class FsmTest : MonoBehaviour
{

    FSMTaskMonitor fsmTaskMinator = new FSMTaskMonitor();

    GotoRun gotoRun;

    // Start is called before the first frame update
    void Start()
    {
        fsmTaskMinator.Add(new Idle());
        fsmTaskMinator.Add(new Walk());
        fsmTaskMinator.Add(new Run());



        gotoRun = new GotoRun();
    }

    // Update is called once per frame
    void Update()
    {

        fsmTaskMinator.Update();

        if (Input.GetKeyDown(KeyCode.G))
        {
            fsmTaskMinator.Input(gotoRun);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            fsmTaskMinator.Skip();
        }
    }

    class Run : FSMTaskTransfer
    {
        public override void Enter()
        {
            base.Enter();
            Debug.Log("In:"+this.GetType());
        }
    }

    class Walk : FSMTaskTransfer
    {
        public override void Enter()
        {
            base.Enter();

            Debug.Log("In:" + this.GetType());
        }
    }


    class Idle : FSMTaskTransfer
    {
        public override void Enter()
        {
            base.Enter();

            Debug.Log("In:" + this.GetType());
        }
    }

    class GotoRun : FSMTaskConditions
    {
        public override bool Condition()
        {
            return Input.GetKeyDown(KeyCode.G);
        }
    }
}


