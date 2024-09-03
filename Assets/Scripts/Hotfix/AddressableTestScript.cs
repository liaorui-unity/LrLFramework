
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build.DataBuilders;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using NET;
using Table;
using System;
using System.Reflection;
using UnityEngine.Events;
using Audio;

public class AddressableTestScript : MonoBehaviour
{
    Delegate @delegate;

    IUnityMes mes;


    void Awake()
    {
        var method = this.GetType().GetMethod("OnMasterSerEvent");


         mes = method.Create(this);


        @delegate = InputDelegate.CreateDelegate(method, this);

       
    }

   public AssetReference asset;




    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var t = Streamable.Get<Ss>();
            Debug.Log(t);
            Debug.Log(t.f);

            var a = PoolMgr.Get("prefabA");

            Debug.Log(a);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            this.AddCheckEvent();
            this.AddInputEvents();
            this.AddMethodFuncs();

          
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            AudioLoadRes.OnGetAudioLoadInfo = (string group) =>
            {
                var infos = new List<AudioLoadInfo>();

                infos.Add(new AudioLoadInfo()
                {
                    sign = AudioLoadSign.Sound,
                    path = "sound",
                });

                infos.Add(new AudioLoadInfo()
                {
                    sign = AudioLoadSign.Subtitle,
                    path = "subtitle",
                });

                return new AudioLoadData()
                {
                    loadType = AudioLoadType.Addreassable,
                    group = "All",
                    infos = infos
                };
            };

            AudioMgr.Instance.LoadAllClip("All");

            
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            AudioMgr.Play(new List<string>() { "01_03_16", "01_03_17" });

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            AudioMgr.PlaySubtitle(new List<string>() { "01_03_16", "01_03_17" });
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            AudioMgr.PlayBgMusic("01_03_16");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            AudioMgr.Play("01_03_17");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            AudioMgr.Stop("01_03_17");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            AudioMgr.Stop();
        }


        if (Input.GetKeyDown(KeyCode.C))
        {
            TableManager.instance.Show();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            TableManager.instance.Hide(false);
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            //InputMethod.InputExit(KeyCode.Tab,34,0);
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();
            UICall.Invoke("OnMasterSerEvent", 34.0f, 35f);

            stopwatch.Stop();
            Debug.Log("CheckValue耗时：" + stopwatch.ElapsedTicks);

            System.Diagnostics.Stopwatch stopwatch1 = new System.Diagnostics.Stopwatch();
            stopwatch1.Start();
            @delegate.DynamicInvoke(34.0f, 35f);

            stopwatch1.Stop();
            Debug.Log("CheckValue耗时：" + stopwatch1.ElapsedTicks);

            System.Diagnostics.Stopwatch stopwatch2 = new System.Diagnostics.Stopwatch();
            stopwatch2.Start();
            mes.Call(0.45f,345.0f);
            stopwatch2.Stop();
            Debug.Log("gt耗时：" + stopwatch2.ElapsedTicks);
        }
        //   

        System.Diagnostics.Stopwatch stopwatch3 = new System.Diagnostics.Stopwatch();
        stopwatch3.Start();
        this.CheckValue(g);
        stopwatch3.Stop();
       // Debuger.Log("gt耗时：" + stopwatch3.ElapsedTicks);
    }



    [ChangeEvent(nameof(GG))]
    public   int g;

    public void GG(int value)
    {

    }

    [FuncMethod]
    public void OnMasterSerEvent(float old, float newValue)
    {
      //  Debuger.Log(old + "_" + newValue);
    }

    public Ss f;
}

[System.Serializable]
public class TR1 : StreamableObject
{
    public string er;
}

[System.Serializable]
public class TR2 : StreamableObject
{
    public string er;
}


[System.Serializable]
public class Ss : StreamableObject
{
    public int a;
    public FH f;
    public VerticalWrapMode mode;
    public Transform d;
}



[System.Serializable]
public class FH
{
    public string d;
}