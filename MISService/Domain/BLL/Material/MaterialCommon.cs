using System.Collections.Generic;
using System.Linq;
using MyCommon;

namespace SpecDomain.BLL.Material
{
    public class MaterialCommon
    {
        public static  string GetMaterialName(Model.Material material)
        {
            return material.MaterialCategory1.CategoryName + " " +
                   (material.CategoryID2 == 0 ? "" : material.MaterialCategory2.CategoryName + " ") +
                   (material.CategoryID3 == 0 ? "" : material.MaterialCategory3.CategoryName + " ") +
                   (material.CategoryID4 == 0 ? "" : material.MaterialCategory4.CategoryName);
        }


        public static string GetMaterialPrice(Model.Material material)
        {
            var currentPrice = string.Format("{0:C2}", material.Price);
            var unitName = material.MaterialPriceUnit.UnitName;
            return currentPrice + "/" + unitName;
        }


        public static string GetCategory3Name(Model.Material material)
        {
            return
                (material.CategoryID3 == 0 ? "" : material.MaterialCategory3.CategoryName + " ") +
                (material.CategoryID4 == 0 ? "" : material.MaterialCategory4.CategoryName);
        }

        public static string GetMaterialLabel(Model.Material material)
        {
            var materialPrice = material.MaterialPrices.FirstOrDefault(x => x.Active);
            double price = 0;
            string unitName = "na";
            if (materialPrice != null)
            {
                price = MyConvert.ConvertToDouble(materialPrice.Price);
                unitName = materialPrice.MaterialPriceUnit.UnitName;
            }
            var s = GetMaterialName(material) +
                    "--" +
                    string.Format("{0:C2}", price) +
                    "/" +
                    unitName;
            return s;
        }

        public string[] GetQueryString(string q)
        {
            char[] splitter = { ' ' };
            q = q.Trim();
            q = q.Replace(",", " ");
            return q.Split(splitter);
        }

    }
}