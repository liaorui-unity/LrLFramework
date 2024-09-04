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
}