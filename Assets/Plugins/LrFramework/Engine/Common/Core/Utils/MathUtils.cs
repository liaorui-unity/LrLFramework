using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LogInfo;

namespace Table
{
    /// <summary>
    /// 数学
    /// @author hannibal
    /// @time 2014-11-1
    /// </summary>
    public class MathUtils
    {
        /**字节转换M*/
        static public float BYTE_TO_M = 1.0f / (1024 * 1024);
        /**字节转换K*/
        static public float BYTE_TO_K = 1.0f / (1024);

        static public Vector3 MAX_VECTOR3 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        static public Vector3 MIN_VECTOR3 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        static public Vector3 TMP_VECTOR3 = new Vector3(0f, 0f, 0f);
        static public Vector2 TMP_VECTOR2 = new Vector2(0f, 0f);

        public static float Min(float first, float second)
        {
            return (first < second ? first : second);
        }
        public static float Max(float first, float second)
        {
            return (first > second ? first : second);
        }
        /// <summary>
        /// 获得一个数的符号(-1,0,1)
        /// </summary>
        /// <returns>大于0：1，小于0：-1，等于0:0</returns>
        public static int Sign(float value)
        {
            return (value < 0 ? -1 : (value > 0 ? 1 : 0));
        }
        /// <summary>
        /// 产生随机数
        /// </summary>
        /// <returns>x>=param1 && x<param2</returns>
        public static float RandRange(float param1, float param2)
        {
            float loc = Random.Range(param1, param2);
            return loc;
        }
        /// <summary>
        /// 产生随机数
        /// </summary>
        /// <returns>x>=param1 && x<param2</returns>
        public static int RandRange_Int(int param1, int param2)
        {
            int loc = param1 + (int)(Random.Range(0f, 1f) * (param2 - param1));
            loc = Mathf.Clamp(loc, param1, param2 - 1);
            return loc;
        }
        /// <summary>
        /// 从数组中产生随机数[-1,1,2]
        /// </summary>
        /// <returns>-1/1/2中的一个</returns>
        public static T RandRange_Array<T>(T[] arr)
        {
            if (arr.Length == 0)
            {
                Info.LogError("容器数量为空");
                return default(T);
            }
            T loc = arr[RandRange_Int(0, arr.Length)];
            return loc;
        }
        /// <summary>
        /// 从数组中产生随机数[-1,1,2]
        /// </summary>
        /// <returns>-1/1/2中的一个</returns>
        public static T RandRange_List<T>(List<T> arr)
        {
            if (arr.Count == 0)
            {
                Info.LogError("错误的参数");
                return default(T);
            }
            T loc = arr[RandRange_Int(0, arr.Count)];
            return loc;
        }
        /// <summary>
        /// 随机1/-1
        /// </summary>
        /// <returns>1/-1</returns>
        public static int Rand_Sign()
        {
            int[] arr = new int[2] { -1, 1 };
            int loc = RandRange_Array<int>(arr);
            return loc;
        }
        /// <summary>
        /// 产生-factor到factor的随机向量
        /// </summary>
        /// <param name="factor">长度</param>
        /// <returns></returns>
        public static Vector3 Rand_Vector3(float length)
        {
            float x = (1 - 2 * Random.value) * length;
            float y = (1 - 2 * Random.value) * length;
            float z = (1 - 2 * Random.value) * length;
            return new Vector3(x, y, z);
        }
        /// <summary>
        /// 百分之五十概率
        /// </summary>
        /// <returns></returns>
        public static bool Rand_HalfPercent()
        {
            if (Rand_Sign() == 1) return true;
            return false;
        }
        /// <summary>
        /// 根据概率随机
        /// </summary>
        /// <param name="list">概率分布</param>
        /// <returns>索引</returns>
        public static int RandRange_Percent(List<uint> list)
        {
            if (list == null || list.Count == 0) return -1;

            uint total_percent = 0;
            list.ForEach(delegate(uint v) { total_percent += v; });
            //随机一个概率值，用这个概率值去判断落在哪个区间；之所以从0开始，是后面的区间判断使用的是>=&&<方式
            int random_value = MathUtils.RandRange_Int(0, (int)total_percent);

            uint cur_total = 0;
            //遍历需要随机的道具列表，看处于哪个区间
            for (int i = 0; i < list.Count; ++i)
            {
                uint cur_random = list[i];
                if (random_value >= cur_total && random_value < cur_total + cur_random)
                {
                    return i;
                }
                else
                {
                    cur_total += cur_random;
                }
            }
            return 0;
        }
        /// <summary>
        /// 限制范围
        /// </summary>
        public static float Clamp(float n, float min, float max)
        {
            if (min > max)
            {
                var i = min;
                min = max;
                max = i;
            }

            return (n < min ? min : (n > max ? max : n));
        }
        /// <summary>
        /// 把一个数转换到0-360之间
        /// </summary>
        /// <param name="num"></param>
        /// <returns>[0,360)</returns>
        public static float Cleap0_360(float num)
        {
            num = num % 360;
            num = num < 0 ? num + 360 : num;

            return num;
        }
        /// <summary>
        /// 合并两个bounds
        /// </summary>
        public static Bounds UnionBounds(Bounds first, Bounds sec)
        {
            Vector3 vec = new Vector3();
            Bounds result = new Bounds();
            vec.x = MathUtils.Min(first.min.x, sec.min.x);
            vec.y = MathUtils.Min(first.min.y, sec.min.y);
            vec.z = MathUtils.Min(first.min.z, sec.min.z);
            result.min = vec;
            vec.x = MathUtils.Max(first.max.x, sec.max.x);
            vec.y = MathUtils.Max(first.max.y, sec.max.y);
            vec.z = MathUtils.Max(first.max.z, sec.max.z);
            result.max = vec;
            return result;
        }
        /// <summary>
        /// bound1是否包含bound2
        /// </summary>
        /// <param name="bound1"></param>
        /// <param name="bound2"></param>
        /// <returns></returns>
        public static bool ContainerBounds(Bounds bound1, Bounds bound2)
        {
            return true;
        }
        /**
         * 弧度转化为度 
         */
        public static float ToDegree(float radian)
        {
            return radian * (180.0f / 3.1415926f);
        }
        /// <summary>
        /// 度转化为弧度 
        /// </summary>
        public static float ToRadian(float degree)
        {
            return degree * (3.1415926f / 180.0f);
        }
        /// <summary>
        /// 向量投影到平面
        /// </summary>
        /// <param name="planeNormal"></param>
        /// <param name="vector"></param>
        /// <returns>投影向量</returns>
        public static Vector3 VectorProjectOnPlane(Vector3 planeNormal, Vector3 vector)
        {
            return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
        }
        /// <summary>
        /// 点到直线距离
        /// </summary>
        /// <param name="point">点坐标</param>
        /// <param name="linePoint1">直线上一个点的坐标</param>
        /// <param name="linePoint2">直线上另一个点的坐标</param>
        /// <returns></returns>
        public static float PointProjectLineDistance(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            Vector3 vec1 = point - linePoint1;
            Vector3 vec2 = linePoint2 - linePoint1;
            Vector3 vecProj = Vector3.Project(vec1, vec2);
            float dis = Mathf.Sqrt(Mathf.Pow(Vector3.Magnitude(vec1), 2) - Mathf.Pow(Vector3.Magnitude(vecProj), 2));
            return dis;
        }

