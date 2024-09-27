
#if CLR
using HybridCLR;
#endif

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Program
{
#if !CLR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        Debug.Log("BeforeSceneLoad");
        MainLogic.startup.AddListener(Main);
    }
#endif
    public static void Main()
    {
        if (Application.isPlaying)
        {
            var MainStep = new GameObject("MainStep", typeof(MainStep));
        }
        MainLogic.startup.RemoveListener(Main);
    }

    /// <summary>
    /// Ϊaot assembly����ԭʼmetadata�� ��������aot�����ȸ��¶��С�
    /// һ�����غ����AOT���ͺ�����Ӧnativeʵ�ֲ����ڣ����Զ��滻Ϊ����ģʽִ��
    /// </summary>
    public static void LoadMetadataForAOTAssemblies()
    {
#if CLR
        // ���Լ�������aot assembly�Ķ�Ӧ��dll����Ҫ��dll������unity build���������ɵĲü����dllһ�£�������ֱ��ʹ��ԭʼdll��
        // ������BuildProcessors������˴�����룬��Щ�ü����dll�ڴ��ʱ�Զ������Ƶ� {��ĿĿ¼}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} Ŀ¼��

        /// ע�⣬����Ԫ�����Ǹ�AOT dll����Ԫ���ݣ������Ǹ��ȸ���dll����Ԫ���ݡ�
        /// �ȸ���dll��ȱԪ���ݣ�����Ҫ���䣬�������LoadMetadataForAOTAssembly�᷵�ش���
        foreach (var aotDllbyte in LoadHotfix.aotdllAssets)
        {

            // ����assembly��Ӧ��dll�����Զ�Ϊ��hook��һ��aot���ͺ�����native���������ڣ��ý������汾����
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(aotDllbyte.bytes,HomologousImageMode.SuperSet);
            Debuger.Log($"LoadMetadataForAOTAssembly:{aotDllbyte.name}. ret:{err}");
        }
#endif
    }


    public static async Task<List<T>> DownLoadType<T>(string label)
    {
        var assets = new List<T>();
        var load = await Addressables.LoadAssetsAsync<T>
        (new AssetLabelReference() { labelString = label },
                                (_) =>
                                {
                                    if (!assets.Contains(_))
                                        assets.Add(_);
                                }
        ).Task;
        return assets;
    }

    public static async Task<T> DownLoadType<T>(string label, string findName)
    {
        var asset = default(T);
        var load = await Addressables.LoadAssetsAsync<T>
        (new AssetLabelReference() { labelString = label },
                                (_) =>
                                {
                                    if (_.ToString().Contains(findName))
                                        asset = _;
                                }
        ).Task;
        return asset;
    }


    public static async Task DownLoadInstantiateType(string label)
    {
        await Addressables.LoadAssetsAsync<GameObject>
        (
            new AssetLabelReference() { labelString = label },
            (_) =>
            {
               GameObject.Instantiate(_).name = _.name;
            }
        ).Task;
    }
}
