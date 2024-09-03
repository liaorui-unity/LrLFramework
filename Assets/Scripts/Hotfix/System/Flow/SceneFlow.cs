using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SceneFlow : IFlowTask
{
    public int layer => 0;

    public int order => 10;

    public  async Task Logic()
    {
        Debug.Log("º”‘ÿ≥°æ∞");

        await Observable.ToObservable(Progess());

        await Task.Delay(1000);
    }

    IEnumerator Progess()
    {
        var loadHandle = Addressables.LoadSceneAsync("Assets/Scenes/hotfix");

        while (!loadHandle.IsDone)
        {
            // LoadWindowMgr.instance.Radio(loadHandle.PercentComplete);
            yield return 0;
        }
        // LoadWindowMgr.instance.Radio(1.0f);

        yield return new WaitForSeconds(1.0f);

    }
}
