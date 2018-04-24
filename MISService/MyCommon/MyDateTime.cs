using System;
using System.Globalization;

namespace MyCommon
{
    public class MyDateTime
    {
        //Difference
        public static double GetHoursDiffOfTwoObjects(object obj1, object obj2)
        {
            double d = 0;
            //Neither of the objs is null
            if (!(MyConvert.IsNullString(obj1) || MyConvert.IsNullString(obj2)))
            {
                //both is date
                if (MyConvert.IsDate(obj1) & MyConvert.IsDate(obj2))
                {
                    System.DateTime dt1 = Convert.ToDateTime(obj1);
                    System.DateTime dt2 = Convert.ToDateTime(obj2);
                    //differnece of the hours, only the hours
                    d = GetHoursDifferenceOfTwoHours(dt1, dt2);
                }
            }

            return d;
        }
        public static double GetHoursDifferenceOfTwoDates(DateTime dt1, DateTime dt2)
        {
            double hoursWorked = 0;
            if (dt1 < dt2)
            {
                hoursWorked = (dt2.DayOfYear - dt1.DayOfYear)*24*60;
                //Day
                hoursWorked += (dt2.Hour - dt1.Hour)*60;
                //Hours
                hoursWorked += (dt2.Minute - dt1.Minute);
                //Minutes
                hoursWorked += (dt2.Second - dt1.Second)/60;
                hoursWorked = Math.Round(hoursWorked/60, 2);
            }

            return hoursWorked;
        }

        public static double GetHoursDifferenceOfTwoHours(DateTime dt1, DateTime dt2)
        {
            double hoursWorked = 0;
            hoursWorked += (dt2.Hour - dt1.Hour)*60;
            //Hours
            hoursWorked += (dt2.Minute - dt1.Minute);
            //Minutes
            hoursWorked += (dt2.Second - dt1.Second)/60;
            hoursWorked = Math.Round(hoursWorked/60, 2);
            return hoursWorked;
            //result = ((bdt2.Hour - bdt1.Hour) * 60 + bdt2.Minute - bdt1.Minute) / 60
        }

        public static int GetDateDiff(DateTime startDate, DateTime endDate)
        {
            return (endDate-startDate ).Days;
        }
        public static int GetDateTimeYearDifference(DateTime startDate, DateTime endDate)
        {
            TimeSpan span = startDate.Subtract(endDate);
            int d = Convert.ToInt32(span.Days / 365);
            return Math.Abs(d);
        }

     //BusinessDay
        public static int GetDiffHoursOfWeekday(DateTime bt, DateTime et)
        {
            //Not exactly return the diff hours
            Double Hours = 0;
            var dtBase = new DateTime(2000, 1, 1);

            int dt1 = (bt - dtBase).Days;
            int dt2 = (et - dtBase).Days;
            //does no matter
            if (dt2 < dt1)
            {
                return 0;
            }

            //Sae Day
            if (dt2 == dt1)
            {
                Double hStart = Convert.ToDouble(bt.Hour) + bt.Minute/60.0;
                Double hEnd = Convert.ToDouble(et.Hour) + et.Minute/60.0;
                Hours = hEnd - hStart;
                return Convert.ToInt32(Hours);
            }

            //Week Difference
            int weekDiff = (dt2 - dt1)/7;
            if (weekDiff > 0)
            {
                return 24*5;
            }

            //<7

            switch (bt.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    bt = bt.AddDays(2);
                    break;
                case DayOfWeek.Saturday:
                    bt = bt.AddDays(1);
                    break;
                default:
                    break;
            }


            Hours = (et.DayOfYear - bt.DayOfYear)*24;

            Double hStart1 = Convert.ToDouble(bt.Hour) + bt.Minute/60.0;
            Double hEnd1 = Convert.ToDouble(et.Hour) + et.Minute/60.0;
            Hours += hEnd1 - hStart1;

            return Convert.ToInt32(Hours);
        }

