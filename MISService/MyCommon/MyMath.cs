using System;

namespace MyCommon
{
    public class MyMath
    {
        public static double GetMinimumValue(double minimumValue, double originalValue)
        {
            return originalValue < minimumValue ? minimumValue : originalValue;
        }


        public static string GetAccuracy(double est, double act)
        {
            //
            if (Math.Abs(est - 0) < 0.001) return "NA";

            var d1 = est/act;
            if (est < act)
            {
                d1 = (est - act)/act;
            }

            d1 = Math.Round(d1*100, 2);
            return string.Format("{0:0}", d1) + "%";

        }


        public static string GetDirectRatio(double dividend, double divisor)
        {
            // dividend, b is called the divisor, and c is called the quotient
            if (Math.Abs(divisor) < 0.01) return "NA";
            double d1 = Math.Round(dividend / divisor * 100,  2);
            return string.Format("{0:0}", d1) + "%";
            
        }



        public static string GetVariance(double baseNumber, double varianceNumber)
        {
            //
            if (Math.Abs(baseNumber - 0) < 0.01) return "NA";

            var d1 = (varianceNumber -baseNumber) /baseNumber  ;

            if (Math.Abs(d1) > 10) return ">1000%";
            
            d1 = Math.Round(d1*100, 2);
            return string.Format("{0:0}", d1) + "%";

        }

        public static string GetVarianceNoAbs(object baseTotal, object compareTotal)
        {
            double baseTotalAmount = MyConvert.ConvertToDouble(baseTotal);
            double compareTotalAmount = MyConvert.ConvertToDouble(compareTotal);
            if (baseTotalAmount < 0.1) return "NA";
            double v1 = (baseTotalAmount - compareTotalAmount) / baseTotalAmount;
            if (v1 <= -10.00) return "<-1000%";
            return MyConvert.ConvertDoubleToPercentage(v1);
        }


        public static string GetNagativeVariance(double baseNumber, double varianceNumber)
        {
            //
            if (Math.Abs(baseNumber - 0) < 0.01) return "NA";

            var d1 = (varianceNumber - baseNumber) / baseNumber;

            if (Math.Abs(d1) > 10) return ">1000%";

            d1 = -Math.Round(d1 * 100, 2);

            return string.Format("{0:0}", d1) + "%";

        }
        
    }
}