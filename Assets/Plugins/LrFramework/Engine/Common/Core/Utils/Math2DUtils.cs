using UnityEngine;
using System.Collections;

namespace Table
{
    /// <summary>
    /// 数学-2D
    /// @author hannibal
    /// @time 2014-12-2
    /// </summary>
    public class Math2DUtils
    {
        /// <summary>
        /// 距离平方值
        /// </summary>
        public static float distance_square(float x1, float y1, float x2, float y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }
        /// <summary>
        /// 距离
        /// </summary>
        public static float distance(float x1, float y1, float x2, float y2)
        {
            return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }
        public static float distance(float x, float y)
        {
            return Mathf.Sqrt(x * x + y * y);
        }
        /**
         * 两点间的弧度
                ^(y)
                |(90)
                |
                |
    (180)-------|--------->(x)
                |		  (0)
                |
                |
                |(270)
         */
        public static float LineRadians(float x1, float y1, float x2, float y2)
        {
            return Mathf.Atan2(y2 - y1, x2 - x1);
        }
        /// <summary>
        /// 方向转弧度
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float Point2Radians(float x, float y)
        {
            return Mathf.Atan2(y, x);
        }
        /// <summary>
        /// 弧度转方向
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static Vector2 Radians2Point(float radian)
        {
            Vector2 pt = new Vector2();
            pt.x = Mathf.Cos(radian);
            pt.y = Mathf.Sin(radian);
            return pt;
        }
        /// <summary>
        /// 根据度数获得朝向
        /// X轴正方向为1，顺时钟方向为加
        /// </summary>
        /// <param name="degree">度数(0-360)</param>
        /// <param name="chunkNums">划分几份</param>
        /// <returns></returns>
        public static uint getFace(float degree, uint chunkNums)
        {
            float perAngle = 360 / chunkNums;
            uint nFace = (uint)((MathUtils.Cleap0_360(degree) + perAngle * 0.5f) / perAngle) + 1;//从1开始
            nFace = nFace > chunkNums ? nFace - chunkNums : nFace;
            return nFace;
        }

        public static float getAngleByFace8(int face)
        {
            float angle = 0;
            if (face >= 0 && face <= 8)
                angle = (face - 1) * 45;
            return angle;
        }
        /// <summary>
        /// 左边还是右边
        /// </summary>
        /// <param name="num">度数</param>
        /// <returns>0~89或271~360=1，90或270=0,91~269=-1</returns>
        public static int getLeftOrRightFace(int degree)
        {
            degree = (int)MathUtils.Cleap0_360((float)degree);
            if (degree >= 0 && degree <= 90 || degree >= 270 && degree <= 360)
            {
                return 1;
            }

            return -1;
        }
        /// <summary>
        /// 获取左中右
        /// </summary>
        static public int getSide(float i)
        {
            return i > 0 ? 1 : (i == 0 ? 0 : -1);
        }
        /// <summary>
        /// 获得象限：x右方向为0，逆时针转
        /// </summary>
        /// <param name="fDegree">0～360</param>
        /// <param name="chunkNums">划分几份</param>
        /// <returns>[0,chunkNums)</returns>
        static public uint GetQuadrant(float fDegree, uint chunkNums)
        {
            if (chunkNums == 0)
            {
                return 0;
            }
            float perAngle = 360.0f / chunkNums;
            uint nFace = (uint)((fDegree + perAngle * 0.5f) / perAngle);//从0开始
            nFace = nFace >= chunkNums ? nFace - chunkNums : nFace;
            return nFace;
        }
        /// <summary>
        /// 获得所处区间 
        /// </summary>
        /// <param name="cur">当前值</param>
        /// <param name="total">总值</param>
        /// <param name="section_count">区间总数</param>
        /// <returns>1开头的区间值</returns>
        public static int getSection(float cur, float total, int section_count)
        {
            float perAngle = total / section_count;
            int sec = (int)(cur / perAngle + 1);//从1开始
            sec = (int)MathUtils.Clamp(sec, 1, section_count);
            return sec;
        }
        /// <summary>
        /// 单位化
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="power">长度</param>
        public static Vector2 normalPoint(float x1, float y1, float x2, float y2, float power = 1)
        {
            Vector2 pt = new Vector2(x2 - x1, y2 - y1);
            pt.Normalize();
            pt *= power;
            return pt;
        }
        /// <summary>
        /// 两个矩形是否相交 
        /// </summary>
        /// <param name="rect1"></param>
        /// <param name="rect2"></param>
        /// <returns></returns>
        public static bool intersectRect(Rect rect1, Rect rect2)
        {
            if ((rect1.xMax > rect2.xMin) &&
                (rect1.xMin < rect2.xMax) &&
                (rect1.yMax > rect2.yMin) &&
                (rect1.yMin < rect2.yMax) ||
                (rect2.xMax > rect1.xMin) &&
                (rect2.xMin < rect1.xMax) &&
                (rect2.yMax > rect1.yMin) &&
                (rect2.yMin < rect1.yMax))
            {
                return true;
            }
            return false;
        }
        public static bool containerCircle(Vector2 c, Vector2 h, Vector2 p, float r)
        {
            Vector2 v = p - c;    // 第1步：转换至第1象限
            v = new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
            Vector2 u = v - h; // 第2步：求圆心至矩形的最短距离矢量
            //u = new Vector2(u.x < 0 ? 0 : u.x, u.y < 0 ? 0 : u.y);
            return Vector2.Dot(u, u) <= r * r; // 第3步：长度平方与半径平方比较
        }
        /// <summary>
        /// 两个向量夹角
        /// </summary>
        public static float AngleBetween(Vector2 vector1, Vector2 vector2)
        {
            float _sin = vector1.x * vector2.y - vector2.x * vector1.y;
            float _cos = vector1.x * vector2.x + vector1.y * vector2.y;

            float fATan = Mathf.Atan2(_sin, _cos);

            // [0~360]
            if (fATan > 2 * Mathf.PI)
                fATan %= 2 * Mathf.PI;
            else if (fATan < 0)
                fATan += 2 * Mathf.PI;
            return 2 * Mathf.PI - fATan;
        }
    }
}