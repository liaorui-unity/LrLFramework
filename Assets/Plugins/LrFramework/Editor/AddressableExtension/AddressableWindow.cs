using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using System.IO;
using UnityEditor;
using UnityEngine.U2D;
using UnityEditor.U2D;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using System.Threading.Tasks;
using System.Linq;
using UnityEditor.Callbacks;
using UnityEngine.Events;


public enum BuildEnvironment
{
    Local,
    Debug,
    Release
}

public enum BuildServer
{
    Local,
    Remote,
}


public class AddressableWindow : EditorWindow
{
    private static AddressableAssetSettings setting => AddressableBuildTools.assetSettings;

    static AddressablePath _addressableConfig;
    public static AddressablePath addressableConfig
    {
        get
        {
            if (_addressableConfig == null)
            {
                _addressableConfig = AssetDatabase.LoadAssetAtPath<AddressablePath>(urlPath);
                if (_addressableConfig == null)
                {
                    _addressableConfig = ScriptableObject.CreateInstance<AddressablePath>();

                    _addressableConfig.infos = new List<PathInfo>()
                    {
                        new PathInfo(){ environment = BuildEnvironment.Local,   version = "1.0.0"},
                        new PathInfo(){ environment = BuildEnvironment.Debug,   version = "1.0.0"},
                        new PathInfo(){ environment = BuildEnvironment.Release, version = "1.0.0"},
                    };
                    _addressableConfig.Save();
                    AssetDatabase.CreateAsset(_addressableConfig, urlPath);
                }
                _addressableConfig.Load();
            }
            return _addressableConfig;
        }
    }

    [MenuItem("BuildTools/Compile  => HotfixDll Add Build")]
    public static void Compile2Ab()
    {
        Compile();
        AssetDatabase.Refresh();
        BuildContent();
    }



    [MenuItem("BuildTools/Compile  => HotfixDll")]
    public static void Compile()
    {
       CopyAllAssembliesPostIl2CppStripDir();
    }


    [MenuItem("BuildTools/Generate => Link All")]
    public static void Generate()
    {
        PrebuildCommand.GenerateAll();
    }


