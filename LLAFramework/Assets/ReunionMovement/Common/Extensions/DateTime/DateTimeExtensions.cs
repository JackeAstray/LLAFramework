using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 时间扩展
    /// </summary>
    public static class DateTimeExtensions
    {
        // 时间戳
        private static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public enum TimeType
        {
            Seconds = 1,
            Minutes = 2,
            Hours = 3,
            Days = 4
        }

        /// <summary>
        /// 获得增加时间后的时间
        /// </summary>
        /// <param name="type">1、秒 2、分钟 3、小时 4、天</param>
        /// <param name="value">索要添加的数值</param>
        /// <returns></returns>
        public static DateTime GetAddTime(TimeType type, int value)
        {
            DateTime time = DateTime.Now;
            DateTime result = DateTime.Now;

            switch (type)
            {
                case TimeType.Seconds:
                    return time.AddSeconds(value);
                case TimeType.Minutes:
                    return time.AddMinutes(value);
                case TimeType.Hours:
                    return time.AddHours(value);
                case TimeType.Days:
                    return time.AddDays(value);
                default:
                    throw new ArgumentException("无效的时间类型.");
            }
        }

        /// <summary>
        /// 获取目标时间与当前时间的时间差
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeDifference(this DateTime time)
        {
            //当前时间减去目标时间
            return DateTimeOffset.Now - time;
        }

        /// <summary>
        /// 获取目标时间与当前时间的时间差
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeDifference(this DateTimeOffset time)
        {
            //当前时间减去目标时间
            return DateTimeOffset.Now - time;
        }

        /// <summary>
        /// 获取时间差字符串
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string GetTimeDifferenceString(this TimeSpan timeSpan)
        {
            string timeStr = string.Format("{0}{1}{2}{3}",
                timeSpan.Days == 0 ? "" : timeSpan.Days + "天",
                timeSpan.Hours == 0 ? "" : timeSpan.Hours + "小时",
                timeSpan.Minutes == 0 ? "" : timeSpan.Minutes + "分钟",
                timeSpan.Seconds < 0 ? "0秒" : timeSpan.Seconds + "秒");

            return timeStr;
        }

        /// <summary>
        /// 获取时间差字符串
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string GetTimeDifferenceString(this int seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            string timeStr = string.Format("{0}{1}{2}{3}",
                timeSpan.Days == 0 ? "" : timeSpan.Days + "天",
                timeSpan.Hours == 0 ? "" : timeSpan.Hours + "小时",
                timeSpan.Minutes == 0 ? "" : timeSpan.Minutes + "分钟",
                timeSpan.Seconds < 0 ? "0秒" : timeSpan.Seconds + "秒");

            return timeStr;
        }



        /// <summary>
        /// 秒数据换算为日期
        /// </summary>
        /// <param name="tick"></param>
        /// <param name="totalMilliseconds"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(this long tick, bool totalMilliseconds = false)
        {
            var dateTimeOffset = totalMilliseconds ? UnixEpoch.AddMilliseconds(tick) : UnixEpoch.AddSeconds(tick);
            return dateTimeOffset.LocalDateTime;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="totalMilliseconds"></param>
        /// <returns></returns>
        public static long GetTimestamp(this DateTime dateTime, bool totalMilliseconds = false)
        {
            var dateTimeOffset = new DateTimeOffset(dateTime);
            return totalMilliseconds ? dateTimeOffset.ToUnixTimeMilliseconds() : dateTimeOffset.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <param name="totalMilliseconds"></param>
        /// <returns></returns>
        public static long GetTimestamp(this DateTimeOffset dateTime, bool totalMilliseconds = false)
        {
            return totalMilliseconds ? dateTime.ToUnixTimeMilliseconds() : dateTime.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 格式的日期string 转换为 DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static DateTime GetDateTimeByString(this string dateTime)
        {
            string[] formats = new string[]
            {
            "yyyy-MM-dd HH:mm:ss",
            "yyyy/MM/dd HH:mm:ss",
            "yyyy-MM-dd",
            "yyyy/MM/dd",
            };

            if (DateTime.TryParseExact(dateTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime result))
            {
                return result;
            }
            else
            {
                throw new FormatException("无效的日期时间格式。");
            }
        }

        /// <summary>
        /// 获取时间字符串
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string GetStringByDateTime(this DateTime self, int type = 0)
        {
            string timeStr = "";

            switch (type)
            {
                case 0:
                    timeStr = self.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case 1:
                    timeStr = self.ToString("yyyy/MM/dd HH:mm:ss");
                    break;
            }

            return timeStr;
        }
    }
}