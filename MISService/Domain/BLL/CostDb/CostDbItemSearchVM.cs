using System;
using System.Collections.Generic;
using System.Linq;
using MyCommon;
using SpecDomain.BLL.Material;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.CostDb
{
    public class CostDbItemSearchVm
    {
        public List<MyKeyValuePair> Values { get; private set; }

        public CostDbItemSearchVm(string q, string typeText, int categoryID0, int categoryID1, int categoryID2, int estItemID)
        {

            var estCostTypeID = Convert.ToInt32(typeText);
            var db = new SpecificationDbEntities();
            var estCostType = db.EST_Cost_Type.Find(estCostTypeID );
            var dbTypeID = estCostType.TemplateDbTypeID;
            IAutoCompleteReponses vm;

            //several estCostType may in one db Type, eg. all labour hours in LabourHourDB
            switch (dbTypeID)
            {
             
                case (int)NTemplateDbTypeID.Labour:
                    vm = new CostDbItemSearchLabourHour(q, estCostTypeID);
                    break;
                case (int)NTemplateDbTypeID.Material:
                    vm = new CostDbItemSearchMaterial(q, categoryID0, categoryID1, categoryID2);
                    break;
                default:
                    if(estCostTypeID==(int)NEstCostType.ShopStandItem )
                    {
                        vm = new CostDbItemSearchStandardItem(q, estItemID);    
                    }
                    else
                    {
                        vm = new CostDbItemSearchOthers(estCostTypeID);    
                    }
                    
                    break;
            }

            Values = vm.Values;
        }
    }

    public class CostDbItemSearchLabourHour : IAutoCompleteReponses
    {
        public List<MyKeyValuePair> Values { get; set; }

        public CostDbItemSearchLabourHour(string q, int estCostTypeID)
        {
            var db = new SpecificationDbEntities();
            var options =db.EST_Cost_Db_LabourHourProcedure.Where(x=> x.EstCostTypeID ==estCostTypeID  ).OrderBy(x => x.Name).ToList();
            if (!MyCommon.MyConvert.IsNullString(q))
            {
                q = q.ToUpper();
                options = options.Where(x => x.Name.ToUpper().Contains(q)).ToList();
            }

            Values = new List<MyKeyValuePair>();
            foreach (var option in options)
            {
                var str = option.EST_Cost_Db_LabourHourPosition.RateB.ToString("C2") + "/" + "Hour";
                var acr = new MyKeyValuePair
                              {
                                  Value = option.Name + "--" + str,
                                  Key = option.DbItemID
                              };
                Values.Add(acr);
            }
        }
    }

    public class CostDbItemSearchOthers : IAutoCompleteReponses
    {
        public List<MyKeyValuePair> Values { get; set; }

        public CostDbItemSearchOthers(int estCostTypeID)
        {
             var db = new SpecificationDbEntities();
            var options = db.EST_Cost_Db_Other.Where(x => x.EstCostTypeID ==estCostTypeID ).ToList().OrderBy(x => x.Name).ToList();

            Values = options.Select(x => new MyKeyValuePair() 
                                             {
                                                 Value = x.Name + "--" + x.UnitPrice.ToString("C2") + "/" + x.Unit,
                                                 Key = x.DbItemID,
                                             }).ToList();

        }
    }

    public class CostDbItemSearchStandardItem : IAutoCompleteReponses
    {
        public List<MyKeyValuePair> Values { get; set; }

        public CostDbItemSearchStandardItem(string q, int estItemID)
        {
            var db = new SpecificationDbEntities();
            var quoteItem = db.EST_Item.Find(estItemID);
            var productID = quoteItem.ProductID;
            var options = db.EST_Cost_Template.Where(x => x.ProductID == productID).ToList();

            if (MyConvert.IsNullString(q))
            {
                Values = options.Select(x => new MyKeyValuePair()
                                                 {
                                                     Value = x.Name+ "--" + 0.ToString("C2") + "/" + "EA",
                                                     Key = x.TemplateID,
                                                 }).ToList();
            }
            else
            {
                var qs = new QueryStringForMaterialSearch(q);
                Values = GetStandardItemsFilteredByQueryString(qs, options);
            }
        }


        private List<MyKeyValuePair> GetStandardItemsFilteredByQueryString(QueryStringForMaterialSearch qString, IEnumerable<EST_Cost_Template > options)
        {
            var values = new List<MyKeyValuePair>();
            foreach (var opt in options)
            {
                var name = opt.Name;
                var b0 = true;
                var b1 = true;
                var b2 = true;

                if (qString.QLength >= 1)
                {
                    if (!name.ToUpper().Contains(qString.Q0))
                    {
                        b0 = false;
                    }
                }
                if (qString.QLength >= 2)
                {
                    if (!name.ToUpper().Contains(qString.Q1))
                    {
                        b1 = false;
                    }
                }
                if (qString.QLength >= 3)
                {
                    if (!name.ToUpper().Contains(qString.Q2))
                    {
                        b2 = false;
                    }
                }
                if (!(b0 & b1 & b2)) continue;

                var value = new MyKeyValuePair()
                {
                    Value = name + "--" + 0.ToString("C2") + "/" + "EA",
                    Key = opt.TemplateID,
                };
                values.Add(value);
            }
            return values;
        }
    }
    
    public class CostDbItemSearchMaterial : IAutoCompleteReponses
    {
        public List<MyKeyValuePair> Values { get; set; }

        public CostDbItemSearchMaterial(string q, int categoryID0, int categoryID1, int categoryID2)
        {
            Values = new List<MyKeyValuePair>();
            if (categoryID0 == 0 && q.Length < 3) return;

            var db = new SpecificationDbEntities();
            var qs = new QueryStringForMaterialSearch(q);
            List<Model.Material> materials;
            if (categoryID0 > 0)
            {
                materials = db.Materials.Where(x => x.CategoryID0 == categoryID0 &&
                                                    (categoryID1 == 0 | x.CategoryID1 == categoryID1) &&
                                                    (categoryID2 == 0 | x.CategoryID2 == categoryID2))
                    .ToList();

            }
            else
            {
                materials = db.Materials.Where(x => x.MaterialCategory1.CategoryName.Contains(qs.Q0)).ToList();

            }

            Values  = GetMaterialsFilteredByQueryString(qs, materials);
        }

         private List<MyKeyValuePair > GetMaterialsFilteredByQueryString(QueryStringForMaterialSearch qString, IEnumerable<Model.Material> materials )
         {
             var values = new List<MyKeyValuePair>();
             foreach (var m in materials)
             {
                 var name = MaterialCommon.GetMaterialName(m);
                 var b0 = true;
                 var b1 = true;
                 var b2 = true;

                 if (qString.QLength >=1)
                 {
                     if (!name.ToUpper().Contains(qString.Q0))
                     {
                         b0 = false;
                     }
                 }
                 if (qString.QLength >= 2)
                 {
                     if (!name.ToUpper().Contains(qString.Q1))
                     {
                         b1 = false;
                     }
                 }
                 if (qString.QLength >= 3)
                 {
                    if (!name.ToUpper().Contains(qString.Q2))
                    {
                        b2 = false;
                    }
                 }
                 if (!(b0 & b1 & b2 )) continue;
                 var value = new MyKeyValuePair()
                                 {
                                     Value = name+ "--" + m.Price.ToString("C2") + "/" + m.MaterialPriceUnit.UnitName ,
                                     Key = m.MaterialID,
                                 };
                 values.Add(value);
             }
             return values;
         }
    }
    
    public class QueryStringForMaterialSearch
    {
        public int QLength { get; set; }
        public string Q0 { get; set; }
        public string Q1 { get; set; }
        public string Q2 { get; set; }

        public QueryStringForMaterialSearch(string q)
        {
            char[] splitter = {' '};
            q = q.Trim();
            q = q.Replace(",", " ");

            var qa = q.Split(splitter);
             Q0 = qa[0].Trim().ToUpper();
             Q1 = "";
             Q2 = "";
             QLength = qa.Length;
            if (QLength >= 2)
            {
                Q1 = qa[1].Trim().ToUpper();
            }
            if (QLength >= 3)
            {
                Q2 = qa[2].Trim().ToUpper();
            }
        }

    }

    
}