    [MenuItem("BuildTools/Window  => AddressableWindow")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AddressableWindow));
    }

    [MenuItem("Assets/Lr/AddressableBuildWindow",false ,01)]
    public static void Addressable()
    {
        EditorWindow.GetWindow(typeof(AddressableWindow));
    }

    [MenuItem("Assets/Lr/", false, 01)]
    public static void LR() { }


    private void OnEnable()
    {
        this.titleContent = new GUIContent("Addressable 打包窗口");
        isChooseTarget = false;

        environment = (BuildEnvironment)System.Enum.Parse(typeof(BuildEnvironment), PlayerPrefs.GetString("BuildEnvironment", BuildEnvironment.Debug.ToString()));
    

        SetActiveProfileId();

    }

    private void OnDisable()
    {
        PlayerPrefs.SetString(environment.ToString() + "_BuildUrl", loadurl);
        PlayerPrefs.SetString("BuildEnvironment", environment.ToString());
    }

    #region 自动标记

    #region 图集
    public static string AtlasRoot
    {
        get
        {
            return Application.dataPath + "/AddressableAssets/Local/Atlas";
        }
    }
    public static string SpriteAtlas
    {
        get
        {
            return Application.dataPath + "/AddressableAssets/Local/SpriteAtlas";
        }
    }

    public static void AutoCreateSpriteAtlas()
    {
        DirectoryInfo[] dirs = new DirectoryInfo(AtlasRoot).GetDirectories();
        foreach (var info in dirs)
        {
            addSpriteAtlas(AtlasRoot + "/" + info.Name, info);
        }
    }

    /// <summary>
    /// 自动创建图集
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="dir">文件夹</param>
    private static void addSpriteAtlas(string path, DirectoryInfo dir)
    {
        var dirs = dir.GetDirectories();
        if (dirs == null || dirs.Length == 0)
        {
            string name = path.Replace(AtlasRoot + "/", string.Empty).Replace("/", "_");
            string filePath = SpriteAtlas + "/" + name + ".spriteatlas";
            if (File.Exists(filePath))
            {
                int assetIndex = filePath.IndexOf("Assets");
                string guidPath = filePath.Remove(0, assetIndex);
                var guid = AssetDatabase.AssetPathToGUID(guidPath);
                var group = setting.FindGroup("Local_SpriteAtlas");
                var entry = setting.CreateOrMoveEntry(guid, group);
                var label = name + ".spriteatlas";
                if (entry.address != name)
                {
                    entry.SetAddress(name);
                    // addAddressInfo("Local_SpriteAtlas", name);
                }
                List<string> oldLabels = new List<string>();
                foreach (var item in entry.labels)
                {
                    if (item != label)
                        oldLabels.Add(item);
                }
                for (int i = 0; i < oldLabels.Count; i++)
                {
                    entry.SetLabel(oldLabels[i], false);
                    setting.RemoveLabel(oldLabels[i]);
                }
                if (!setting.GetLabels().Contains("SpriteAtlas"))
                {
                    setting.AddLabel("SpriteAtlas");
                }
                entry.SetLabel("SpriteAtlas", true);
                if (!setting.GetLabels().Contains(label))
                {
                    setting.AddLabel(label);
                }
                entry.SetLabel(label, true);
                return;
            }
            else
            {
                SpriteAtlas atlas = new SpriteAtlas();

                //设置打包参数
                SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
                {
                    blockOffset = 1,
                    enableRotation = true,
                    enableTightPacking = false,
                    padding = 2,
                };
                atlas.SetPackingSettings(packSetting);

                //设置打包后Texture图集信息
                SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings()
                {
                    readable = false,
                    generateMipMaps = false,
                    sRGB = true,
                    filterMode = FilterMode.Bilinear,
                };
                atlas.SetTextureSettings(textureSettings);

                //设置平台图集大小压缩等信息
                TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings()
                {
                    maxTextureSize = 4096,
                    format = TextureImporterFormat.Automatic,
                    crunchedCompression = true,
                    textureCompression = TextureImporterCompression.Compressed,
                    compressionQuality = 50,
                };
                atlas.SetPlatformSettings(platformSettings);
                int index = filePath.IndexOf("Assets");
                string atlasPath = filePath.Remove(0, index);
                AssetDatabase.CreateAsset(atlas, atlasPath);
                index = path.IndexOf("Assets");
                string spritePath = path.Remove(0, index);
                Object obj = AssetDatabase.LoadAssetAtPath(spritePath, typeof(Object));
                atlas.Add(new[] { obj });
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                int assetIndex = filePath.IndexOf("Assets");
                string guidPath = filePath.Remove(0, assetIndex);
                var guid = AssetDatabase.AssetPathToGUID(guidPath);
                var group = setting.FindGroup("Local_SpriteAtlas");
                var entry = setting.CreateOrMoveEntry(guid, group);
                var label = name + ".spriteatlas";
                if (entry.address != name)
                {
                    entry.SetAddress(name);
                    //  addAddressInfo("Local_SpriteAtlas", name);
                }
                List<string> oldLabels = new List<string>();
                foreach (var item in entry.labels)
                {
                    if (item != label)
                        oldLabels.Add(item);
                }
                for (int i = 0; i < oldLabels.Count; i++)
                {
                    entry.SetLabel(oldLabels[i], false);
                    setting.RemoveLabel(oldLabels[i]);
                }
                if (!setting.GetLabels().Contains(label))
                {
                    setting.AddLabel(label);
                }
                entry.SetLabel(label, true);
                if (!setting.GetLabels().Contains("SpriteAtlas"))
                {
                    setting.AddLabel("SpriteAtlas");
                }
                entry.SetLabel("SpriteAtlas", true);
                AssetDatabase.Refresh();
            }
        }
        else
        {
            if (dirs.Length > 0)
            {
                foreach (var info in dirs)
                {
                    addSpriteAtlas(path + "/" + info.Name, info);
                }
            }

        }
    }

    #endregion

    #endregion

    #region 资源打包


    /// <summary>
    /// 目标平台
    /// </summary>
    private BuildTarget target;

    private BuildEnvironment environment;

    /// <summary>
    /// 标记是否有打包报错信息
    /// </summary>
    private bool isBuildSuccess = true;

    private bool isChooseTarget = false;

    /// <summary>
    /// 版本信息
    /// </summary>
    private string version
    {
        get => addressableConfig.infos.Find(x => x.environment == environment).version;
        set => addressableConfig.infos.Find(x => x.environment == environment).version = value;
    }

    private bool isServer
    {
        get => addressableConfig.infos.Find(x => x.environment == environment).isServer;
        set => addressableConfig.infos.Find(x => x.environment == environment).isServer = value;
    }

    private bool isCRC
    {
        get => addressableConfig.infos.Find(x => x.environment == environment).isCRC;
        set => addressableConfig.infos.Find(x => x.environment == environment).isCRC = value;
    }

    //是否加密
    private static bool isEncrypt
    {
        get => addressableConfig.isEncrypt;
        set => addressableConfig.isEncrypt = value;
    }

    private string plat => PlatformMappingService.GetPlatformPathSubFolder();

    private string platSystem => target.ToString();

    private string AppBuildAddressableData = "BuildAddressableData";

    string LocalTag  => AddressableBuildTools.LabelTag;
    string RemotedTag => AddressableBuildTools.RemotedTag;
    string UpdateTag  => AddressableBuildTools.UpdateTag;

    string remoteLoadPathTag => AddressableAssetSettings.kRemoteLoadPath;

    string remoteBuildPathTag = AddressableAssetSettings.kRemoteBuildPath;



    string[] _vers;
    string[] vers => _vers = _vers ?? version.Split('.');
    string versionPath => vers[0] + "." + vers[1] + "/" + setting.profileSettings.GetProfileName(setting.activeProfileId);


    /// <summary>
    /// Lib打包生成的数据地址
    /// </summary>
    string creatBuildDataPath => $"{AddressableBuildTools.LibPath}/{plat}";

    /// <summary>
    /// App 打包生成的数据地址
    /// </summary>
    string appOutBuildDataPath => $"{AddressableBuildTools.AppDataPath}/{AppBuildAddressableData}/{plat}/{versionPath}";

    string remoteBuildPath => setting.profileSettings.GetValueByName(setting.activeProfileId, remoteBuildPathTag).Replace("[BuildTarget]", platSystem);

    string ContentStateDataBinPath => $"{AddressableBuildTools.AppDataPath}/{AppBuildAddressableData}/{plat}/{versionPath}/{AddressableBuildTools.AddressablesStatebin}";

    string buildAssets => $"{AddressableBuildTools.AssetPath}/{version}/{environment}";

    static string urlPath => $"Assets/AddressableAssets/Setting/AddressablePath.asset";

    string productName => PlayerSettings.productName;

    /// <summary>
    /// 远程资源地址
    /// </summary>
    public string loadurl => setting.profileSettings.GetValueByName(setting.activeProfileId, remoteLoadPathTag);

    public string buildurl => setting.profileSettings.GetValueByName(setting.activeProfileId, remoteBuildPathTag);


    AddressableAssetSettings defaultSettings => AddressableAssetSettingsDefaultObject.Settings;


    public void CopyBuildData()
    {
        if (!Directory.Exists(creatBuildDataPath)) Directory.CreateDirectory(creatBuildDataPath);

        Debug.Log($"资源复制{creatBuildDataPath}=>{appOutBuildDataPath}");

        AddressableBuildTools.CopyDirectory(creatBuildDataPath, appOutBuildDataPath, false);
    }



    /// <summary>
    /// 打包配置设置
    /// </summary>
    private void SetActiveProfileId()
    {
        var names = setting.profileSettings.GetAllProfileNames();
        if (!names.Contains(environment.ToString()))
        {
            setting.profileSettings.AddProfile(environment.ToString(), setting.activeProfileId);
        }



        var id = setting.profileSettings.GetProfileId(environment.ToString());
        if (setting.activeProfileId != id) setting.activeProfileId = id;

    }

    /// <summary>
    /// 打包参数设置
    /// </summary>
    private void SetBuildParameters()
    {
        if (UnityEditor.PlayerSettings.bundleVersion != version)
            UnityEditor.PlayerSettings.bundleVersion = version;
        if (!UnityEditor.PlayerSettings.allowUnsafeCode)
            UnityEditor.PlayerSettings.allowUnsafeCode = true;
        if (UnityEditor.PlayerSettings.stripEngineCode)
            UnityEditor.PlayerSettings.stripEngineCode = false;
    }


    /// <summary>
    /// 标记为资源分组
    /// 0 小包，所有资源存放资源服务器
    /// 1 分包 ，Local资源存本地，Remoted资源存资源服务器
    /// 2 整包，所有资源存本地
    /// </summary>
    private void GroupSchemasStatus(int status)
    {
        List<AddressableAssetGroup> deleteList = new List<AddressableAssetGroup>();
        for (int i = 0; i < setting.groups.Count; i++)
        {
            var group = setting.groups[i];
            if (!AddressableBuildTools.filterGroups.Contains(group.name))
            {
                if (group.entries.Count <= 0)
                {
                    ///删除没有资源的分组
                    deleteList.Add(group);
                }
                else
                {
                    foreach (var schema in group.Schemas)
                    {
                        if (schema is BundledAssetGroupSchema)
                        {
                            bool bundleCrc = true;
                            var bundledAssetGroupSchema = (schema as BundledAssetGroupSchema);
                            string buildPath = bundledAssetGroupSchema.BuildPath.GetName(setting);
                            string loadPath  = bundledAssetGroupSchema.LoadPath.GetName(setting);

                            if (group.name.Contains(LocalTag))
                            {
                                bundleCrc = false;
                                buildPath = status == 0 ? AddressableAssetSettings.kRemoteBuildPath : AddressableAssetSettings.kLocalBuildPath;
                                loadPath = status == 0 ? AddressableAssetSettings.kRemoteLoadPath : AddressableAssetSettings.kLocalLoadPath;
                            }
                            else if (group.name.Contains(RemotedTag))
                            {
                                bundleCrc = (!(status == 2)) == true ? isCRC : false;
                                buildPath = status == 2 ? AddressableAssetSettings.kLocalBuildPath : isServer? AddressableAssetSettings.kRemoteBuildPath : AddressableAssetSettings.kLocalBuildPath;
                                loadPath = status == 2 ? AddressableAssetSettings.kLocalLoadPath : isServer ? AddressableAssetSettings.kRemoteLoadPath : AddressableAssetSettings.kLocalLoadPath;
                            }
                            else if (group.name.Contains(UpdateTag))
                            {
                                bundleCrc = isCRC;
                                buildPath = AddressableAssetSettings.kRemoteBuildPath;
                                loadPath = AddressableAssetSettings.kRemoteLoadPath;
                            }
                       
                            bundledAssetGroupSchema.BuildPath.SetVariableByName(setting, buildPath);
                            bundledAssetGroupSchema.LoadPath.SetVariableByName(setting, loadPath);
                            bundledAssetGroupSchema.UseAssetBundleCrc = bundleCrc;
                            bundledAssetGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
                            bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;
      
                        }
                        else if (schema is ContentUpdateGroupSchema)
                        {
                            var updateGroupSchema = (schema as ContentUpdateGroupSchema);

                            if (group.name.Contains(LocalTag))
                            {
                                updateGroupSchema.StaticContent = !(status == 0);
                            }
                            else if (group.name.Contains(RemotedTag))
                            {
                                updateGroupSchema.StaticContent = (status == 2);
                            }
                            else if (group.name.Contains(UpdateTag))
                            {
                                updateGroupSchema.StaticContent = false;
                            }
                        }
                    }
                }
            }
        }
        for (int i = 0; i < deleteList.Count; i++)
        {
            setting.RemoveGroup(deleteList[i]);
        }
    }


    private void build()
    {
        BuildPipeline.BuildPlayer(GetBuildScenes(), AddressableBuildTools.OutPath, target, BuildOptions.None);
    }


    /// <summary>
    /// 获取打包场景
    /// </summary>
    /// <returns></returns>
    private string[] GetBuildScenes()
    {
        List<string> pathList = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                pathList.Add(scene.path);
            }
        }
        return pathList.ToArray();
    }



    /// <summary>
    /// build新包
    /// </summary>
    /// <param name="status"></param>
    private async void buildPackageByStatus(int status)
    {
        isBuildSuccess = true;
        AddressableBuildTools.ClearConsole();
        Application.logMessageReceived += onLogMessage;

        build();
        Application.logMessageReceived -= onLogMessage;
        AssetDatabase.Refresh();

        BuildDone(status, true);
    }



    /// <summary>
    /// 更新有修改的资源
    /// </summary>
    /// <param name="status"></param>
    private async void buildByStatus(int status)
    {
        await CopyAllAssembliesPostIl2CppStripDir();

        PrebuildCommand.GenerateAll();
        
        AddressableBuildMD5.FindOldMD5(remoteBuildPath);

        BuildContent();

        AddressableBuildMD5.FindNewMD5(remoteBuildPath);
        AddressableBuildMD5.CopyNeedFiles2BuildAssets(buildAssets);

        CopyBuildData();
        BuildDone(status,false);
    }


    public static void BuildContent()
    {
        if (isEncrypt)
        {
            var name   = nameof(AddressableAssetSettings.BuildPlayerContent);
            var method = typeof(AddressableAssetSettings).GetMethod(name, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            for (int i = 0; i < AddressableAssetSettingsDefaultObject.Settings.DataBuilders.Count; i++)
            {
                var item = AddressableAssetSettingsDefaultObject.Settings.DataBuilders[i];

                if (item.GetType().Name.Contains("BuildScriptPackedModeAES"))
                {
                    AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilderIndex = i;

                    var input = new AddressablesDataBuilderInput(AddressableAssetSettingsDefaultObject.Settings);

                    method.Invoke(null, new object[] { null, input });
                }
            }
        }
        else
        {
            AddressableAssetSettings.BuildPlayerContent();
        }
    }



    private void BuildDone(int status, bool buildApp)
    {
        if (isBuildSuccess)
        {
            string showMessage = string.Empty;
            if (status == 0)
            {
                showMessage = buildApp ? "打包小包成功" : "打包小包资源完成";
            }
            else if (status == 1)
            {
                showMessage = buildApp ? "打包分包成功" : "打包分包资源完成";
            }
            else if (status == 2)
            {
                showMessage = buildApp ? "打包整包成功" : "打包整包资源完成";
            }
            if (EditorUtility.DisplayDialog(buildApp ? "打包完成" : "打包资源", showMessage, "确定"))
            {
                if (buildApp)
                {
                    EditorUtility.RevealInFinder(AddressableBuildTools.OutPath);
                    AddressableBuildTools.OutPath = string.Empty;
                }
            }
        }
        else
        {
            if (EditorUtility.DisplayDialog("打包失败", "请检测报错信息", "确定"))
            {
                EditorUtility.RevealInFinder(AddressableBuildTools.OutPath);
                AddressableBuildTools.OutPath = string.Empty;
            }
        }
    }


    /// <summary>
    /// 更新版本号，需要强制更新
    /// </summary>
    private void UpdateBuildVersion()
    {
        string[] ver = version.Split('.');
        version = ver[0] + "." + (int.Parse(ver[1]) + 1) + ".0";
        addressableConfig.Save();
    }

    /// <summary>
    /// 更新资源号，不用强制更新安装包
    /// </summary>
    private void UpdateAssetVersion()
    {
        string[] ver = version.Split('.');
        version = ver[0] + "." + ver[1] + "." + (int.Parse(ver[2]) + 1);

        addressableConfig.Save();
    }

    

    List<AddressableBuildGUI> buildGUIs = new List<AddressableBuildGUI>()
    {
          new AddressableBuildGUI("整包",BuildStatus.LocalGroup),
          new AddressableBuildGUI("分包",BuildStatus.AnyGroup),
          new AddressableBuildGUI("小包",BuildStatus.RemotedGroup),
    };

    string[] buildGuiNames = new string[]
        {
         "整包",
         "分包",
         "小包"
        };

    int selectID = 1;

    private void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Space(10);

        var labelsize = GUI.skin.label.fontSize;
        var labelalignment = GUI.skin.label.alignment;
        GUI.skin.label.fontSize = 24;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("打包窗口");
        if (!isChooseTarget)
        {
            target = EditorUserBuildSettings.activeBuildTarget;
            isChooseTarget = true;
        }


        var oldenvironment = environment;
        environment = (BuildEnvironment)EditorGUILayout.EnumPopup("输出包环境(Environment)", environment);
        SetActiveProfileId();
        if (oldenvironment != environment)
        {
            PlayerPrefs.SetString(oldenvironment.ToString() + "_BuildUrl", loadurl);

            version = addressableConfig.infos.Find(x => x.environment == environment).version;
            oldenvironment = environment;
        }




        ShowField(() =>
        {
            EditorGUILayout.LabelField("资源是否加密", GUILayout.Width(150), GUILayout.Height(20));
            isEncrypt = EditorGUILayout.Toggle(isEncrypt, GUILayout.MinWidth(320));
        });


        EditorGUICustom.BoxColor(Color.black, (_) =>
        {
            GUILayout.Label("当前打包环境:" + environment);

            target = (BuildTarget)EditorGUILayout.EnumPopup("打包平台(Platform)", target);
          
       

            SetBuildParameters();
            GUILayout.Space(10);

            GUI.skin.label.fontSize = labelsize;
            GUI.skin.label.alignment = labelalignment;

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("打包版本(Version)", GUILayout.Width(145), GUILayout.Height(20));
            version = EditorGUILayout.TextArea(version, GUILayout.Width(50), GUILayout.Height(20));
            GUILayout.Space(50);

                if (GUILayout.Button("升级版本号"))
                {
                    UpdateBuildVersion();
    
                }

                if (GUILayout.Button("升级资源版本号"))
                {
                    UpdateAssetVersion();
           
                }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);


            GUILayout.Space(10);


            ShowField(() =>
            {
                EditorGUILayout.LabelField("是否开启远程服务器(Remote))", GUILayout.Width(240), GUILayout.Height(20));
                isServer = EditorGUILayout.Toggle(isServer, GUILayout.MinWidth(320));
            });

            if (isServer)
            {
                ShowField(() =>
                {
                    EditorGUILayout.LabelField("远程服务器资源是否开启CRC对比", GUILayout.Width(240), GUILayout.Height(20));
                    isCRC = EditorGUILayout.Toggle(isCRC, GUILayout.MinWidth(240), GUILayout.Height(20));
                });

                ShowField(() =>
                {
                    EditorGUILayout.LabelField("远程服务器构建地址(RemoteBuildPath)", GUILayout.Width(240), GUILayout.Height(20));
                    GUI.enabled = false;
                    EditorGUILayout.TextArea(buildurl, GUILayout.MinWidth(240), GUILayout.Height(20));
                    GUI.enabled = true;
                });

                ShowField(() =>
                {
                    EditorGUILayout.LabelField("远程服务器加载地址(RemoteLoadPath)", GUILayout.Width(240), GUILayout.Height(20));
                    GUI.enabled = false;
                    EditorGUILayout.TextArea(loadurl, GUILayout.MinWidth(240), GUILayout.Height(20));
                    GUI.enabled = true;
                });
            }


            GUILayout.Space(10);

        });

        Repaint();

        var helpRect = EditorGUILayout.GetControlRect(GUILayout.Height(20));
        SetBuildGUI(helpRect);

        var boxRect  = EditorGUILayout.GetControlRect(GUILayout.Width(120));
        CheckAssetUpdateOrAppendGUI(boxRect);


        GUILayout.Space(10);
        if (GUILayout.Button("直接出包"))
        {
            isBuildSuccess = true;
            AddressableBuildTools.ClearConsole();
            Application.logMessageReceived += onLogMessage;
            AssetDatabase.Refresh();
            build();
            AssetDatabase.Refresh();
            if (isBuildSuccess)
            {
                if (EditorUtility.DisplayDialog("一键打包完成", "一键打包完成", "确定"))
                {
                    EditorUtility.RevealInFinder(AddressableBuildTools.OutPath);
                    AddressableBuildTools.OutPath = string.Empty;
                }
            }
            else
            {
                if (EditorUtility.DisplayDialog("打包失败", "请检测报错信息", "确定"))
                {
                    EditorUtility.RevealInFinder(AddressableBuildTools.OutPath);
                    AddressableBuildTools.OutPath = string.Empty;
                }
            }
            Application.logMessageReceived -= onLogMessage;
        }



        GUILayout.Space(10);
        GUILayout.EndVertical();


        void ShowField(UnityAction action)
        {
            GUILayout.BeginHorizontal();
            action?.Invoke();
            GUILayout.EndHorizontal();
        }

    }




    private void SetBuildGUI(Rect rect)
    {
        var title = "打包设置\n"
          + "    [整包=> 所有的资源 Group 设置为 Local, 资源打包在Streaming中,等待加载] \n"
          + "    [分包=> 资源 Group 为 Local 和 Remoed, 分别打包在本地和远程打包地址,等待加载] \n"
          + "    [小包=> 所有的资源 Group 设置为 Remoed, 资源打包在远程打包地址中,等待加载] ";
        EditorGUILayout.HelpBox(title, MessageType.Info,true);
     
        selectID  = GUILayout.Toolbar( selectID, buildGuiNames);
     
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < buildGUIs.Count; i++)
        {
            var gui = buildGUIs[i];
            EditorGUI.BeginDisabledGroup(i != selectID);
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button( gui.setName))
            {
                GroupSchemasStatus((int)gui.status);
                EditorUtility.DisplayDialog(gui.buildDisplay[0], gui.buildDisplay[1], gui.buildDisplay[2]);
            }

            if (GUILayout.Button(gui.buildName))
            {
                buildByStatus((int)gui.status);
            }

            if (GUILayout.Button(gui.buildAppName))
            {
                buildPackageByStatus((int)gui.status);
            }

            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void CheckAssetUpdateOrAppendGUI(Rect boxRect)
    {
        boxRect.height = 18;


        string boxTxt = "资源【更新 丶增量打包】 \n "
                      + "--------------------------------------------------------------------------------\n"
                      + "       [当前平台的资源]  =>\n "
                      + "--------------------------------------------------------------------------------\n"
                      + "       [选择平台的资源]  =>\n "
                      + "--------------------------------------------------------------------------------\n";

        var startRt = EditorGUILayout.GetControlRect(GUILayout.Height(1));
        EditorGUILayout.HelpBox(boxTxt, MessageType.Info);
        var endRt   = EditorGUILayout.GetControlRect(GUILayout.Height(1));

        var offect        = (endRt.y - startRt.y) / 2;
        var mHeight       = offect + boxRect.y;
        var autoRect      = new Rect(boxRect.x + 200, mHeight , boxRect.width, boxRect.height - 1);
        var autoRectApp   = new Rect(autoRect.x + boxRect.width, mHeight, boxRect.width, boxRect.height - 1);
        var selectRect    = new Rect(boxRect.x + 200, mHeight+ offect /2, boxRect.width, boxRect.height - 1);
        var selectRectApp = new Rect(selectRect.x + boxRect.width, mHeight + offect / 2, boxRect.width, boxRect.height - 1);

        if (GUI.Button(selectRectApp, "手动增量打包"))
        {
            string buildPath = ContentUpdateScript.GetContentStateDataPath(true);
            BuildAppendAssets(buildPath);
        }
        if (GUI.Button(autoRectApp, "自动增量打包"))
        {
            BuildAppendAssets(ContentStateDataBinPath);
        }

        if (GUI.Button(selectRect, "手动更新"))
        {
            string buildPath = ContentUpdateScript.GetContentStateDataPath(true);
            CheckContentUpdateAssets(buildPath);
        }
        if (GUI.Button(autoRect, "自动更新"))
        {
            CheckContentUpdateAssets(ContentStateDataBinPath);
        }
    }

    private void BuildAppendAssetsGUI(Rect boxRect)
    {
        string boxTxt = "资源增量更新 \n "
                      + "---------------------------------------------------------\n"
                      + "       [当前平台资源增量更新]=>\n "
                      + "---------------------------------------------------------\n"
                      + "       [选择平台资源增量更新]=>\n "
                      + "---------------------------------------------------------\n";

        EditorGUILayout.HelpBox(boxTxt, MessageType.Info);

        var endRt = EditorGUILayout.GetControlRect(GUILayout.Height(1));

        var mHeight = (endRt.y - boxRect.y) / 2 + boxRect.y;
        var autoRect = new Rect(boxRect.x + 200, mHeight - 10, boxRect.width, boxRect.height - 1);
        var selectRect = new Rect(boxRect.x + 200, mHeight + 14, boxRect.width, boxRect.height - 1);

        if (GUI.Button(selectRect, "手动增量打包资源"))
        {
            string buildPath = ContentUpdateScript.GetContentStateDataPath(true);
            BuildAppendAssets(buildPath);
        }
        if (GUI.Button(autoRect,"自动增量打包资源"))
        {
            BuildAppendAssets(ContentStateDataBinPath);
        }


    }



    private void CheckContentUpdateAssets(string buildPath)
    {
        //对比addressable缓存，获取更改资源的列表
        Debug.Log("buildPath = " + buildPath);
        List<AddressableAssetEntry> entrys = ContentUpdateScript.GatherModifiedEntries(defaultSettings, buildPath);
        if (entrys.Count == 0) return;

        //打印已经更改的资源名字
        StringBuilder sbuider = new StringBuilder();
        sbuider.AppendLine("---------Need Update Assets----------");
        foreach (var _ in entrys)
        {
            sbuider.AppendLine(_.address);
        }
        sbuider.AppendLine("-------------------------------------");
        Debug.Log(sbuider.ToString());


        //将被修改过的资源单独分组
        UpdateAssetVersion();
        var groupName =$"{AddressableBuildTools.UpdateTag}{version}";
        ContentUpdateScript.CreateContentUpdateGroup(defaultSettings, entrys, groupName);
        SetGroupSchemas(groupName);
        AssetDatabase.Refresh();
    }

    private void BuildAppendAssets(string path)
    {
        isBuildSuccess = true;
        AddressableBuildTools.ClearConsole();
        AssetDatabase.Refresh();
        Application.logMessageReceived += onLogMessage;


        AddressableBuildMD5.FindOldMD5(remoteBuildPath);
        AssetDatabase.Refresh();
   
        AddressablesPlayerBuildResult result = ContentUpdateScript.BuildContentUpdate(defaultSettings, path);
        AssetDatabase.Refresh();

        CopyBuildData();
        AssetDatabase.Refresh();

        AddressableBuildMD5.FindNewMD5(remoteBuildPath);
        AddressableBuildMD5.CopyNeedFiles2BuildAssets(buildAssets);
        AssetDatabase.Refresh();

        Debug.Log(path);
        Debug.Log("BuildFinish path = " + defaultSettings.RemoteCatalogBuildPath.GetValue(defaultSettings));


        Application.logMessageReceived -= onLogMessage;

        if (isBuildSuccess)
            EditorUtility.DisplayDialog("增量打包", "增量打包成功", "确定");
        else
            EditorUtility.DisplayDialog("增量打包资源失败", "请检查报错信息", "确定");
    }

    private static void SetGroupSchemas(string groupName)
    {
        var updateGroup = setting.FindGroup(groupName);
        if (updateGroup != null)
        {
            foreach (var schema in updateGroup.Schemas)
            {
                if (schema is BundledAssetGroupSchema)
                {
                    var bundledAssetGroupSchema = (schema as BundledAssetGroupSchema);
                    bundledAssetGroupSchema.LoadPath.SetVariableByName(updateGroup.Settings, AddressableAssetSettings.kRemoteLoadPath);
                    bundledAssetGroupSchema.BuildPath.SetVariableByName(updateGroup.Settings, AddressableAssetSettings.kRemoteBuildPath);
                   
                    bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;
                    bundledAssetGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
                }
            }
        }
    }

    private void onLogMessage(string condition, string StackTrace, LogType type)
    {

        if (type == LogType.Error)
        {
            if (condition != "EndLayoutGroup: BeginLayoutGroup must be called first.")
                isBuildSuccess = false;
        }
    }
    #endregion

    [DidReloadScripts]
    public static void CopyDll()
    {
        if(addressableConfig.IsScriptRebuildCopyDll)
        CopyAllAssembliesPostIl2CppStripDir();
    }

 
    public async static Task CopyAllAssembliesPostIl2CppStripDir()
    {
        CompileDllCommand.CompileDllActiveBuildTarget();

        var outputDir = $"{AddressableBuildTools.ShortAddressableAssetsPath}/Remoted/{AddressableBuildTools.GroupTag}aotdlls/{AddressableBuildTools.LabelTag}AotDll";
        var outputHotDir = $"{AddressableBuildTools.ShortAddressableAssetsPath}/Remoted/{AddressableBuildTools.GroupTag}hotfixs/{AddressableBuildTools.LabelTag}HotDll";
        var source = $"{SettingsUtil.AssembliesPostIl2CppStripDir}/{EditorUserBuildSettings.activeBuildTarget}";
        var assembly = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget);


        Debug.Log(SettingsUtil.HotUpdateDllsRootOutputDir);
        Debug.Log(SettingsUtil.GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget));

        Debug.Log(outputDir);
        Debug.Log(outputHotDir);

        if (Directory.Exists(outputDir) == false)
            Directory.CreateDirectory(outputDir);

        if (Directory.Exists(outputHotDir) == false)
            Directory.CreateDirectory(outputHotDir);


        foreach (var srcFile in Directory.GetFiles(source))
        {
            var fileName = Path.GetFileName(srcFile);
            if (fileName.EndsWith(".dll"))
            {
                string dstFile = $"{outputDir}/{Path.GetFileName(srcFile)}.bytes";
                File.Copy(srcFile, dstFile, true);
            }
        }

        List<string> hotNames = new List<string>(SettingsUtil.HybridCLRSettings.hotUpdateAssemblies);
        hotNames.AddRange(SettingsUtil.HybridCLRSettings.hotUpdateAssemblyDefinitions.Select(_ => _.name));

        foreach (var srcFile in Directory.GetFiles(assembly))
        {
            var fileName = Path.GetFileNameWithoutExtension(srcFile);
            if (hotNames.Contains(fileName))
            {
                string dstFile = $"{outputHotDir}/{Path.GetFileName(srcFile)}.bytes";
                File.Copy(srcFile, dstFile, true);
            }
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        await Task.Delay(1000);
    }
}
