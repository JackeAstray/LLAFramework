using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameLogin.DatePicker
{
    public class DatePicker : UIBehaviour
    {
        private Text dateText = null;
        private Calendar calendar = null;
        private DateTime dateTime = DateTime.Today;

        // get data from this property
        public DateTime DateTime
        {
            get { return dateTime; }
            set
            {
                dateTime = value;
                RefreshDateText();
            }
        }

        protected override void Awake()
        {
            dateText = this.transform.Find("DateText").GetComponent<Text>();
            calendar = this.transform.Find("Calendar").GetComponent<Calendar>();
            calendar.onDayClick.AddListener(dateTime => { DateTime = dateTime; });
            transform.Find("PickButton").GetComponent<Button>().onClick.AddListener(() =>
             { calendar.gameObject.SetActive(true); });
            RefreshDateText();
        }

        private void RefreshDateText()
        {
            if (calendar.DisplayType == E_DisplayType.Standard)
            {
                switch (calendar.CalendarType)
                {
                    case E_CalendarType.Day:
                        dateText.text = DateTime.ToShortDateString();
                        break;
                    case E_CalendarType.Month:
                        dateText.text = DateTime.Year + "/" + DateTime.Month;
                        break;
                    case E_CalendarType.Year:
                        dateText.text = DateTime.Year.ToString();
                        break;
                }
            }
            else
            {
                switch (calendar.CalendarType)
                {
                    case E_CalendarType.Day:
                        dateText.text = DateTime.Year + "年" + DateTime.Month + "月" + DateTime.Day + "日";
                        break;
                    case E_CalendarType.Month:
                        dateText.text = DateTime.Year + "年" + DateTime.Month + "月";
                        break;
                    case E_CalendarType.Year:
                        dateText.text = DateTime.Year + "年";
                        break;
                }
            }
            calendar.gameObject.SetActive(false);
        }
    }
}