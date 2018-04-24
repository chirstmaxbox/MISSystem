using System;

namespace MyCommon
{
    public class MyDateTimePart
    {
        //DateTime
        public DateTime Value { get; set; }

        //DaetTime Part
        public string Ampm { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public DateTime YearMonthDate { get; set; }

        public MyDateTimePart(DateTime initialDatetime)
        {
            Value = initialDatetime;
            Initialization();
        }

        #region ****************** From DateTime Part to Datetime *****************

        private void Initialization()
        {
            int interval = 15;

            YearMonthDate = Value.Date;
            int hour = Value.Hour;
            //0, 15, 45
            int minute = Value.Minute;
            if (minute >= 45)
            {
                hour += 1;
                minute = 0;
            }
            else
            {
                minute = minute / interval;
                minute = (minute + 1) * interval;
            }

            Hour = hour;
            Minute = minute;
            Ampm = "AM";
            if (hour > 12)
            {
                Hour = hour - 12;
                Ampm = "PM";
            }
        }

        public void Plus24Hours()
        {
            System.DateTime dt2;
            switch (Value.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    Value = Value.AddDays(3);
                    break;
                case DayOfWeek.Saturday:
                    Value = Value.AddDays(2);
                    dt2 = new System.DateTime(Value.Year, Value.Month, Value.Day, 18, 0, 0);
                    Value = dt2;
                    break;
                case DayOfWeek.Sunday:
                    Value = Value.AddDays(1);
                    dt2 = new System.DateTime(Value.Year, Value.Month, Value.Day, 18, 0, 0);
                    Value = dt2;
                    break;
                default:
                    Value = Value.AddDays(1);
                    break;
            }
            Initialization();

        }

        public void Plus48Hours()
        {
            System.DateTime dt2;
            switch (Value.DayOfWeek)
            {
                case DayOfWeek.Thursday:
                case DayOfWeek.Friday:
                    Value = Value.AddDays(4);
                    break;
                case DayOfWeek.Saturday:
                    Value = Value.AddDays(3);
                    dt2 = new System.DateTime(Value.Year, Value.Month, Value.Day, 18, 0, 0);
                    Value = dt2;
                    break;
                case DayOfWeek.Sunday:
                    Value = Value.AddDays(2);
                    dt2 = new System.DateTime(Value.Year, Value.Month, Value.Day, 18, 0, 0);
                    Value = dt2;
                    break;
                default:
                    Value = Value.AddDays(2);
                    break;
            }
        }

        #endregion

        #region ****************** From DateTime Part to Datetime *****************

        public void GetValue()
        {
            //Get the Required Time
            //Tomorrow , 00:00:00

            Hour = MyCommon.MyDateTime.ConvertClockSystemFrom12To24(Hour, Ampm);


            Value = new DateTime(YearMonthDate.Year, YearMonthDate.Month, YearMonthDate.Day, Hour, Minute, 0);

        }

        #endregion

    }
}