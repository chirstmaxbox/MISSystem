using System;
using System.Collections.Generic;

namespace MyCommon
{
    public class MyString
    {
        public static string Left(string param, int length)
        {
            //we start at 0 since we want to get the characters starting from the left 
            //and with the specified lenght and assign it to a variable
            string result = param.Substring(0, length);
            //return the result of the operation
            return result;
        }

        public static string Right(string param, int length)
        {
            //start at the index based on the lenght of the sting minus the specified lenght and assign it a variable1
            string result = param.Substring(param.Length - length, length);
            //return the result of the operation
            return result;
        }

        public static string ConvertArrayToString(string[] a1)
        {
            //return str=a1(0)+a1(1)+a1(2)....

            string s1 = "";

            int count = a1.Length;

            for (int i = 0; i <= count - 1; i++)
            {
                if (!MyConvert.IsNullString(a1[i]))
                {
                    //First Row
                    if (i == 0)
                    {
                        s1 = a1[i];
                    }
                    else
                    {
                        s1 += ", " + a1[i];
                    }
                }
            }

            return s1;
        }

        public static string FixCrLf(object value)
        {
            if (Convert.IsDBNull(value))
            {
                return null;
            }
            else if (String.IsNullOrEmpty(Convert.ToString(value)))
            {
                return String.Empty;
            }
            else
            {
                String s = Convert.ToString(value);
                return s.Replace(Environment.NewLine, "<br />");
            }
        }

        public static bool IsStringLengthLongerThan(int stringLength, object compareString)
        {
            bool b = true;
            if (MyConvert.IsNullString(compareString))
            {
                b = false;
            }
            else
            {
                string s = Convert.ToString(compareString);
                if (s.Length < stringLength)
                {
                    b = false;
                }
            }
            return b;
        }

        public static bool IsContainsAny(string str, params string[] values)
        {
            var isContains = false;

            if (!string.IsNullOrEmpty(str) && values.Length > 0)
            {


                foreach (var value in values)
                {
                    if (str.ToUpper().Contains(value.ToUpper()))
                    {
                        isContains = true;
                    }
                    else
                    {
                        isContains = false;
                        break;
                    }
                }

            }

            return isContains;

        }

        public static List<int> ConvertStringToIntList(string stringToBeConverted, string prefix)
        {
            var separator1 = new char[] { ',' };
            var strArray = stringToBeConverted.Split(separator1);

            var l1 = new List<int>();
            var count = strArray.Length;

            for (int i = 0; i < count; i++)
            {
                if (strArray[i].Contains(prefix))
                {
                    var s1 = strArray[i].Replace(prefix, "").Trim();
                    l1.Add(Convert.ToInt32(s1));
                }
            }

            return l1;
        }

        public static List<int> ConvertStringToIntList(string stringToBeConverted)
        {
            var separator1 = new char[] { ',' };
            var strArray = stringToBeConverted.Split(separator1);

            var l1 = new List<int>();
            var count = strArray.Length;

            for (int i = 0; i < count; i++)
            {
                var s1 = strArray[i].Trim();
                l1.Add(Convert.ToInt32(s1));
            }

            return l1;
        }

        public static string ConvertIntListToString(List<int> a1)
        {
            var a2 = new string[a1.Count];
            int i = 0;
            foreach (var j in a1)
            {
                a2[i++] = Convert.ToString(j);

            }

            return ConvertArrayToString(a2);

        }


        public static string RemoveSpace(object str1)
        {
            //Remove All Space
            var v1 = "";

            if (str1 != null)
            {
                string s1 = Convert.ToString(str1);
                if (!string.IsNullOrEmpty(s1))
                {
                    if (s1.Length > 0        )
                    {
                        v1 = s1.Replace(" ", "");
                    }
                }
            }
            return v1;
        }

        public static string RemoveLastCharIfIsCommaFromString(object obj)
        {
            if (MyConvert.IsNullString(obj)) return "";
            string str = obj.ToString();
            char last = str[str.Length - 1];
            string str2 = ",";
            char last2 = str2[0];
            if (last != last2) return str;

            return str.Remove(str.Length - 1, 1);
        }


        public static string UppercaseFirstChar(string str)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(str)) return "";

            // Only Char
            if (str.Length == 1) return str.ToUpper();

            // Return char and concat substring.
            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }
    }
}