using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

public class LoadDll : MonoBehaviour
{
    public static string AotDll = "AotDll";
    public static string HotDll = "HotDll";

    public static List<TextAsset> aotdllAssets = new List<TextAsset>();
    public static List<TextAsset> hotdllAssets = new List<TextAsset>();


    async void Start()
    {
        await transform.GetComponent<CheckUpdateCatalog>()?.CheckUpdte();

        Debuger.Log("加载aot dll");
        await DownLoadLabelDlls(AotDll, aotdllAssets);

        Debuger.Log("加载hot dll");
        await DownLoadLabelDlls(HotDll, hotdllAssets);

       
        Debuger.Log("Assembly.Load => hot dll");

        foreach (var asset in hotdllAssets)
        {
            Debuger.Log("dll:"+asset.name);

            if (asset.name != "Assembly-CSharp.dll")
                continue;

            var assembly = Assembly.Load(asset.bytes);
            var program  = assembly.GetType("HotfixProgram");

            Debuger.Log("hotfix程序：" + program);

            if (program != null)
            {
                Debuger.Log("实例化aot dll");
                program.GetMethod("LoadMetadataForAOTAssemblies")?.Invoke(null, null);

                Debuger.Log("实例化hot dll");
                program.GetMethod("Main")?.Invoke(null, null);

                break;
            }
        }
    }

    async Task DownLoadLabelDlls<T>(string label,List<T> assets)
    {
        var load = await Addressables.LoadAssetsAsync<T>( new AssetLabelReference() { labelString = label }
            ,(_) =>
            {
                if (!assets.Contains(_))
                {
                    assets.Add(_);
                }
            })
            .Task;
    }

    private void OnDestroy()
    {
        aotdllAssets.Clear();
        hotdllAssets.Clear();
    }
}
