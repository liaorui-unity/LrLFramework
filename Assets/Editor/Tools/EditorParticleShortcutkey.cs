
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：特效快捷键
// 创建时间：2020-10-28 13:41:51
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Table
{
    public class EditorParticleShortcutkey : MonoBehaviour
    {
        [MenuItem("Tools/Shortcutkey %~", true, 1050)]
        static private bool Change_ParticleRandomSeed()
        {
            Object[] select_objs = Selection.objects;
            if (select_objs == null || select_objs.Length != 1)
                return false;

            Object o = select_objs[0];
            GameObject obj = o as GameObject;
            if (obj == null) return false;
            return obj.GetComponent<ParticleSystem>() != null;
        }

        [MenuItem("Tools/Shortcutkey %`", false, 1050)]
        static void ParticleRandomSeed()
        {
            Object[] select_objs = Selection.objects;
            if (select_objs == null || select_objs.Length != 1)
                return;

            Object o = select_objs[0];
            GameObject obj = o as GameObject;
            if (obj == null) return;
            ParticleSystem ps = obj.GetComponent<ParticleSystem>();
            if (ps == null) return;
            
            uint seed = (uint)(MathUtils.RandRange(0, 1) * uint.MaxValue);
            ps.Pause();
            ps.useAutoRandomSeed = false;
            ps.randomSeed = seed;
            ps.Play();
        }
    }
}
