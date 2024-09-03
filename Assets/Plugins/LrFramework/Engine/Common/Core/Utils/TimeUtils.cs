using UnityEngine;
using System.Collections;
using System;

namespace Table
{
    /// <summary>
    /// 时间工具类，内部使用，逻辑层统一使用Time.cs
    /// @author hannibal
    /// @time 2014-11-14
    /// </summary>
    public class TimeUtils
    {
        /// <summary>
        /// 北京时间与utc时间相差秒数
        /// </summary>
        public const long UTC_LOACL_TIME = 28800L;

        /// <summary>
        /// 1970到现在的毫秒数
        /// </summary>
        static public long MillisecondSince1970
        {
            get
            {
                System.TimeSpan duration = System.DateTime.Now - DateTime.Parse("1970-1-1");
                return (long)duration.TotalMilliseconds;
            }
        }
        /// <summary>
        /// 1970到现在的秒数
        /// </summary>
        static public long SecondSince1970
        {
            get
            {
                System.TimeSpan duration = System.DateTime.Now - DateTime.Parse("1970-1-1");
                return (long)duration.TotalSeconds;
            }
        }
        /// <summary>
        /// 1970到现在的天数
        /// </summary>
        static public long DaySince1970
        {
            get
            {
                System.TimeSpan duration = System.DateTime.Now - DateTime.Parse("1970-1-1");
                return (long)duration.TotalDays;
            }
        }

        /// <summary>
        /// 秒转换为字符串时间
        /// </summary>
        /// <param name="second"></param>
        /// <returns>yyyy-MM-dd-hh:mm:ss</returns>
        static public string GetSecondSince1970(long second)
        {
            System.DateTime baseDate = new System.DateTime(1970, 1, 1);
            baseDate = baseDate.AddSeconds(second);
            // 调整为当前系统时区
            baseDate = baseDate.ToLocalTime();
            return baseDate.ToString("yyyy-MM-dd-hh:mm:ss");
        }
        static public string GetDaySince1970(long day)
        {
            System.DateTime baseDate = new System.DateTime(1970, 1, 1);
            baseDate = baseDate.AddDays(day);
            // 调整为当前系统时区
            baseDate = baseDate.ToLocalTime();
            return baseDate.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 获取当前时间  “年” “月” “日” “时” “分”
        /// </summary>
        static public string GetNowTime(out UInt32 year,
                                        out UInt32 month,
                                        out UInt32 day,
                                        out UInt32 hour,
                                        out UInt32 min)
        {
            System.DateTime baseDate = new System.DateTime(1970, 1, 1);
            System.TimeSpan duration = System.DateTime.Now - baseDate;
            return GetTimeSince1970((UInt32)duration.TotalSeconds,
                                    out year,
                                    out month,
                                    out day,
                                    out hour,
                                    out min);
        }

        static public string GetNowTime()
        {
            System.DateTime baseDate = new System.DateTime(1970, 1, 1);
            System.TimeSpan duration = System.DateTime.Now - baseDate;
            return GetSecondSince1970((long)duration.TotalSeconds);
        }

        static public string GetTimeSince1970(long second,
                                              out UInt32 year,
                                              out UInt32 month,
                                              out UInt32 day,
                                              out UInt32 hour,
                                              out UInt32 min)
        {
            System.DateTime baseDate = new System.DateTime(1970, 1, 1);
            baseDate = baseDate.AddSeconds(second);
            // 调整为当前系统时区
            baseDate = baseDate.ToLocalTime();
            year = (UInt32)baseDate.Year;
            month = (UInt32)baseDate.Month;
            day = (UInt32)baseDate.Day;
            hour = (UInt32)baseDate.Hour;
            min = (UInt32)baseDate.Minute;
            return baseDate.ToString("yyyy-MM-dd-hh:mm");
        }

        /// <summary>
        /// utc转北京时间
        /// </summary>
        /// <param name="secondsFromUtcStart"></param>
        /// <returns></returns>
        static public DateTime UtcTime2Local(long secondsFromUtcStart)
        {
            DateTime dateTime = new DateTime(1970, 1, 1);
            return dateTime.AddTicks((secondsFromUtcStart + 28800L) * 10000000L);
        }
    }
}