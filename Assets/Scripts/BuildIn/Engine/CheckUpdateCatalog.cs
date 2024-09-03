using UniRx;
using System.IO;
using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using System.Linq;

public class CheckUpdateCatalog : MonoBehaviour
{
    NeedUpdateGUI updateGUI;
    LoadNewRemotedAsset remotedAsset;

    static List<string> needMaps = new List<string>();

    public async Task CheckUpdte()
    {
        try
        {
            needMaps.Clear();
            updateGUI = updateGUI ?? new NeedUpdateGUI();
            remotedAsset = remotedAsset ?? new LoadNewRemotedAsset();

            ///初始化Addressables
            var init = Addressables.InitializeAsync();
            await init.Task;
            await Task.Delay(1); //实例化后要等待一帧------很重要

            /// 检查更新内容
            var updates = Addressables.CheckForCatalogUpdates(false);
            await updates.Task;
            await Task.Delay(10);//实例化后要等一会------很重要

            Debug.Log("need Updates:" + updates.Result.Count);

            await SelectUpdate(updates.Result);
            Addressables.Release(updates);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("updates:" + ex);
        }
    }

    async Task SelectUpdate(List<string> updateResults)
    {
        if (updateResults.Count > 0)
        {
            var loc = (IResourceLocator)await Addressables.LoadContentCatalogAsync(remotedAsset.remotedCatalogPath, false);

            foreach (var item in loc.Keys)
            {
                var downloadsize = await Addressables.GetDownloadSizeAsync(item).Task;
                if (downloadsize > 0)  needMaps.Add(item.ToString());
            }


            if (needMaps.Count > 0)
            {
                updateGUI.size = await Addressables.GetDownloadSizeAsync(needMaps).Task;
                updateGUI.Show();

                Debug.Log($"下载数据大小：{ updateGUI.size}");
                if (await updateGUI.waitSelect)
                {
                    //await AsyncDownAssetImpl(updates.Result);
                    await StartCoroutine(IEDownAssetImpl(updateResults));
                }
            }
        }
    }

     IEnumerator IEDownAssetImpl(List<string> updates)
    {
        Debug.Log("download result Count: "+ needMaps.Count);
        if (needMaps.Count > 0)
        {
            //var updateResult = Addressables.UpdateCatalogs(updates);
            // yield return updateResult;

            var download = Addressables.DownloadDependenciesAsync(needMaps, Addressables.MergeMode.Union,false);
            while (!download.IsDone)
            {
                updateGUI?.UpdateProgress(download.PercentComplete);
   
                yield return null;
            }
            updateGUI?.UpdateProgress(1);

            updateGUI?.CloseSpeed();

            yield return 0;

            Addressables.Release(download);
        }
        else
        {
           // var updateResult = Addressables.UpdateCatalogs(updates);
           // yield return updateResult;
        }
    }
  


    public class LoadNewRemotedAsset
    {

        public List<ResourceLocatorInfo> remotedInfos;
     
        object GetField()
        {
            var impl = typeof(Addressables).GetField("m_AddressablesInstance", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            return impl.GetType().GetField("m_ResourceLocators", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(impl);
        }


        string _remotedPath;
        public string remotedCatalogPath => _remotedPath = _remotedPath ?? GetRemoteCatalogLocationPath();

        string GetRemoteCatalogLocationPath()
        {
            string path = string.Empty;

            remotedInfos ??= (List<ResourceLocatorInfo>)GetField();

            foreach (var info in remotedInfos)
            {
                if (info.CanUpdateContent)
                {
                    if (info.HashLocation != null)
                    {
                        Debug.Log("远程地址：" + info.HashLocation.InternalId);
                        return info.HashLocation.InternalId;
                    }
                }
            }

            return path;
        }

        public string remotedDirc => Path.GetDirectoryName(remotedCatalogPath);
    }
    public class NeedUpdateGUI
    {
        public long   size;
        public float  rate;
        public float  speed;
        public long   downloadSize;

        public Subject<bool> waitSelect = new Subject<bool>();


        float previousTime = 0;

        public void Show()
        {
            LoadPanel.instance.Init(() =>
            {
                waitSelect.OnNext(true);
            });

            ShowPanel();
        }

        public void CloseSpeed()
        {
            LoadPanel.instance.HideSpeed();
        }

        public void Close()
        {
            LoadPanel.instance.Done();
        }

        public void UpdateProgress(float rate)
        {
            if (this.rate != rate)
            {
                var oldsize = this.rate * size;
              
                var offect  = Time.time - previousTime;

                this.rate   = rate;
                this.downloadSize = (long)(rate * size);
                this.speed  = (downloadSize - oldsize) / offect;
                this.previousTime = Time.time;

                ShowPanel();
            }
        }

        void ShowPanel()
        {
            LoadPanel.instance.All($"{UnitFormat(downloadSize)} / {UnitFormat(size)}");
            LoadPanel.instance.Radio(rate);
            LoadPanel.instance.Speed(UnitFormat(speed) + ChangeColor("/s"));
        }

        string UnitFormat(float value)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            int order = 0;
            while (value >= 1024 && order < sizes.Length - 1)
            {
                order++;
                value = value / 1024;
            }
            return $"{value:0.##} {ChangeColor(sizes[order])}";
        }

        string ChangeColor(string str)
        {
            str = $"<color=#1BE3D0>{str}</color>";
            str = $"<size=24>{str}</size>";
            return str;
        }
    }

}
