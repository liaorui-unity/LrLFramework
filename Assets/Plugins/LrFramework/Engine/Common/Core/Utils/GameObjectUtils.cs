using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

#pragma warning disable 0618

namespace Table
{
    /// <summary>
    /// GameObject相关
    /// @author hannibal
    /// @time 2014-11-17
    /// </summary>
    public static class GameObjectUtils
    {
        #region 对象
        /// <summary>
        /// 初始化方位信息
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="is_world"></param>
        public static void InitTransform(this Transform obj, bool is_world = false)
        {
            if (obj == null) return;
            if (!is_world)
            {
                obj.localPosition = Vector3.zero;
                obj.localEulerAngles = Vector3.zero;
            }
            else
            {
                obj.position = Vector3.zero;
                obj.eulerAngles = Vector3.zero;
            }
            obj.localScale = Vector3.one;
        }
        public static void InitTransform(this GameObject obj, bool is_world = false)
        {
            if (obj == null) return;
            obj.transform.InitTransform(is_world);
        }
        /// <summary>
        /// 设置对象层级
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        public static void SetLayer(this GameObject go, int layer)
        {
            if (go == null) return;
            if (layer <= 31)
            {
                go.layer = layer;
                Transform t = go.transform;
                for (int i = 0; i < t.childCount; ++i)
                {
                    SetLayer(t.GetChild(i).gameObject, layer);
                }
            }
        }
        /// <summary>
        /// 设置对象及其孩子MeshRender渲染层级
        /// </summary>
        /// <param name="go"></param>
        /// <param name="sortingOrder"></param>
        public static void SetSortingOrder(this GameObject go, int sortingOrder)
        {
            if (go == null) return;
            Renderer[] array = go.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < array.Length; ++i)
            {
                array[i].sortingOrder = sortingOrder;
            }
        }
        /// <summary>
        /// 设置层级，比order更高一级
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        public static void SetSortingLayer(this GameObject go, string layer)
        {
            if (go == null) return;
            Renderer[] array = go.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < array.Length; ++i)
            {
                array[i].sortingLayerName = layer;
            }
        }

        /// <summary>
        /// 设置节点名
        /// </summary>
        /// <param name="go"></param>
        /// <param name="name"></param>
        /// <param name="recursion">是否影响子节点</param>
        public static void SetName(this GameObject go, string name, bool recursion)
        {
            if (go == null) return;
            go.name = name;
            if (recursion)
            {
                Transform t = go.transform;
                for (int i = 0; i < t.childCount; ++i)
                {
                    SetName(t.GetChild(i).gameObject, name, recursion);
                }
            }
        }

        /// <summary>
        /// 判断节点是否激活
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsEnable(this GameObject obj)
        {
            if (!obj) return false;
            if (!obj.activeSelf) return false;
            if (obj.transform.parent != null)
                return IsEnable(obj.transform.parent.gameObject);
            return true;
        }
        /// <summary>
        /// 获取或添加组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetOrCreateComponent<T>(this GameObject obj) where T : Component
        {
            if (obj == null) return default(T);
            T com = obj.GetComponent<T>();
            if (com == null) com = (T)obj.AddComponent(typeof(T));
            return com;
        }
        public static T GetOrCreateComponent<T>(this Transform node) where T : Component
        {
            if (node == null) return default(T);
            T com = node.GetComponent<T>();
            if (com == null) com = (T)node.gameObject.AddComponent(typeof(T));
            return com;
        }
        public static void DestroyComponent<T>(this GameObject obj) where T : Component
        {
            if (obj == null) return;
            T com = obj.GetComponentInChildren<T>();
            if (com != null) GameObject.Destroy(com);
        }
        public static void DestroyComponent<T>(this Transform node) where T : Component
        {
            if (node == null) return;
            T com = node.GetComponentInChildren<T>();
            if (com != null) GameObject.Destroy(com);
        }
        #endregion

        #region 节点
        private static List<GameObject> child_list = new List<GameObject>();
        /// <summary>
        /// 根据名称获得对象或子对象
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Transform FindChild(Transform node, string name)
        {
            return node.GetChild(name);
        }
        public static Transform GetChild(this Transform node, string name)
        {
            if (node == null) return null;
            GameObject obj = node.gameObject.GetChild(name);
            return obj == null ? null : obj.transform;
        }
        public static GameObject FindChild(GameObject obj, string name)
        {
            return obj.GetChild(name);
        }
        public static GameObject GetChild(this GameObject obj, string name)
        {
            if (obj == null)
            {
                return null;
            }
            child_list.Clear();
            child_list.Add(obj);
            GameObject result = null;
            int i = 0;
            while (i < child_list.Count)
            {
                GameObject gameObject = child_list[i++];
                if (gameObject.name == name)
                {
                    result = gameObject;
                    break;
                }
                Transform t = gameObject.transform;
                int childCount = t.childCount;
                for (int j = 0; j < childCount; j++)
                {
                    GameObject gameObject2 = t.GetChild(j).gameObject;
                    child_list.Add(gameObject2);
                }
            }
            child_list.Clear();
            return result;
        }

        /// <summary>
        /// 删除子节点
        /// </summary>
        /// <param name="node">父节点</param>
        /// <param name="recursion">是否递归</param>
        public static void RemoveAllChild(this Transform node, bool recursion)
        {
            if (node == null) return;
            while (node.childCount > 0)
            {
                Transform child = node.GetChild(0);
                if (recursion) RemoveAllChild(child, true);
                GameObject.DestroyImmediate(child.gameObject);
            }
        }

        /// <summary>
        /// 设置子节点active
        /// </summary>
        /// <param name="node">父节点</param>
        /// <param name="active"></param>
        /// <param name="recursion">是否递归</param>
        public static void SetActiveAllChild(this Transform node, bool active, bool recursion)
        {
            if (node == null) return;
            for (int i = 0; i < node.childCount; i++)
            {
                Transform t = node.GetChild(i);
                if (recursion) SetActiveAllChild(t, active, recursion);
                t.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// 判断是否有父节点
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsParent(this Transform node, string Name)
        {
            if (node == null) return false;
            if (node.parent != null)
            {
                if (node.parent.name == Name)
                    return true;
                else
                    IsParent(node.parent,Name);
            }
            return false;
        }
        /// <summary>
        /// 父节点
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="node"></param>
        /// <param name="b"></param>
        public static void SetParent(this GameObject obj, Transform node, bool b = false)
        {
            if (obj == null) return;
            obj.transform.SetParent(node, b);
        }

        /// <summary>
        /// 遍历子节点，执行函数
        /// </summary>
        /// <param name="obj">根节点</param>
        /// <param name="callback">需要执行的函数</param>
        public delegate bool FindChildDelegate(GameObject obj);
        public static GameObject ForeachChild(this GameObject obj, FindChildDelegate callback)
        {
            if (obj == null || callback == null)
            {
                return null;
            }
            child_list.Add(obj);
            GameObject result = null;
            int i = 0;
            while (i < child_list.Count)
            {
                GameObject gameObject = child_list[i++];
                Transform t = gameObject.transform;
                if (callback(gameObject))
                {
                    result = gameObject;
                    break;
                }
                int childCount = t.childCount;
                for (int j = 0; j < childCount; j++)
                {
                    GameObject gameObject2 = t.GetChild(j).gameObject;
                    child_list.Add(gameObject2);
                }
            }
            child_list.Clear();
            return result;
        }
        #endregion

        #region sprite
        public static Rect GetSpriteRect(SpriteRenderer sprite)
        {
            return new Rect(sprite.bounds.min, sprite.bounds.max);
        }
        public static Vector2 GetSpriteSize(SpriteRenderer sprite)
        {
            return sprite.size;
        }
        public static Rect GetSpriteRect(Sprite sprite)
        {
            return new Rect(sprite.bounds.min, sprite.bounds.max);
        }
        public static Rect GetSpritePixelRect(Sprite sprite)
        {
            return sprite.rect;
        }
        public static Vector2 GetSpriteSize(Sprite sprite)
        {
            return sprite.bounds.size;
        }
        #endregion

        #region 粒子
        /// <summary>
        /// 粒子总时长
        /// </summary>
        public static float GetParticleSystemTime(GameObject obj)
        {
            ParticleSystem[] particleSystems = obj.GetComponentsInChildren<ParticleSystem>();
            float maxDuration = 0;
            foreach (ParticleSystem ps in particleSystems)
            {
                if (ps.emission.enabled)
                {
                    if (ps.main.loop)
                    {
                        return float.MaxValue;
                    }
                    float dunration = 0f;
                    if (ps.emissionRate <= 0)
                    {
                        dunration = ps.startDelay + ps.startLifetime;
                    }
                    else
                    {
                        dunration = ps.startDelay + Mathf.Max(ps.duration, ps.startLifetime);
                    }
                    if (dunration > maxDuration)
                    {
                        maxDuration = dunration;
                    }
                }
            }
            return maxDuration;
        }
        /// <summary>
        /// 粒子缩放
        /// </summary>
        public static void SetParticleScale(GameObject go, float scale)
        {
            if (go == null) return;
            ParticleSystem[] particles = go.GetComponentsInChildren<ParticleSystem>();
            if (particles == null || particles.Length == 0) return;
            foreach (ParticleSystem particle in particles)
            {
                particle.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }
            go.transform.localScale = Vector3.one * scale;
        }
        public static void SetParticleScale(GameObject go, Vector3 scale)
        {
            if (go == null) return;
            ParticleSystem[] particles = go.GetComponentsInChildren<ParticleSystem>();
            if (particles == null || particles.Length == 0) return;
            foreach (ParticleSystem particle in particles)
            {
                particle.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }
            go.transform.localScale = scale;
        }
        /// <summary>
        /// speed
        /// </summary>
        public static void SetParticleSpeed(GameObject go, float speed)
        {
            if (go == null) return;
            ParticleSystem[] particles = go.GetComponentsInChildren<ParticleSystem>();
            if (particles == null || particles.Length == 0) return;
            for (int i = 0; i < particles.Length; ++i)
            {
                particles[i].playbackSpeed = (speed);
            }
        }
        /// <summary>
        /// 清除拖尾
        /// </summary>
        public static void ClearParticleTrail(GameObject go)
        {
            if (go == null) return;
            TrailRenderer[] trails = go.GetComponentsInChildren<TrailRenderer>();
            if (trails == null || trails.Length == 0) return;
            for (int i = 0; i < trails.Length; ++i)
            {
                trails[i].Clear();
            }
        }
        /// <summary>
        /// 获取特效最大的sortingOrder
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static int GetParticleMaxSortingOrder(GameObject go)
        {
            if (go == null) return 0;
            int        order     = 0;
            GameObject effectObj = go.GetChild("VFX").gameObject;
            Renderer[] renders   = effectObj.GetComponentsInChildren<Renderer>();
            foreach (Renderer render in renders)
            {
                if (order < render.sortingOrder)
                {
                    order = render.sortingOrder;
                }
            }

            return order;
        }
        #endregion

        #region 动画
        /// <summary>
        /// 获取动画时间
        /// 1.计算animator当前动画
        /// </summary>
        public static float GetAnimTime(GameObject obj)
        {
            float total_time = 0;
            do
            {///1.animator
                Animator anim = obj.GetComponent<Animator>();
                if (anim == null || anim.runtimeAnimatorController == null) break;
                float time = anim.GetCurrentAnimatorStateInfo(0).length;
                total_time = time > total_time ? time : total_time;
            } while (false);
            if(total_time == 0)
            {///2.animation
                Animation anim = obj.GetComponent<Animation>();
                if (anim != null)
                {
                    foreach (AnimationState state in anim)
                    {
                        if (anim.IsPlaying(state.name) && state.length > total_time)
                            total_time = state.length;
                    }
                }
            }
            return total_time;
        }
        public static float GetAnimTime(GameObject obj, string pose_name)
        {
            float total_time = 0;
            do
            {///1.animator
                Animator anim = obj.GetComponent<Animator>();
                if (anim == null || anim.runtimeAnimatorController == null) break;

                AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
                for (int j = 0; j < clips.Length; ++j)
                {
                    AnimationClip clip = clips[j];
                    if (clip.name.Equals(pose_name, System.StringComparison.OrdinalIgnoreCase))
                    {
                        total_time = clip.length;
                        break;
                    }
                }

            } while (false);
            if (total_time == 0)
            {///2.animation
                Animation anim = obj.GetComponent<Animation>();
                if (anim != null)
                {
                    foreach (AnimationState state in anim)
                    {
                        if (anim.name.Equals(pose_name, System.StringComparison.OrdinalIgnoreCase))
                        {
                            total_time = state.length;
                            break;
                        }
                    }
                }
            }
            return total_time;
        }
        /// <summary>
        /// 获取动画总时间
        /// 1.计算animator当前动画
        /// </summary>
        public static float GetTotalAnimTime(GameObject obj)
        {
            float total_time = 0;
            {///1.animator
                Animator[] anims = obj.GetComponentsInChildren<Animator>();
                for (int i = 0; i < anims.Length; ++i)
                {
                    Animator anim = anims[i];
                    if (anim == null || anim.runtimeAnimatorController == null) continue;
                    float time = anim.GetCurrentAnimatorStateInfo(0).length;
                    total_time = time > total_time ? time : total_time;
                }
            }
            {///2.animation
                Animation[] anims = obj.GetComponentsInChildren<Animation>();
                for (int i = 0; i < anims.Length; ++i)
                {
                    Animation anim = anims[i];
                    if (anim == null) continue;
                    foreach (AnimationState state in anim)
                    {
                        if (anim.IsPlaying(state.name) && state.length > total_time)
                            total_time = state.length;
                    }
                }
            }
            return total_time;
        }
        #endregion

        #region 渲染
        /// <summary>
        /// ui后与特效渲染
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="effect"></param>
        public static void SetUIAboveEffect(GameObject ui, GameObject effect, bool abilityInteractive = true, int offsetValue = 1)
        {
            if (ui == null || effect == null) return;
            Canvas canvas = GetOrCreateComponent<Canvas>(ui);
            if (abilityInteractive)
            {
                GetOrCreateComponent<GraphicRaycaster>(ui);
            }
            canvas.overrideSorting = true;

            int order = GetParticleMaxSortingOrder(effect);
            canvas.sortingOrder = order + offsetValue;
        }
        #endregion
    }
}