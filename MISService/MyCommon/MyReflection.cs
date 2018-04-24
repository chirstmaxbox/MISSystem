using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyCommon
{
    public class MyReflection
    {
       public static  void Copy( object src, object dest)
       {
           var allowCopiedTypeName = new string[] {"INT64", "INT32","INT16", "STRING", "INT", "BOOL","BOOLEAN", "DOUBLE", "DATETIME", "DECIMAL", "BYTE" };      

            var srcProperties = src.GetType().GetProperties(); //PropertyInfo
            
        
            foreach (var srcProperty in srcProperties)
            {
            //debug
                var dstProperty = dest.GetType().GetProperty(srcProperty.Name);
                if (dstProperty == null) continue;

                //if (srcProperty.Name =="erRev")
                //{
                //}
                var pTypeName = GetCoreType(dstProperty.PropertyType).Name.ToUpper();

                if (allowCopiedTypeName.Contains(pTypeName))
                {
                    if (dstProperty.PropertyType.IsAssignableFrom(srcProperty.PropertyType))
                    {
                        dstProperty.SetValue(dest, srcProperty.GetValue(src, null), null);
                    }
                }
            }
        }



       private static Type GetCoreType(Type type)
       {
           if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
               return Nullable.GetUnderlyingType(type);
           else
               return type;
       }

        public static void SetPropertyValue( object srcObject, string propertyName, string value) 
        { 
            var vProperty = srcObject.GetType().GetProperty(propertyName);
            if (vProperty == null) return;
            vProperty.SetValue(srcObject, value, null);            
        }
        public static void SetPropertyValue(object srcObject, string propertyName, int value)
        {
            var vProperty = srcObject.GetType().GetProperty(propertyName);
            if (vProperty == null) return;
            vProperty.SetValue(srcObject, value, null);
        }

        public static object GetPropertyValue(object srcObject, string propertyName)
        {
            var vProperty = srcObject.GetType().GetProperty(propertyName,
                                                            BindingFlags.Public | BindingFlags.Instance |
                                                            BindingFlags.Static);
            return vProperty.GetValue(srcObject, null);
        }


        #region ************** Obselete ********************
        /// <summary>
        /// Generic Dynamic Sorting mechanism
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="genericList"> List of Type </param>
        /// <param name="sortExpression" > Class.Property</param>
        /// <param name="sortDirection"> "asc" | "desc");</param>
        /// <returns></returns>
        public static List<T> MyDynamicSort<T>(List<T> genericList, string sortExpression, string sortDirection)
        {
            var sortReverser = sortDirection.ToLower().StartsWith("asc") ? 1 : -1;

            var comparisonDelegate = new Comparison<T>(delegate(T x, T y)
            {
                //Just to get the compare method info to compare the values.
                MethodInfo compareToMethod = GetCompareToMethod<T>(x, sortExpression);

                //Getting current object value.
                object xSortExpressionValue = x.GetType().GetProperty(sortExpression).GetValue(x, null);

                //Getting the previous value. 
                object ySortExpressionValue = y.GetType().GetProperty(sortExpression).GetValue(y, null);

                //Comparing the current and next object value of collection.
                object result = compareToMethod.Invoke(xSortExpressionValue, new object[] { ySortExpressionValue });

                // result tells whether the compared object is equal,greater,lesser.
                return sortReverser * Convert.ToInt16(result);
            });

            //here we using the comparison delegate to sort the object by its property
            genericList.Sort(comparisonDelegate);

            return genericList;
        }

        /// <summary>
        /// Used to get method information using refection
        /// </summary>
        private static MethodInfo GetCompareToMethod<T>(T genericInstance, string sortExpression)
        {
            Type genericType = genericInstance.GetType();
            object sortExpressionValue = genericType.GetProperty(sortExpression).GetValue(genericInstance, null);
            Type sortExpressionType = sortExpressionValue.GetType();
            MethodInfo compareToMethodOfSortExpressionType = sortExpressionType.GetMethod("CompareTo", new Type[] { sortExpressionType });

            return compareToMethodOfSortExpressionType;
        }


        //public EstItemIndexVm(int estRevID, int selectedEstItemRow, string sort, string sortDirection)
        //{
        //    var originalEstItems = _db.EST_Item.Where(x => x.EstRevID == estRevID).ToList();
        //    var propInfo = typeof(EST_Item).GetProperty(sort);
        //    originalEstItems =originalEstItems.OrderBy(x=>propInfo.GetValue(x,null)).ThenBy(x=>x.EstItemID).ToList();

        //    if (sortDirection.ToUpper() =="DESC")
        //    {
        //        originalEstItems =originalEstItems.OrderByDescending(x=>propInfo.GetValue(x,null)).ThenBy(x=>x.EstItemID).ToList();
        //    }

        //    var item = originalEstItems[selectedEstItemRow-1];
        //    SelectedEstItemID = item.EstItemID;
        //}
        #endregion
    }
}