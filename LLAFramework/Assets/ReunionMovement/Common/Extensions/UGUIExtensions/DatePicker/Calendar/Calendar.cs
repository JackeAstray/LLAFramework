using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace GameLogin.DatePicker
{

    public class Calendar : UIBehaviour
    {
        #region click events
        public class DayClickEvent : UnityEvent<DateTime> { }
        public class MonthClickEvent : UnityEvent<DateTime> { }
        public class YearClickEvent : UnityEvent<DateTime> { }

        private DayClickEvent m_onDayClickEvent = new DayClickEvent();
        public DayClickEvent onDayClick
        {
            get { return m_onDayClickEvent; }
            set { m_onDayClickEvent = value; }
        }
        private MonthClickEvent m_onMonthClickEvent = new MonthClickEvent();
        public MonthClickEvent onMonthClick
        {
            get { return m_onMonthClickEvent; }
            set { m_onMonthClickEvent = value; }
        }
        private YearClickEvent m_onYearClickEvent = new YearClickEvent();
        public YearClickEvent onYearClick
        {
            get { return m_onYearClickEvent; }
            set { m_onYearClickEvent = value; }
        }

        #endregion

        #region private && public members
        private DateTime selectDT = DateTime.Today;
        private readonly CalendarData calendarData = new CalendarData();
        private Transform thisTransform = null;
        [HideInInspector]
        public E_CalendarType CalendarType = E_CalendarType.Day;
        public E_DisplayType DisplayType = E_DisplayType.Chinese;
        private Text timeButtonText = null;
        private GameObject weeksGameObject = null;
        private GameObject daysGameObejct = null;
        private GameObject monthGameObject = null;
        private readonly List<DMY> daysPool = new List<DMY>();
        private readonly List<DMY> monthYearPool = new List<DMY>();
        #endregion

        protected override void Awake()
        {
            thisTransform = transform;
            timeButtonText = thisTransform.Find("Title/TimeButton/Text").GetComponent<Text>();
            weeksGameObject = thisTransform.Find("Container/Weeks").gameObject;
            daysGameObejct = thisTransform.Find("Container/Days").gameObject;
            monthGameObject = thisTransform.Find("Container/Months").gameObject;
            var weekPrefab = weeksGameObject.transform.Find("WeekTemplate").gameObject;
            var dayPrefab = daysGameObejct.transform.Find("DayTemplate").gameObject;
            var monthPrefab = monthGameObject.transform.Find("MonthTemplate").gameObject;
            WeekGenerator(weekPrefab, weeksGameObject.transform);
            DayGenerator(dayPrefab, daysGameObejct.transform);
            MonthGenerator(monthPrefab, monthGameObject.transform);
            thisTransform.Find("Title/NextButton").GetComponent<Button>().onClick.AddListener(OnNextButtonClick);
            thisTransform.Find("Title/LastButton").GetComponent<Button>().onClick.AddListener(OnLastButtonClick);
            thisTransform.Find("Title/TimeButton").GetComponent<Button>().onClick.AddListener(OnTimeButtonClick);
            Refresh();
        }

        #region operation functions
        private void OnTimeButtonClick()
        {
            if (CalendarType == E_CalendarType.Month)
                CalendarType = E_CalendarType.Year;
            if (CalendarType == E_CalendarType.Day)
            {
                CalendarType = E_CalendarType.Month;
                CalendarTypeChange(false);
            }
            Refresh();
        }
        private void OnNextButtonClick()
        {
            if (CalendarType == E_CalendarType.Day)
                selectDT = selectDT.AddMonths(1);
            else if (CalendarType == E_CalendarType.Month)
                selectDT = selectDT.AddYears(1);
            else
                selectDT = selectDT.AddYears(12);
            Refresh();
        }
        private void OnLastButtonClick()
        {
            if (CalendarType == E_CalendarType.Day)
                selectDT = selectDT.AddMonths(-1);
            else if (CalendarType == E_CalendarType.Month)
                selectDT = selectDT.AddYears(-1);
            else
                selectDT = selectDT.AddYears(-12);
            Refresh();
        }
        #endregion

        #region days && weeks && months generator
        private void WeekGenerator(GameObject weekPrefab, Transform parent)
        {
            for (int i = 0; i < 7; i++)
            {
                GameObject week = PrefabGenerator(weekPrefab, parent);
                week.GetComponent<Text>().text = GetWeekName(i.ToString());
            }
            Destroy(weekPrefab);
        }
        private void DayGenerator(GameObject dayPrefab, Transform parent)
        {
            for (int i = 0; i < 42; i++)
            {
                GameObject day = PrefabGenerator(dayPrefab, parent);
                DMY dmy = day.AddComponent<DMY>();
                day.GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectDT = dmy.DateTime;
                    onDayClick.Invoke(dmy.DateTime);
                    Refresh();
                });
                daysPool.Add(dmy);
            }
            Destroy(dayPrefab);
        }
        private void MonthGenerator(GameObject monthPrefab, Transform parent)
        {
            for (int i = 0; i < 12; i++)
            {
                GameObject month = PrefabGenerator(monthPrefab, parent);
                DMY dmy = month.AddComponent<DMY>();
                month.GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectDT = dmy.DateTime;
                    if (CalendarType == E_CalendarType.Month)
                    {
                        CalendarType = E_CalendarType.Day;
                        CalendarTypeChange(true);
                        onMonthClick.Invoke(dmy.DateTime);
                    }
                    if (CalendarType == E_CalendarType.Year)
                    {
                        CalendarType = E_CalendarType.Month;
                        onYearClick.Invoke(dmy.DateTime);
                    }
                    Refresh();
                });
                monthYearPool.Add(dmy);
            }
            Destroy(monthPrefab);
        }
        private GameObject PrefabGenerator(GameObject prefab, Transform parent)
        {
            GameObject go = Object.Instantiate(prefab);
            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one;
            return go;
        }
        private string GetWeekName(string weekName)
        {
            switch (DisplayType)
            {
                case E_DisplayType.Standard:
                    switch (weekName)
                    {
                        case "0":
                            return "Sunday";
                        case "1":
                            return "Monday";
                        case "2":
                            return "Tuesday";
                        case "3":
                            return "Wednesday";
                        case "4":
                            return "Thursday";
                        case "5":
                            return "Friday";
                        case "6":
                            return "Saturday";
                        default:
                            return "";
                    }
                case E_DisplayType.Chinese:
                    switch (weekName)
                    {
                        case "0":
                            return "日";
                        case "1":
                            return "一";
                        case "2":
                            return "二";
                        case "3":
                            return "三";
                        case "4":
                            return "四";
                        case "5":
                            return "五";
                        case "6":
                            return "六";
                        default:
                            return "";
                    }
                default:
                    return "";
            }

        }
        private void CalendarTypeChange(bool isDays)
        {
            weeksGameObject.SetActive(isDays);
            daysGameObejct.SetActive(isDays);
            monthGameObject.SetActive(!isDays);
        }
        #endregion

        #region refresh calendar all component
        private void Refresh()
        {
            RefreshCalendar();
            RefreshTimeButtonContent();
        }
        private void RefreshTimeButtonContent()
        {
            switch (CalendarType)
            {
                case E_CalendarType.Day:
                    if (DisplayType == E_DisplayType.Standard) timeButtonText.text = selectDT.ToShortDateString();
                    else timeButtonText.text = selectDT.Year + "年" + selectDT.Month + "月" + selectDT.Day + "日";
                    break;
                case E_CalendarType.Month:
                    if (DisplayType == E_DisplayType.Standard) timeButtonText.text = selectDT.Year + "/" + selectDT.Month;
                    else timeButtonText.text = selectDT.Year + "年" + selectDT.Month + "月";
                    break;
                case E_CalendarType.Year:
                    if (DisplayType == E_DisplayType.Standard) timeButtonText.text = selectDT.Year.ToString();
                    else timeButtonText.text = selectDT.Year + "年";
                    break;
            }
        }
        private void RefreshCalendar()
        {
            if (CalendarType == E_CalendarType.Day) RefreshDays(calendarData.Days(selectDT));
            else if (CalendarType == E_CalendarType.Month) RefreshMonths(calendarData.Months(selectDT));
            else RefreshYears(calendarData.Years(selectDT));
        }
        private void RefreshDays(List<DateTime> dateTimes)
        {
            for (int i = 0; i < daysPool.Count; i++)
            {
                var fontColor = Color.black;
                if (dateTimes[i].Month != selectDT.Month)
                    fontColor = Color.gray;
                daysPool[i].SetDay(dateTimes[i], DisplayType, fontColor);
            }
        }
        private void RefreshMonths(List<DateTime> dateTimes)
        {
            for (int i = 0; i < monthYearPool.Count; i++)
                monthYearPool[i].SetMonth(dateTimes[i], DisplayType);
        }
        private void RefreshYears(List<DateTime> dateTimes)
        {
            for (int i = 0; i < monthYearPool.Count; i++)
                monthYearPool[i].SetYear(dateTimes[i], DisplayType);
        }
        #endregion
    }

    public enum E_CalendarType
    {
        Day,
        Month,
        Year
    }
    public enum E_DisplayType
    {
        Standard,
        Chinese
    }
    public class CalendarData
    {
        public List<DateTime> Days(DateTime month)
        {
            List<DateTime> days = new List<DateTime>();
            DateTime firstDay = new DateTime(month.Year, month.Month, 1);
            DayOfWeek week = firstDay.DayOfWeek;
            int lastMonthDays = (int)week;
            if (lastMonthDays.Equals(0))
                lastMonthDays = 7;
            for (int i = lastMonthDays; i > 0; i--)
                days.Add(firstDay.AddDays(-i));
            for (int i = 0; i < 42 - lastMonthDays; i++)
                days.Add(firstDay.AddDays(i));
            return days;
        }
        public List<DateTime> Months(DateTime year)
        {
            List<DateTime> months = new List<DateTime>();
            DateTime firstMonth = new DateTime(year.Year, 1, 1);
            months.Add(firstMonth);
            for (int i = 1; i < 12; i++)
                months.Add(firstMonth.AddMonths(i));
            return months;
        }
        public List<DateTime> Years(DateTime year)
        {
            List<DateTime> years = new List<DateTime>();
            for (int i = 5; i > 0; i--)
                years.Add(year.AddYears(-i));
            for (int i = 0; i < 7; i++)
                years.Add(year.AddYears(i));
            return years;
        }
    }
    public class DMY : UIBehaviour
    {
        public DateTime DateTime { get; set; }
        private Text text = null;
        protected override void Awake()
        {
            text = transform.Find("Text").GetComponent<Text>();
        }
        public void SetDay(DateTime dateTime, E_DisplayType displayType, Color fontColor)
        {
            DateTime = dateTime;
            text.text = dateTime.Day.ToString();
            text.color = fontColor;
        }
        public void SetMonth(DateTime dateTime, E_DisplayType displayType)
        {
            DateTime = dateTime;
            text.text = GetMonthString(dateTime.Month.ToString(), displayType);
        }
        public void SetYear(DateTime dateTime, E_DisplayType displayType)
        {
            DateTime = dateTime;
            text.text = dateTime.Year.ToString();
        }
        private string GetMonthString(string month, E_DisplayType displayType)
        {
            switch (displayType)
            {
                case E_DisplayType.Standard:
                    switch (month)
                    {
                        case "1":
                            return "Jan.";
                        case "2":
                            return "Feb.";
                        case "3":
                            return "Mar.";
                        case "4":
                            return "Apr.";
                        case "5":
                            return "May.";
                        case "6":
                            return "Jun.";
                        case "7":
                            return "Jul.";
                        case "8":
                            return "Aug.";
                        case "9":
                            return "Sept.";
                        case "10":
                            return "Oct.";
                        case "11":
                            return "Nov.";
                        case "12":
                            return "Dec.";
                        default:
                            return "";
                    }
                case E_DisplayType.Chinese:
                    switch (month)
                    {
                        case "1":
                            return "一月";
                        case "2":
                            return "二月";
                        case "3":
                            return "三月";
                        case "4":
                            return "四月";
                        case "5":
                            return "五月";
                        case "6":
                            return "六月";
                        case "7":
                            return "七月";
                        case "8":
                            return "八月";
                        case "9":
                            return "九月";
                        case "10":
                            return "十月";
                        case "11":
                            return "十一月";
                        case "12":
                            return "十二月";
                        default:
                            return "";
                    }
                default:
                    return "";
            }
        }
    }

}