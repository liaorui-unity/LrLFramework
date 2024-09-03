using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AddressableExtend
{

    public enum AddressType
    {
        Remoted, Local
    }

    public class AssetInfoSetting : ScriptableObject
    {
        public Object main;

        [System.Serializable]
        public class LabelInfo
        {
            [Header("[Entry 的标签]")]
            public string labelName;

            public Object main;

            [Header("[存放 Entry 的数组]")]
            public List<EntryInfo> entryInfos;
        }

        [System.Serializable]
        public class EntryInfo
        {
            public string address;
            public string addressPath;
        }

        [Header("[Entry 的资源组名]")]
        public string groupName;

        [Header("[Entry 的资源组的类型]")]
        public AddressType addressType;

        [Header("[Entry使用简单的 address]")]
        public bool isSampleName = false;

        [Header("[Entry不自动化构建脚本常量]")]
        public bool isAutoNotName = false;

        [Header("[Entry 过滤一些不需要的后缀文件]")]
        public List<string> fifterSuffixs;

        [Header("[Entry 的标签组]")]
        public List<LabelInfo> labels;
    }
}