        public static DateTime GetDateOfMinusBusinessDays(DateTime startDate, int numberOfBusinessDays)
        {

                  //Start from the day after
            //Knock the start date down one day if it is on a weekend.
            if (startDate.DayOfWeek == DayOfWeek.Saturday | startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                numberOfBusinessDays += 1;
            }

            for (int index = 1; index <=Math.Abs(numberOfBusinessDays); index++)
            {
                switch (startDate.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        startDate = startDate.AddDays(-3);
                        break;
                    case DayOfWeek.Monday:
                    case DayOfWeek.Tuesday:
                    case DayOfWeek.Wednesday:
                    case DayOfWeek.Thursday:
                    case DayOfWeek.Friday:
                        startDate = startDate.AddDays(-1);
                        break;
                    case DayOfWeek.Saturday:
                        startDate = startDate.AddDays(-2);
                        break;
                }
            }

            //check to see if the end date is on a weekend.
            //Option 1: move it ahead to Monday.

            if (startDate.DayOfWeek == DayOfWeek.Saturday)
            {
                startDate = startDate.AddDays(-1);
            }
            else if (startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                startDate = startDate.AddDays(-2);
            }

            //'Option 2: bump it back to the Friday before 
            //If StartDate.DayOfWeek = DayOfWeek.Saturday Then
            //    StartDate = StartDate.AddDays(-2)
            //ElseIf StartDate.DayOfWeek = DayOfWeek.Sunday Then
            //    StartDate = StartDate.AddDays(-1)
            //End If

          

            return startDate;
        }

        public static DateTime GetDateOfAddedBusinessDays(DateTime startDate, int numberOfBusinessDays)
        {
            //Start from the day after
            //Knock the start date down one day if it is on a weekend.
            if (startDate.DayOfWeek == DayOfWeek.Saturday | startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                numberOfBusinessDays -= 1;
            }

            for (int index = 1; index <=numberOfBusinessDays; index++)
            {
                switch (startDate.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        startDate = startDate.AddDays(2);
                        break;
                    case DayOfWeek.Monday:
                    case DayOfWeek.Tuesday:
                    case DayOfWeek.Wednesday:
                    case DayOfWeek.Thursday:
                    case DayOfWeek.Friday:
                        startDate = startDate.AddDays(1);
                        break;
                    case DayOfWeek.Saturday:
                        startDate = startDate.AddDays(3);
                        break;
                }
            }

            //check to see if the end date is on a weekend.
            //Option 1: move it ahead to Monday.

            if (startDate.DayOfWeek == DayOfWeek.Saturday)
            {
                startDate = startDate.AddDays(2);
            }
            else if (startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                startDate = startDate.AddDays(1);
            }

            //'Option 2: bump it back to the Friday before 
            //If StartDate.DayOfWeek = DayOfWeek.Saturday Then
            //    StartDate = StartDate.AddDays(-2)
            //ElseIf StartDate.DayOfWeek = DayOfWeek.Sunday Then
            //    StartDate = StartDate.AddDays(-1)
            //End If

            return startDate;
        }


        //Allow Native, backwords
            public static DateTime AddWorkDays( DateTime date, int workingDays)
            {
                int direction = workingDays < 0 ? -1 : 1;
                DateTime newDate = date;
                while (workingDays != 0)
                {
                    newDate = newDate.AddDays(direction);
                    if (newDate.DayOfWeek != DayOfWeek.Saturday &&
                        newDate.DayOfWeek != DayOfWeek.Sunday &&
                        !IsHoliday(newDate))
                    {
                        workingDays -= direction;
                    }
                }
                return newDate;
            }

            public static bool IsHoliday(DateTime date)
            {
                //            // You'd load/cache from a DB or file somewhere rather than hardcode
                //            DateTime[] holidays =
                //            new DateTime[] { 
                //  new DateTime(2010,12,27),
                //  new DateTime(2010,12,28),
                //  new DateTime(2011,01,03),
                //  new DateTime(2011,01,12),
                //  new DateTime(2011,01,13)
                //};

                //            return holidays.Contains(date.Date);
                return false;
            }
        

        public static string GetCurrentTime()
        {
            return DateTime.Today.ToLongDateString();
        }


        //DesignedDate
        private static DateTime GetFridayDate(DateTime originalDate)
        {
            DateTime d1 = originalDate;
            if (originalDate.DayOfWeek == DayOfWeek.Saturday | originalDate.DayOfWeek == DayOfWeek.Sunday)
            {
                DateTime monday = GetCurrentMondayDate(originalDate);
                d1 = monday.AddDays(4);
            }
            return d1;
        }
    
        public static DateTime GetCurrentMondayDate(DateTime date1)
        {
            var dt = date1;
            int i1 = (int) dt.DayOfWeek - 1;
            DateTime mondayDate = dt.AddDays(-i1);
            if (date1.DayOfWeek==DayOfWeek.Sunday )
            {
                mondayDate = mondayDate.AddDays(-7);
            }
            return mondayDate;
        }

