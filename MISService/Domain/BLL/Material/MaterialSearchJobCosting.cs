using System;
using System.Collections.Generic;

using System.Linq;
using System.Web.Mvc;
using MyCommon;
using SpecDomain.Model;

namespace SpecDomain.BLL.Material
{
  public class MaterialSearchJobCosting
    {
      private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public List<SelectListItem> Values { get; set; }

        public MaterialSearchJobCosting(int categoryID0, int categoryID1, int categoryID2)
        {
            var materials = _db.Materials.Where(x => categoryID0 > 0 &&
                                                    categoryID0 == x.CategoryID0 &&
                                                    (x.CategoryID1 == categoryID1 && categoryID1>0) &&
                                                    (categoryID2 == 0 || x.CategoryID2 == categoryID2) 
                ).OrderBy(x => x.MaterialCategory1.CategoryName)
                .ThenBy(x => x.MaterialCategory2.CategoryName)
                .ThenBy(x => x.MaterialCategory3.CategoryName)
                .ThenBy(x => x.MaterialCategory4.CategoryName)
                .ToList();

            Values = new List<SelectListItem>();
            foreach (var material in materials)
            {
                var mb = new SelectListItem();
                mb.Value = material.MaterialID.ToString("");
                mb.Text = MaterialCommon.GetMaterialName(material);
                Values.Add(mb);
            }

        }


        public MaterialSearchJobCosting(int transactionID, int categoryID2)
        {
            var transaction = _db.JobCostingTransactions.Find(transactionID);
            var materialID = transaction.MaterialID;
            var filter = _db.Materials.Find(materialID);

            var materials = _db.Materials.Where(x => filter.CategoryID0 > 0 &&
                                                    filter.CategoryID0 == x.CategoryID0 &&
                                                    (x.CategoryID1 == filter.CategoryID1 && filter.CategoryID1 > 0) &&
                                                    (categoryID2 == 0 || x.CategoryID2 == categoryID2)
                ).OrderBy(x => x.MaterialCategory1.CategoryName)
                .ThenBy(x => x.MaterialCategory2.CategoryName)
                .ThenBy(x => x.MaterialCategory3.CategoryName)
                .ThenBy(x => x.MaterialCategory4.CategoryName)
                .ToList();

            Values = new List<SelectListItem>();
            foreach (var material in materials)
            {
                var mb = new SelectListItem();
                mb.Value = material.MaterialID.ToString("");
                mb.Text =MaterialCommon.GetCategory3Name(material);
                Values.Add(mb);
            }

        }
    }

  

}