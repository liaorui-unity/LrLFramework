
using HybridCLR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public  class HotfixProgram
{

    public static void Main()
    {
        GameObject main = new GameObject("HotfixMain",typeof(HotfixMain));
    }


    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    public static void LoadMetadataForAOTAssemblies()
    {
        // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
        // 我们在BuildProcessors里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        foreach (var aotDllbyte in LoadDll.aotdllAssets)
        {

            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(aotDllbyte.bytes,HomologousImageMode.SuperSet);
            Debuger.Log($"LoadMetadataForAOTAssembly:{aotDllbyte.name}. ret:{err}");
        }
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