        public static DateTime GetLastMonday()
        {
            DateTime dptMonday = DateTime.Today;
            string aDay = dptMonday.DayOfWeek.ToString();
            //create a time span object of one day  
            if (aDay != "Monday")
            {
                do
                {
                    dptMonday = dptMonday.AddDays(-1);
                    aDay = dptMonday.DayOfWeek.ToString();
                } while (aDay != "Monday");
            }

            DateTime td = DateTime.Today;
            if ((td.DayOfWeek != DayOfWeek.Saturday) && (td.DayOfWeek != DayOfWeek.Sunday))
            {
                dptMonday = dptMonday.AddDays(-7);
            }


            return dptMonday;
        }

        public static DateTime GetLastSaturday()
        {
            DateTime dptMonday = DateTime.Today.AddDays(0);
            string aDay = dptMonday.DayOfWeek.ToString();
            //create a time span object of one day  
            var ad = new TimeSpan(1, 0, 0, 0);
            if (aDay != "Saturday")
            {
                do
                {
                    dptMonday = dptMonday.AddDays(-1);
                    aDay = dptMonday.DayOfWeek.ToString();
                } while (aDay != "Saturday");
            }
            return dptMonday;
        }
        public static DateTime ConvertDatetime(DateTime datePart, DateTime timePart)
        {
            string sdt = "";
            sdt = datePart.ToLongDateString();
            sdt += " ";
            sdt += timePart.ToLongTimeString();
            DateTime dt = Convert.ToDateTime(sdt);
            return dt;
        }

        //public static System.DateTime ConvertDatetime(System.DateTime datePart, System.DateTime timePart)
        //{
        //    string sdt = "";
        //    System.DateTime dt = new DateTime(2007, 1, 1, 0, 0, 0);

        //    sdt = datePart.ToLongDateString();
        //    sdt += " ";
        //    sdt += timePart.ToLongTimeString();
        //    dt = Convert.ToDateTime(sdt);
        //    return dt;
        //}

        //Number
        public static int GetMonthNumber(DateTime workingDate)
        {
            int monthNumber = 0;


            int yearDiff = (workingDate.Year - DateTime.Today.Year)*12;

            int currentMonthNumber = DateTime.Today.Month;
            if (workingDate.Month + yearDiff >= currentMonthNumber + 2)
            {
                monthNumber = 2;
            }
            else if (workingDate.Month + yearDiff == currentMonthNumber + 1)
            {
                monthNumber = 1;
            }

            return monthNumber;
        }

        public static int GetWeekOfYear(DateTime datetimeInput)
        {
            //Max=4

            Calendar cal = CultureInfo.InvariantCulture.Calendar;

            DateTime calculatedate = datetimeInput;

            DayOfWeek day = cal.GetDayOfWeek(datetimeInput);

            if (day == DayOfWeek.Monday)
            {
                calculatedate = calculatedate.AddDays(3);
            }

            if (day == DayOfWeek.Tuesday)
            {
                calculatedate = calculatedate.AddDays(2);
            }

            if (day == DayOfWeek.Wednesday)
            {
                calculatedate = calculatedate.AddDays(1);
            }

            if (day == DayOfWeek.Friday)
            {
                calculatedate = calculatedate.AddDays(-1);
            }

            if (day == DayOfWeek.Saturday)
            {
                calculatedate = calculatedate.AddDays(-2);
            }

            if (day == DayOfWeek.Sunday)
            {
                calculatedate = calculatedate.AddDays(-3);
            }

            // Return the week of our adjusted day

            int i = cal.GetWeekOfYear(calculatedate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return i;
        }

        public static int GetMyWeekNumberStartFrom20000101(DateTime datetimeInput)
        {
            var baseDate = new DateTime(2000, 1, 1);
            var days = (datetimeInput - baseDate).TotalDays;
            var weeks = days/7;
            weeks = Math.Ceiling(weeks);
            return Convert.ToInt32(weeks);
        }


        public static int GetYearNumber()
        {
            int yearNow = DateTime.Today.Year;
            return (yearNow - 2000);
        }

        //Plan Month
        public static int GetWeekNumber(DateTime workingDate)
        {
            DateTime monday = GetCurrentMondayDate(DateTime.Today);
            int i = 0;
            if (workingDate >= monday.AddDays(7) && workingDate < monday.AddDays(14))
            {
                i = 1;
            }
            else if (workingDate >= monday.AddDays(14) && workingDate < monday.AddDays(21))
            {
                i = 2;
            }
            else if (workingDate >= monday.AddDays(21))
            {
                i = 3;
            }
            return i;
        }

        //
        public static int ConvertClockSystemFrom12To24(int hour, string ampm)
        {
            var h = hour;
            ampm = ampm.ToUpper();
            if (ampm == "AM" && hour == 12)
            {
                h = 0;
            }

            if (ampm == "PM" && hour != 12)
            {
                h += 12;
            }


            return h;
        }
    }
}