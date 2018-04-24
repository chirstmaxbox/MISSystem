using System;
using System.Net.Mime;

namespace MyCommon
{
    public class MyDateTimePayroll
    {
        public static DateTime GetCurrentPayrollStartDate()
        {
            var d1 = new DateTime(2007, 12, 31);
            var d2 = DateTime.Today;
            var d3 = new DateTime(2099, 12, 31);
            while (d1 < d3)
            {
                if (d2 >= d1 & d2 < d1.AddDays(14))
                {
                    break; //
                }
                d1 = d1.AddDays(14);
            }
            return d1;
        }


        public static int GetPayrollDateIndex(DateTime d2)
        {
            d2 = MyDateTime.GetCurrentMondayDate(d2);
            var d1 = new DateTime(2007, 12, 31);
            int i = 0;
            var d3 = new DateTime(2099, 1, 1);
            while (d1 < d3)
            {
                if (d2 >= d1 & d2 < d1.AddDays(14))
                {
                    break;
                }
                d1 = d1.AddDays(14);
                i += 1;
            }
            return i;
        }

    }


//public static void FilloutPayrollDropdownint getPayrollDateIndex()
//{
//    System.DateTime d1 = 31/12/2007 00:00:00;
//    int i = 0;
//    System.DateTime d2 = System.DateTime.Today;
//    while (d1 < 01/01/2018 00:00:00) {
//        if (d2 >= d1 & d2 < d1.AddDays(14)) {
//            break; // TODO: might not be correct. Was : Exit While
//        }
//        d1 = d1.AddDays(14);
//        i += 1;
//    }
//    return i;
//}

//public static string GetDisplayedDate(DropDownList ddl)
//{
//    string dt = "";
//    int itemCount = ddl.Items.Count;
//    int i = ddl.SelectedIndex;
//    if (i <= itemCount - 1) {
//        dt = ddl.Items(i + 1).Text;
//    } else {
//        dt = ddl.Items(i).MediaTypeNames.Text;
//    }

//    System.DateTime et = Convert.ToDateTime(dt);
//    return et.ToLongDateString();
//}

}