        /// <summary>
        /// 判断一个点是否在相机视锥体
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static bool PtInFrustum(Camera camera, Vector3 pos)
        {
            Vector3 pt = camera.WorldToScreenPoint(pos);
            if (pt.x < 0 || pt.x > Screen.width || pt.y < 0 || pt.y > Screen.height || pt.z <= 0)
                return false;
            return true;
        }
        /// <summary>
        /// 转换为水平方向：忽视y轴
        /// </summary>
        public static Vector3 ToHorizontal(Vector3 vec)
        {
            vec.y = 0;
            return vec;
        }
        /// <summary>
        /// 水平距离：忽视y轴
        /// </summary>
        public static float HorizontalDistance(Vector3 vec1, Vector3 vec2)
        {
            vec1.y = 0;
            vec2.y = 0;
            return (vec1 - vec2).magnitude;
        }
        /// <summary>
        /// 转换为垂直方向：忽视z轴
        /// </summary>
        public static Vector3 ToVertical(Vector3 vec)
        {
            vec.z = 0;
            return vec;
        }
        /// <summary>
        /// 垂直距离：忽视z轴
        /// </summary>
        public static float VerticalDistance(Vector3 vec1, Vector3 vec2)
        {
            vec1.z = 0;
            vec2.z = 0;
            return (vec1 - vec2).magnitude;
        }
        /// <summary>
        /// 两点垂直方向夹角
        /// </summary>
        public static float VerticalAngle(Vector3 pt1, Vector3 pt2)
        {
            Vector3 pt_top, pt_bottom;
            if (pt1.y > pt2.y)
            { pt_top = pt1; pt_bottom = pt2; }
            else
            { pt_top = pt2; pt_bottom = pt1; }
            Vector3 pt = new Vector3(pt_top.x, pt_bottom.y, pt_top.z);
            return Vector3.Angle((pt_bottom - pt_top), (pt - pt_top));
        }
        /// <summary>
        /// 水平方向角度
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>度数[0,360)</returns>
        public static float HorizontalDegree(Vector3 direction)
        {
            float degree = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            return Cleap0_360(degree);
        }
        /// <summary>
        /// 求两个向量的有向夹角
        /// </summary>
        public static float VectorAngle(Vector3 fromVector, Vector3 toVector)
        {
            float angle = Vector3.Angle(fromVector, toVector); //求出两向量之间的夹角
            Vector3 normal = Vector3.Cross(fromVector, toVector);//叉乘求出法线向量
            angle *= Mathf.Sign(Vector3.Dot(normal, Vector3.up));  //求法线向量与物体上方向向量点乘，结果为1或-1，修正旋转方向
            return angle;
        }
    }
}