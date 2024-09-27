#if CLR
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
using LogInfo;
public class LoadHotfix : MonoBehaviour
{
    public static string AotDll = "AotDll";
    public static string HotDll = "HotDll";

    public static List<TextAsset> aotdllAssets = new List<TextAsset>();
    public static List<TextAsset> hotdllAssets = new List<TextAsset>();


    public async Task Load()
    {
        Debug.Log("����aot dll");
        await DownLoadLabelDlls(AotDll, aotdllAssets);

        Debug.Log("����hot dll");
        await DownLoadLabelDlls(HotDll, hotdllAssets);


        Debug.Log("Assembly.Load => hot dll");

        foreach (var asset in hotdllAssets)
        {
            Debug.Log("dll:"+asset.name);

            if (asset.name != "Assembly-CSharp.dll")
                continue;

            var assembly = Assembly.Load(asset.bytes);
            var program  = assembly.GetType("Program");

            Debug.Log("hotfix����" + program);

            if (program != null)
            {
                Debug.Log("ʵ����aot dll");
                program.GetMethod("LoadMetadataForAOTAssemblies")?.Invoke(null, null);

                Debug.Log("ʵ����hot dll");
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
#endif
