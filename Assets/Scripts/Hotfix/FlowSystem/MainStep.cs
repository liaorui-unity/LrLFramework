using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UniRx;
using System.Threading.Tasks;
using Sailfish;
using Flow;

public class MainStep : MonoBehaviour
{
    private void Awake()
    {
        Debuger.Log("=======启动流程逻辑=======");
        DontDestroyOnLoad(this.gameObject);
        StartFlow();
    }


    public async void StartFlow()
    {
        LoadPanel.instance.Title("资源预加载");

        FlowAppend.GetAppends();
        FlowGame  .GetLayerFlow();
        
        FlowAppend.Prefix();

        await FlowGame.StartSync(()=>
        {
            Debug.Log("进度："+FlowGame.Progress);
            LoadPanel.instance.Radio(FlowGame.Progress);
        });

        FlowAppend.Postfix();
        FlowGame.Clear();

        LoadPanel.instance.Title("预加载完毕");
        await Task.Delay(1000);
        LoadPanel.instance.Done();
    }


}
