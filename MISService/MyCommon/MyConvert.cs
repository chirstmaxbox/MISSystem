using System;
using System.Data.SqlClient;

namespace MyCommon
{
    public class MyConvert
    {
        public static bool IsNumeric(Object str)
        {
            bool b = false;
            if (str != null)
            {
                if (!Convert.IsDBNull(str))
                {
                    String s = Convert.ToString(str);
                    Double number;
                    b = Double.TryParse(s, out number);
                }
            }
            return b;
        }

        public static bool IsNullString(object str)
        {
            bool b = true;
            if (str != null)
            {
                string s1 = Convert.ToString(str);

                if (!Convert.IsDBNull(s1))
                {
                    string s2 = Convert.ToString(s1).Trim();
                    if (!string.IsNullOrEmpty(s2) && s2.Length > 0 && "&nbsp;" != s2)
                    {
                        b = false;
                    }
                }
            }
            return b;
        }

        public static bool IsDate(Object str)
        {
            bool b = false;
            if (str != null)
            {
                if (!Convert.IsDBNull(str))
                {
                    String s = Convert.ToString(str);
                    DateTime date;
                    b = DateTime.TryParse(s, out date);
                    if (date.Year <1900)
                    {
                        b = false;
                    }
                }
            }
            return b;
        }

        public static bool ConvertToBool(object str)
        {
            if (str == null) return false;
            if (Convert.IsDBNull(str)) return false;
            var s = Convert.ToString(str);
            return s.ToUpper() == "TRUE";
        }


        public static DateTime ConvertToDate(Object str)
        {
            DateTime dt = DateTime.Today;
            if (str != null)
            {
                if (!Convert.IsDBNull(str))
                {
                    String s = Convert.ToString(str);
                    DateTime date;
                    bool b = DateTime.TryParse(s, out date);
                    if (b)
                    {
                        dt = date;
                    }
                }
            }
            return dt;
        }


        public static string ConvertNullableDateToString(Object str)
        {
            var s = "";
            if (IsDate(str))
            {
                var d1 = Convert.ToDateTime(str);
                s = d1.ToString("MMM dd, yyyy hh:mm tt");
            }
            return s;
        }


        public static string ConvertNullableDateToUniversal(Object str)
        {
            var s = "";
            if (!IsDate(str)) return s;
            var d1 = Convert.ToDateTime(str);
            s = d1.ToString("u");
            return s;
        }

        public static string ConvertNullableDateToRFC1123(Object str)
        {
            var s = "";
            if (!IsDate(str)) return s;
            var d1 = Convert.ToDateTime(str);
            s = d1.ToString("r");
            return s;
        }


        public static decimal ConvertToDecimal(object str)
        {
            decimal i = 0;
            if (str == null) return i;
            var s = Convert.ToString(str);
            if (Convert.IsDBNull(str)) return i;

            decimal number;
            var b = decimal.TryParse(s, out number);
            if (b)
            {
                i = number;
            }
            return i;
        }

 
        public static int ConvertToInteger(object str)
        {
            int i = 0;
            if (str != null)
            {
                var s = Convert.ToString(str);
                if (!Convert.IsDBNull(str))
                {
                    int number;
                    var b = int.TryParse(s, out number);
                    if (b)
                    {
                        i = number;
                    }
                }
            }
            return i;
        }

        public static long ConvertToLong(object str)
        {
            long i = 0;
            if (str != null)
            {
                String s = Convert.ToString(str);
                if (!Convert.IsDBNull(str))
                {
                    long  number;
                    var b = long.TryParse(s, out number);
                    if (b)
                    {
                        i = number;
                    }
                }
            }
            return i;
        }

        public static double ConvertToDouble(object str)
        {
            double d = 0;
            if (str != null)
            {
                string s = Convert.ToString(str);

                if (!Convert.IsDBNull(s))
                {
                    Double number;
                    Boolean b = Double.TryParse(s, out number);
                    if (b)
                    {
                        d = number;
                    }
                }
            }
            return d;
        }

        public static double ConvertToDouble(object str, int decimals)
        {
            double d = 0;
            if (str != null)
            {
                string s = Convert.ToString(str);

                if (!Convert.IsDBNull(s))
                {
                    Double number;
                    Boolean b = Double.TryParse(s, out number);
                    if (b)
                    {
                        d = number;
                    }
                }
            }

            return Math.Round(d, decimals);
        }

        public static string ConvertToString(object str)
        {
            string s = "";
            if (str != null)
            {
                string s1 = Convert.ToString(str);
                if (!Convert.IsDBNull(str))
                {
                    string temp = Convert.ToString(str);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        s = temp.Trim();
                    }
                }
            }
            return s;
        }


        public static string RemoveAccountingFormatCharacter(object str)
        {
            if (IsNullString(str)) return "0";
            string s1 = ConvertToString(str);
            if (s1.Length == 0) return "0";

            string s2 = s1.Substring(0, 1);
            if (s2 == "$")
            {
                s1 = s1.Remove(0, 1);
            }
            while (s1.Contains(","))
            {
                int int1 = s1.IndexOf(",");
                s1 = s1.Remove(int1, 1);
            }

            return s1;
        }

