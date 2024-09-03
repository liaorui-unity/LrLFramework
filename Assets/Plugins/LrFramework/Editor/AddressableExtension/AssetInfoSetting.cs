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
            [Header("[Entry �ı�ǩ]")]
            public string labelName;

            public Object main;

            [Header("[��� Entry ������]")]
            public List<EntryInfo> entryInfos;
        }

        [System.Serializable]
        public class EntryInfo
        {
            public string address;
            public string addressPath;
        }

        [Header("[Entry ����Դ����]")]
        public string groupName;

        [Header("[Entry ����Դ�������]")]
        public AddressType addressType;

        [Header("[Entryʹ�ü򵥵� address]")]
        public bool isSampleName = false;

        [Header("[Entry���Զ��������ű�����]")]
        public bool isAutoNotName = false;

        [Header("[Entry ����һЩ����Ҫ�ĺ�׺�ļ�]")]
        public List<string> fifterSuffixs;

        [Header("[Entry �ı�ǩ��]")]
        public List<LabelInfo> labels;
    }
}