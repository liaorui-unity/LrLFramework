using Sailfish;
using System.IO;
using UnityEngine;

public class JsonMono : MonoBehaviour, ISerializationCallbackReceiver
{
    public const string pattern = @"^[0-9.-]+$";

    [System.Serializable]
    public class JsonSetting
    {
        public bool   isDefault = true;
        public string saveName;

        internal bool   checkDefault;
        internal string checkName;

        public bool IsChange()
        {
            if (checkDefault != isDefault)
            {
                checkDefault = isDefault;
                return true;
            }

            if (checkName  != saveName)
            {
                checkName   = saveName;
                return true;
            }

            return false;
        }
    }

    const  string m_folder   =  "CustomJson";
    string        m_jsonPath => $"{Application . streamingAssetsPath}/{m_folder}/{saveName}.json";
    bool          m_isHas    => File           . Exists(m_jsonPath);

    public JsonSetting jsonSetting = new JsonSetting();

    public virtual string saveName
    {
        get => jsonSetting.isDefault ? GetType().ToString() : jsonSetting.saveName;
    }


    public void LoadJsonFile()
    {
        if (!m_isHas)
        {
            SaveJsonFile();
        }
        else
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(m_jsonPath), this);
        }
    }

 
    public void SaveJsonFile()
    {
        if (!m_isHas)
        {
            var directory = Path.GetDirectoryName(m_jsonPath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        }
        File.WriteAllText(m_jsonPath, JsonUtility.ToJson(this));

#if UNITY_EDITOR
        try
        {
            if (!Application.isPlaying) UnityEditor.EditorUtility.SetDirty(this.gameObject);
        }
        catch { }
#endif
    }


    bool m_isSerialize = false;
    private void Awake()
    {
        if (m_isSerialize)
        {
            m_isSerialize = true;
            LoadJsonFile();
            JsonGUIContent.AddJsonMono(this);
        }
    }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        if (m_isSerialize    == false)
        {
            m_isSerialize    =  true;
            LoadJsonFile();
            JsonGUIContent . AddJsonMono(this);

            if (string.IsNullOrEmpty(jsonSetting.saveName))
                jsonSetting.saveName = GetType().ToString();
        }
    }

    public void OnDestroy()
    {
        if (m_isSerialize)
        {
            SaveJsonFile();
            JsonGUIContent.RemoveJsonMono(this);
        }
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (jsonSetting.IsChange())
        {
            if (jsonSetting.isDefault)
            { 
                jsonSetting.saveName = GetType().ToString();
            }
            return;
        }

        if (m_isSerialize) SaveJsonFile();
    }
#endif
}