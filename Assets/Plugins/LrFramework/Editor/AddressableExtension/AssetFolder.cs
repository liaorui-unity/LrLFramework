using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;
using System.Text;
using dnlib.DotNet.Resources;

namespace AddressableExtend
{
    public class AssetFile
    {
        public string   address;
        public string   addressHash;
        public string   addressPath;
        public string[] addressLabel;
    }


    public class AssetFolder
    {
        public string group;
        public List<AssetGroup> resGroups = new List<AssetGroup>();

        public AssetFolder(DirectoryInfo group)
        {

            this.group = group.Name;
            var direcs = group.GetDirectories();
            var groups = direcs.Where((_) => _.Name.Contains(AddressableBuildTools.GroupTag));

            foreach (var item in groups)
            {
                resGroups.Add(new AssetGroup(item)
                {
                    name = $"{this.group}_{item.Name}".Replace(AddressableBuildTools.GroupTag, "")
                });
            }
        }
    }


    public class AssetGroup
    {

        public string name = "Main";
        public List<AssetFile> resFiles;

        AssetInfoSetting customGroup;

        public AssetGroup(DirectoryInfo group)
        {
            resFiles    = new List<AssetFile>();
            customGroup = AssetFolderMenu.GetAssetInfo(group.FullName);

            GroupFindLabels(group);

            EditorUtility.SetDirty(customGroup);
            AssetDatabase.SaveAssets();
        }

        void GroupFindLabels(DirectoryInfo dirName)
        {
            //搜集当前组 Label 的 物体
            var dirs = dirName.GetDirectories();
            foreach (var item in dirs)
            {
                var label = item.Name.Replace(AddressableBuildTools.LabelTag, "");
                var labelTags = label.Split('_');

                LabelFindFiles(new DirectoryInfo(item.FullName), label, labelTags);
            }

                 //搜集当前组没有label的物体
            var defaultInfo = GetLabelInfo( "default");
            LabelFiles(dirName.GetFiles(), defaultInfo, new string[] { });

            RemoveSurplus(dirs);
        }


        void RemoveSurplus(DirectoryInfo[] dirs)
        {
            List<string> filters = new List<string>() { "default" };
            List<AssetInfoSetting.LabelInfo> needInfos = new List<AssetInfoSetting.LabelInfo>();

            foreach (var item in dirs)
            {
                filters.Add(item.Name.Replace(AddressableBuildTools.LabelTag, ""));
            }


            foreach (var item in customGroup.labels)
            {
                if (filters.Contains(item.labelName))
                {
                    continue;
                }
                needInfos.Add(item);
            }

            foreach (var item in needInfos)
            {
                customGroup.labels.Remove(item);
            }

          
        }

        void LabelFindFiles(DirectoryInfo dirName, string label, string[] labels)
        {
            AssetInfoSetting.LabelInfo labelInfo = GetLabelInfo( label);

            LabelFiles(dirName.GetFiles(), labelInfo, labels);

            if (dirName.GetDirectories().Length > 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("格式不对，文件夹层数过多 => 标准 Local");
                builder.AppendLine("                               L(Group)x");
                builder.AppendLine("                                   L(Label)y ");
                Debug.LogError(builder.ToString());
            }
        }

        private AssetInfoSetting.LabelInfo GetLabelInfo( string label)
        {
            if (customGroup.labels == null)
            {
                customGroup.labels = new List<AssetInfoSetting.LabelInfo>();
            }

            var labelInfo = customGroup.labels?.Find(_ => _.labelName == label);
            if (labelInfo == null)
            {
                 labelInfo = AssetFolderMenu.CreatLabel(customGroup, label);
            }

            labelInfo.entryInfos?.Clear();
            return labelInfo;
        }

        private void LabelFiles(FileInfo[] files, AssetInfoSetting.LabelInfo labelInfo, string[] labels)
        {
            if (files.Length <= 0)
                return;


            foreach (var file in files)
            {
                if (IsJudge(file.Name) || file.Name.EndsWith(".meta"))
                    continue;

                var subKey = file.FullName
                    .ReplaceSymbol()
                    .Replace(AssetConst.FD_AAFull, "")
                    .Replace(AddressableBuildTools.GroupTag, "")
                    .Replace(AddressableBuildTools.LabelTag, "");


                var subValue = file.FullName
                    .ReplaceSymbol()
                    .TrimUnityPath();


                AssetFile resFile = new AssetFile();
                resFile.address   = IsSamepleName(customGroup.isSampleName, subKey);
                resFile.addressPath   = subValue;
                resFile.addressLabel  = labels;
                resFile.addressHash   = AssetDatabase.AssetPathToGUID(subValue);
                resFiles.Add(resFile);


                labelInfo.entryInfos.Add(new AssetInfoSetting.EntryInfo()
                {
                    address = resFile.address,
                    addressPath = resFile.addressPath
                });
            }
        }

        public bool IsJudge(string name)
        {
            foreach (var item in customGroup.fifterSuffixs)
            {
                if(name.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        public string IsSamepleName(bool isSample, string key)
        {
            if (!isSample)
            {
                return key.Remove(key.LastIndexOf('.'));
            }
            else
            {
                return Path.GetFileNameWithoutExtension(key);
            }
        }
    }

   
}