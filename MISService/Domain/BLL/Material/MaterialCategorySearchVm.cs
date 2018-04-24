using System;
using System.Collections.Generic;
using System.Linq;
using MyCommon;
using SpecDomain.Model;

namespace SpecDomain.BLL.Material
{
    public class MaterialCategorySearchVm
    {
   //    private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        public List<MyKeyValuePair> Values { get; private set; }
        public MaterialCategorySearchVm(string q, string typeText, string parentID, string parentText)
        {
            var pID =MyConvert.ConvertToInteger( parentID);
            IAutoCompleteReponses vm;
            var typeID  = Convert.ToInt32(typeText);

            switch(typeID)
            {
                case 0:
                    vm = new MaterialCategoryVm0();
                    break;
                case 1:
                    vm = new MaterialCategoryVm1(pID, parentText);
                    break;
                case 2:
                    vm = new MaterialCategoryVm2(pID, parentText);
                    break;
                default :
                    vm = new MaterialCategoryVm3(pID, parentText);
                    break;
                 }

            Values = vm.Values;
        }

 }

    public class MaterialCategoryVm0 : IAutoCompleteReponses
    {
        public List<MyKeyValuePair> Values { get; set; }

        public MaterialCategoryVm0()
        {
            var db = new SpecificationDbEntities();
            var options =db.MaterialCategory0.Where(x => x.CategoryID >= 0).ToList().OrderBy(x => x.CategoryName).ToList();
            Values = options.Select(x => new MyKeyValuePair
                                             {
                                                Value = x.CategoryName,
                                                Key = x.CategoryID,
                                             }
                ).ToList();
        }
    }

    public class MaterialCategoryVm1 : IAutoCompleteReponses
    {
        public List<MyKeyValuePair> Values { get; set; }

        public MaterialCategoryVm1(int parentID, string parentText)
        {
            var db = new SpecificationDbEntities();
            //if this Parent exist
            var parent = db.MaterialCategory0.Find(parentID);
            var options = db.MaterialCategory1.Where(x => x.CategoryID == 0).ToList();
            if (MyString.RemoveSpace(parent.CategoryName)==MyString .RemoveSpace( parentText ))
            {
                options = db.MaterialCategory1.Where(x => x.ParentID  == parentID || x.CategoryID == 0)
                                                 .OrderBy(x => x.CategoryName)
                                                 .ToList();                
            }
            //Name="Choose"to ""
            var opt0 = options.Find(x => x.CategoryID == 0);
            opt0.CategoryName = "";
            //Result
            Values = options.Select(x => new MyKeyValuePair
                                             {
                                                 Value  = x.CategoryName,
                                                 Key = x.CategoryID,
                                             }
                ).ToList();
        }
    }

    public class MaterialCategoryVm2 : IAutoCompleteReponses
    {
        public List<MyKeyValuePair> Values { get; set; }

        public MaterialCategoryVm2(int parentID, string parentText)
        {
            var db = new SpecificationDbEntities();
            //if this Parent exist
            var parent = db.MaterialCategory1.Find(parentID);
            var options = db.MaterialCategory2.Where(x => x.CategoryID == 0).ToList();
            if (MyString.RemoveSpace(parent.CategoryName) == MyString.RemoveSpace(parentText))
            {
                options = db.MaterialCategory2.Where(x => x.ParentID  == parentID || x.CategoryID == 0)
                                                 .OrderBy(x => x.CategoryName)
                                                 .ToList();
            }

            var opt0 = options.Find(x => x.CategoryID == 0);
            opt0.CategoryName = "";
            Values = options.Select(x => new MyKeyValuePair
                                             {
                                               Value = x.CategoryName,
                                               Key = x.CategoryID,
                                             }
                                    ).ToList();
        }
    }

    public class MaterialCategoryVm3 : IAutoCompleteReponses
    {
        public List<MyKeyValuePair> Values { get; set; }

        public MaterialCategoryVm3(int parentID, string parentText)
        {
            var db = new SpecificationDbEntities();
            //if this Parent exist
            var parent = db.MaterialCategory2.Find(parentID);                                               //Diff
            var options = db.MaterialCategory3.Where(x => x.CategoryID == 0).ToList();                      //Diff
            if (MyString.RemoveSpace(parent.CategoryName) == MyString.RemoveSpace(parentText))
            {
                options = db.MaterialCategory3.Where(x => x.ParentID == parentID || x.CategoryID == 0)      //Diff
                                                 .OrderBy(x => x.CategoryName)
                                                 .ToList();
            }

            var opt0 = options.Find(x => x.CategoryID == 0);
            opt0.CategoryName = "";
            Values = options.Select(x => new MyKeyValuePair
            {
                Value = x.CategoryName,
                Key = x.CategoryID,
            }
                                    ).ToList();
        }
    }


}