        public static double ConvertAccountingFormatStringToDouble(object  str)
        {
            var s1 = RemoveAccountingFormatCharacter(str);
            return ConvertToDouble(s1);
        }

        public static Boolean IsAccountingFormatNumberic(object str)
        {
            var s1 = RemoveAccountingFormatCharacter(str);
            return IsNumeric(s1);
        }

 
        public static double ConvertPercentageToDouble(object str )
        {
            //Input:  123.45%
            if (IsNullString(str)) return 0;
            var s1 = ConvertToString(str);
            if (s1.Length == 0) return 0;

            double av = 0;
            try
            {
                if (!(s1 == string.Empty | s1 == "&nbsp;"))
                {
                    if (s1.Contains("%"))
                    {
                        int int1 = s1.IndexOf("%");
                        s1 = s1.Remove(int1, 1);
                    }
                    av = System.Convert.ToDouble(s1);
                    av = Math.Round(av / 100, 2);
                }


            }
            catch (SqlException ex)
            {
                //LogError(ex.Message);
            }

            return av;
        }

        public static string ConvertDoubleToPercentage(object percentage)
        {
            var d1 = MyConvert.ConvertToDouble(percentage);
            if (Math.Abs(d1 - 0) < 0.0001) return "0.0%";
            d1 = Math.Round(d1*100, 2);
            return string.Format("{0:0}", d1)+ "%";
        }


        

  
//This function converts a string like 5'-6 1/4" to a decimal number of inches that can be used in calculation.
        public static double ConvertToInches(string toBeConvertedString)
        {

            return ConvertToDouble(toBeConvertedString);


            if (IsNullString(toBeConvertedString)) return 0;

            //These values are used to examine the input string, one character at a time
            string vVal = null;
            //shorter name for input string
            int i = 0;
            //counter to step through each character in input string
            //temporary storage of each input string character

            //These variables hold the values we discover for feet, inches and the numerator and denominator of the fractional inches
            int iFt = 0;
            //used to store number of feet
            int iIn = 0;
            //number of inches
            int iNumerator = 0;
            //numerator of fractional inches
            int iDenominator = 0;
            //denominator of fractional inches

            //In the process of discovering values for feet and inches, these variable are used to accumulate and hold numeric values
            int iTemp = 0;
            //Used to build a number as each digit is read

            //We want to ignore spaces, except for the very important space betweenthe number of inches and the numerator of the fractional inches
            //This variable is true if the last character processed was a space
            bool bLastCharWasSpace = false;

            //First we assign input string to variable with shorter name
            vVal = toBeConvertedString;

            //If input string is numeric, then we don't want to convert it
            if (IsNumeric(vVal))
            {
                return Convert.ToDouble(vVal);
            }

            //Now we step through each character in input string from left to right
            iTemp = 0;

            for (i = 0; i < vVal.Length; i++)
            {
                var vChar = vVal.Substring(i, 1);

                //If character is a number, 
                //then we combine it with numbers before it to get a number that we can assign to feet, inches, numerator or denominator
                if (IsNumeric(vChar))
                {
                    //If this is a number and the last character was a space 
                    //then chances are, the number in iTemp is inches and we need to start over building the numerator of fractional inches
                    if (bLastCharWasSpace == true & iIn == 0)
                    {
                        iIn = iTemp;
                        iTemp = 0;
                    }

                    //As we read number from left to right, we multiply value of previous number (if any) by 10 and add this number
                    if (iTemp == 0)
                    {
                        iTemp = Convert.ToInt32(vChar);
                    }
                    else
                    {
                        iTemp = iTemp*10;
                        iTemp = iTemp + Convert.ToInt32(vChar);
                    }

                    //The number we've been buiding must be feet
                }
                else if (vChar == "'" | vChar == "f")
                {
                    iFt = iTemp;
                    iTemp = 0;

                    //The number we've been bulding must be the numerator of fraction
                }
                else if (vChar == "/")
                {
                    iNumerator = iTemp;
                    iTemp = 0;

                    //The number we've been building must be inches or
                    //the denominator of the fraction, so we check to see if
                    //there is a numerator
                }
                else if (vChar == "\"" | vChar == "i")
                {
                    if (iNumerator > 0)
                    {
                        iDenominator = iTemp;
                        iTemp = 0;
                        //If no numerator, then the number must be inches
                    }
                    else if (iIn == 0)
                    {
                        iIn = iTemp;
                        iTemp = 0;
                    }
                }

                //Now we set our indicator so that when we process the next
                //character, we will know if the last character was a space
                if (vChar == " ")
                {
                    bLastCharWasSpace = true;
                }
                else
                {
                    bLastCharWasSpace = false;
                }
            }

            //To avoid dividing by zero if there was no numerator for fraction,
            //we set denominator to 1
            if (iDenominator == 0)
                iDenominator = 1;

            //Finally, we calculate number of decimal inches and return value
            double cInches = (iFt*12) + iIn + (iNumerator/iDenominator);
            return cInches;
        }


        


    }
}