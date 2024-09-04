using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace Table
{
    /// <summary>
    /// 通过配置表切割动画
    /// 
    /// 一 操作方式：
    /// 1.选择fbx文件，右键菜单选择Reimport
    /// 
    /// 二 配置文件格式([开始帧-结束帧]+空格+[动画名称]+空格+[是否循环]+空格+[美术备注])
    /// 1-3 idle loop 待机
    /// 5-9 attack once 攻击
    /// @author hannibal
    /// @time 2017-1-3
    /// </summary>
    public class EditorClipFbxAnim : AssetPostprocessor
    {
        public void OnPreprocessModel()
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            string asset_path = modelImporter.assetPath.ToLower();
            if (!asset_path.Contains("@") && asset_path.IndexOf("model") >= 0)
            {
                try
                {
                    var txt_path = Application.dataPath + "/../" + asset_path.Replace("fbx", "txt");
                    if(!File.Exists(txt_path))
                    {
                        Debug.LogWarning("没有动画剪辑配置文件");
                        return;
                    }
                    StreamReader file = new StreamReader(txt_path);
                    string sAnimList = file.ReadToEnd();
                    file.Close();

                    System.Collections.ArrayList List = new ArrayList();
                    ParseAnimFile(sAnimList, ref List);
                    
                    modelImporter.animationType = ModelImporterAnimationType.Generic;
                    modelImporter.clipAnimations = (ModelImporterClipAnimation[])List.ToArray(typeof(ModelImporterClipAnimation));
                }
                catch (System.Exception e)
                {
                    Debug.LogError("导入出错:" + e.Message);
                }
            }
        }

        static void ParseAnimFile(string sAnimList, ref System.Collections.ArrayList List)
        {
            Regex regexString = new Regex(" *(?<firstFrame>[0-9]+) *- *(?<lastFrame>[0-9]+) *(?<name>[^\r^\n]*[^\r^\n^ ]) *(?<loop>(Loop|loop|Once|once)) *(?<note>[^\r^\n]*[^\r^\n^ ])",
                RegexOptions.Compiled | RegexOptions.ExplicitCapture);

            Match match = regexString.Match(sAnimList, 0);
            while (match.Success)
            {
                ModelImporterClipAnimation clip = new ModelImporterClipAnimation();

                if (match.Groups["firstFrame"].Success)
                {
                    clip.firstFrame = System.Convert.ToInt32(match.Groups["firstFrame"].Value, 10);
                }
                if (match.Groups["lastFrame"].Success)
                {
                    clip.lastFrame = System.Convert.ToInt32(match.Groups["lastFrame"].Value, 10);
                }

                if (match.Groups["name"].Success)
                {
                    clip.name = match.Groups["name"].Value;
                }

                if (match.Groups["loop"].Success)
                {
                    string type = match.Groups["loop"].Value;
                    switch (type)
                    {
                        case "loop":
                        case "Loop": clip.loopTime = true; break;
                        case "once":
                        case "Once": clip.loopTime = false; break;
                    }
                }
                else
                {
                    clip.loopTime = false;
                }

                List.Add(clip);

                match = regexString.Match(sAnimList, match.Index + match.Length);
            }
        }
    }
}