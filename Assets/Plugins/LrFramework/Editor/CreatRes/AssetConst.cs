using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Linq;



namespace AddressableExtend
{

    public class AssetConst
    {
        public const string NewGroup = "AutoNew";
        public const string NewLabel = "name";

        public const string AA_Root       = "Assets/AddressableAssets";
        public const string AA_Setting    = "Assets/AddressableAssets/Setting";
        public const string AA_Asset      = "Assets/AddressableAssets/Asset";
        public const string Remoted       = "Remoted";
        public const string Local         = "Local";

        public const string Group_Sign    = "AssetGroups";
        public const string Root_Sign     = "AddressableAssets";

        public static string AA_Sign      = $"Assets/{Root_Sign}";


        public static string FD_AAFull  = $"{System.Environment.CurrentDirectory}/{AA_Asset}";
        public static string FD_Remoted = $"{System.Environment.CurrentDirectory}/{AA_Asset}/{Remoted}";
        public static string FD_Local   = $"{System.Environment.CurrentDirectory}/{AA_Asset}/{Local}";
        public static string FD_Group   = $"{System.Environment.CurrentDirectory}/{AA_Setting}/{Group_Sign}";

        public static List<string> baseDirs = new List<string> { Local, Remoted };

        public static string autoClassPath = $"{System.Environment.CurrentDirectory}/Assets/Scripts/Auto/AutoAssetClass.cs";
    }

    public class AssetClass
    { 

        public StringBuilder builder = new StringBuilder();
        public List<AssetInfoSetting> addressableCustoms = new List<AssetInfoSetting>();

        List<string> filter = new List<string>()
        {
          "Log"
        };

        public bool Intersect(string name)
        {
            foreach (var item in filter)
            {
                if (name.Contains(item))
                    return true;
            }
            return false;
        }

      

        public AssetClass(int order)
        {
            var folder = AssetConst.FD_Group;

            DirectoryInfo directory = new DirectoryInfo(folder);

            if (directory == null)
            {
                Debug.LogError($"folder not has {folder}");
                return;
            }

            var files = directory.GetFiles("*.asset", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (file.Name.EndsWith(".asset") == false )
                {
                    continue;
                }

                var asset = AssetDatabase.LoadAssetAtPath<AssetInfoSetting>(file.FullName.TrimUnityPath());
                if (asset != null)
                {
                    addressableCustoms.Add(asset);
                }
            }



            var className = directory.Name == AssetConst.Root_Sign ? "AddressFolder" : CreatUtil.GetClassString(directory.Name);

            builder.Clear();
            builder.AppendLine($"  {CreatUtil.GetFormat(order)}//自动生成的脚本类，不要修改");
            builder.AppendLine($"  {CreatUtil.GetFormat(order)}public class {className}");
            builder.AppendLine($"  {CreatUtil.GetFormat(order)}{CreatUtil.LeftSymbol}");

            var classOrder = order + 5;

            foreach (var item in addressableCustoms)
            {
                if (item.isAutoNotName)
                    continue;

                var type = item.addressType == AddressType.Remoted ? "r" : "l";
                var classGroup = FormatFiled(item.groupName.Substring(0, 1).ToUpper() + item.groupName.Substring(1));
                builder.AppendLine($"  {CreatUtil.GetFormat(classOrder)}//自动生成的脚本类，不要修改");
                builder.AppendLine($"  {CreatUtil.GetFormat(classOrder)}public class {type}{classGroup}");
                builder.AppendLine($"  {CreatUtil.GetFormat(classOrder)}{CreatUtil.LeftSymbol}");

                var group = $"{item.addressType}_{item.groupName}";
    
                var groupAddress = AddressableBuildTools.assetSettings.groups.Find(_ => _.name.Contains(group));

                if (groupAddress != null)
                {
                    var paths = groupAddress.entries?
                              .Select(_ => _.AssetPath.Replace('\\', '/'))
                              .ToList();



                    foreach (var label in item.labels)
                    {
                        string labelType = string.Empty;
                        if (label.labelName != "default") labelType = "_" + label.labelName;

                        if (label.entryInfos == null)
                            label.entryInfos = new List<AssetInfoSetting.EntryInfo>();

                        foreach (var entry in label.entryInfos)
                        {
                            if (paths.Contains(entry.addressPath))
                            {
                                paths.Remove(entry.addressPath);
                            }

                            string filed = item.isSampleName ? entry.address : Path.GetFileName(entry.address);
                            filed = FormatFiled(filed);

                            builder.AppendLine($"  {CreatUtil.GetFormat(classOrder)}{CreatUtil.SetStringField(filed, entry.address)}");
                        }
                    }


                    var finds = groupAddress.entries.Where(_ => paths.Contains(_.AssetPath.Replace('\\', '/'))).ToList();
                    foreach (var find in finds)
                    {
                        string labelType = string.Empty;
                        foreach (var label in find.labels)
                        {
                            labelType += "_" + label;
                        }

                        string filed = Path.GetFileName(find.address);
                        filed = FormatFiled(filed);

                        builder.AppendLine($"  {CreatUtil.GetFormat(classOrder)}{CreatUtil.SetStringField(filed, find.address)}");
                    }
                }
                builder.AppendLine($"  {CreatUtil.GetFormat(classOrder)}{CreatUtil.RightSymbol}");
            }
            builder.AppendLine($"  {CreatUtil.GetFormat(order)}{CreatUtil.RightSymbol}");
        }


        string FormatFiled(string filed)
        {
            return filed
                  .Replace('-', '_')
                  .Replace('.', '_')
                  .Replace("(", "")
                  .Replace(")", "");
        }
    }
}