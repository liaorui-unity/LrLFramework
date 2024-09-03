
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.AddressableAssets.Settings;

namespace AddressableExtend
{

    public class AssetProcessInfo : Editor
    {
        static Dictionary<string, int> addressDics    = new Dictionary<string, int>();
        static AddressableAssetSettings assetSettings => AddressableBuildTools.assetSettings;
        static List<AddressableAssetGroupSchema> newSchemas => 
           new List<AddressableAssetGroupSchema> 
           {
               assetSettings.DefaultGroup.Schemas[0],
               assetSettings.DefaultGroup.Schemas[1] 
           };

        public static void Init()
        {
            if (!Directory.Exists(AssetConst.FD_AAFull))
                Directory.CreateDirectory(AssetConst.FD_AAFull);
            if (!Directory.Exists(AssetConst.FD_Remoted))
                Directory.CreateDirectory(AssetConst.FD_Remoted);
            if (!Directory.Exists(AssetConst.FD_Local))
                Directory.CreateDirectory(AssetConst.FD_Local);
        }


        public static void AutoAssetID()
        {
            var isOn = EditorUtility.DisplayDialog("自动生成key", "资源整理完毕，是否自动生成 \n" +
                 "addressable Entry 对应的 cs脚本常量", "是", "否");
            if (isOn)
            {
                var auto = new AssetClass(3);

                Debug.Log(auto.builder.ToString());
            }

            AssetDatabase.Refresh();
        }


        public static void AutoAllAdaptGroupFolder()
        {
            AutoAdaptGroupFolder();
            AutoAssetID();
        }

        public static void AutoAdaptGroupFolder()
        {
            Init();
            
            addressDics.Clear();
            DirectoryInfo info = new DirectoryInfo(AssetConst.FD_AAFull);
            foreach (var item in info.GetDirectories())
            {
                if (AssetConst.baseDirs.Contains(item.Name))
                {
                    ProcessGroups(new AssetFolder(item));
                }
            }
            AssetDatabase.Refresh();
        }

        static void ProcessGroups(AssetFolder folder)
        {
            foreach (var groupSinle in folder.resGroups)
            {
                var assetGroup = assetSettings.FindGroup(groupSinle.name);
                if (assetGroup == null)
                {
                    assetGroup = assetSettings.CreateGroup(groupSinle.name, false, false, true, newSchemas);
                }

                ProcessEntry(groupSinle, assetGroup);
            }
        }

        static void ProcessEntry(AssetGroup groupSinle, AddressableAssetGroup assetGroup)
        {
            foreach (var file in groupSinle.resFiles)
            {
                var entry = assetGroup.GetAssetEntry(file.addressHash);
                if (entry == null)
                {
                    entry = assetSettings.CreateOrMoveEntry(file.addressHash, assetGroup, false, true);
                }

                if (addressDics.TryGetValue(file.address, out int count))
                {
                    entry.address = $"{file.address}({count})";
                    addressDics[file.address] = ++count;
                }
                else
                {
                    entry.address = file.address;
                    addressDics.Add(file.address, 1);
                }

                ProcessEntryLabel(file, entry);
            }
        }

        static void ProcessEntryLabel(AssetFile file, AddressableAssetEntry entry)
        {
            foreach (var label in file.addressLabel)
            {
                if (!assetSettings.GetLabels().Contains(label))
                {
                    assetSettings.AddLabel(label);
                }
                entry.SetLabel(label, true);
            }
        }


   
        public static void ReplaceUpdateContentEntry()
        {
            var updates    = new List<AddressableAssetGroup>();
            var tempGroups = new Dictionary<string, List<AddressableAssetEntry>>();

            foreach (var item in assetSettings.groups)
            {
                if (item.name.Contains(AddressableBuildTools.UpdateTag))
                {
                    updates.Add(item);
                }
            }

            foreach (var item in updates)
            {
                foreach (var entry in item.entries)
                {
                    if (!entry.AssetPath.Contains(AddressableBuildTools.ShortAddressableAssetsPath))
                    {
                        continue;
                    }

                    var paths = entry.AssetPath
                        .Replace($"{AssetConst.AA_Asset}/", "")
                        .ReplaceSymbol()
                        .Split('/');

                    var groupName = $"{paths[0]}_{paths[1]}".ReplaceTag();

                    if (tempGroups.TryGetValue(groupName, out List<AddressableAssetEntry> assetEntries))
                    {
                        assetEntries.Add(entry);
                    }
                    else
                    {
                        assetEntries = new List<AddressableAssetEntry>();
                        assetEntries.Add(entry);
                        tempGroups[groupName] = assetEntries;
                    }
                }
            }

            foreach (var item in tempGroups)
            {
                var group = assetSettings.FindGroup(item.Key);
                assetSettings.MoveEntries(item.Value, group);
            }


            foreach (var item in updates)
            {
                if (item.entries.Count <= 0)
                {
                    assetSettings.RemoveGroup(item);
                }
            }

            tempGroups.Clear();
        }
    }
}