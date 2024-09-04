using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Flow;

public class PreGoFlow : IFlowTask
{

    public  int layer => 0;
    public  int order => 20;

    public  async Task Logic()
    {
     //   var loadHandle = Addressables.LoadAssetsAsync<GameObject>
                  //     (new AssetLabelReference() { labelString = "preload" }, null);

       // await  loadHandle.Task;
       await Task.CompletedTask;
    }
}
