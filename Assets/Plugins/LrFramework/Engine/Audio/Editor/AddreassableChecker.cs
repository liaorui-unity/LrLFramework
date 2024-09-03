using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public static class AddressablesDefineChecker
{
    static AddressablesDefineChecker()
    {
        CheckAndAddAddressablesDefine();
    }

    private static void CheckAndAddAddressablesDefine()
    {
        string define = "ADDRESSABLES_AVAILABLE";
        string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

        if (!scriptingDefineSymbols.Contains(define))
        {
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            if (File.Exists(manifestPath))
            {
                string jsonText = File.ReadAllText(manifestPath);
                if (jsonText.Contains("com.unity.addressables"))
                {
                    scriptingDefineSymbols += ";" + define;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, scriptingDefineSymbols);
                    Debug.Log("已添加 Addressables 预处理指令！");
                }
            }
        }
    }
}