using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UniRx;
using System.Threading.Tasks;
using Sailfish;

public class HotfixMain : MonoBehaviour
{
    FlowGame flowGame;

    private void Awake()
    {
        Debuger.Log("=======运行了项目的热更新代码=======");
        DontDestroyOnLoad(this.gameObject);
        Flow();
    }
    

    public async void Flow()
    {
        LoadPanel.instance.Title("资源预加载");
        flowGame = new FlowGame();
        flowGame . FindFlow();

        var allNum  = flowGame.tasks.Count;
        var cuttent = 0;
        await flowGame.Start(()=>
        {
            cuttent++;
            LoadPanel.instance.Radio(cuttent/(float)allNum);
        });

        LoadPanel.instance.Title("预加载完毕");
        await Task.Delay(1000);
        LoadPanel.instance.Done();
    }

}
