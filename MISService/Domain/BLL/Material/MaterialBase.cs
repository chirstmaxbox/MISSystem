using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyCommon;
using SpecDomain.Model;

namespace SpecDomain.BLL.Material
{
  public class MaterialBase: SpecDomain.Model.Material 
    {
      //Response
      public bool IsNewCategoryCreated { get; set; }

      //Show on index
        public string  CategoryName0 { get; set; }
        public string  CategoryName1 { get; set; }
        public string  CategoryName2 { get; set; }
        public string  CategoryName3 { get; set; }
        public string  CategoryName4 { get; set; }

        public string CurrentPrice { get; set; }
        public string UnitName { get; set; }
        public string MaterialName { get; set; }

        public string SyncMethodName { get; set; }


      //Edit
        public IEnumerable<SelectListItem> Categories0 { get; set; }
        public IEnumerable<SelectListItem> Categories1 { get; set; }
        public IEnumerable<SelectListItem> Categories2 { get; set; }
        public IEnumerable<SelectListItem> Categories3 { get; set; }
        public IEnumerable<SelectListItem> Categories4 { get; set; }
        public IEnumerable<SelectListItem> Units { get; set; }

        private SpecificationDbEntities _db = new SpecificationDbEntities();
 
        public MaterialBase (SpecDomain.Model.Material material )
        {
            MyReflection.Copy(material, this);
            InitializeNames(material);
            InitializePrice(material);
        }
        
        private void InitializeNames(SpecDomain.Model.Material material)
        {
            CategoryName0 = CategoryID0 == 0 ? "" : material.MaterialCategory0.CategoryName;
            CategoryName1 = CategoryID1 == 0 ? "" : material.MaterialCategory1.CategoryName;
            CategoryName2 = CategoryID2 == 0 ? "" : material.MaterialCategory2.CategoryName;
            CategoryName3 = CategoryID3 == 0 ? "" : material.MaterialCategory3.CategoryName;
            CategoryName4 = CategoryID4 == 0 ? "" : material.MaterialCategory4.CategoryName;
            MaterialName = CategoryName1 + " " + CategoryName2 + " " + CategoryName3 + " " + CategoryName4;
        }

        private void InitializePrice(SpecDomain.Model.Material material)
        {
            CurrentPrice  =string .Format("{0:C2}", material .Price );
            UnitName = material.MaterialPriceUnit.UnitName;
        }

        public MaterialBase(int materialID)
        {
            var material = _db.Materials.Find(materialID);
            MyReflection.Copy(material, this);
            InitializeNames(material);
            InitializePrice(material);
        }
        
        public MaterialBase()
        {
        }
       
        public void RefreshNames()
        {
            //IDs Changed
            _db = new SpecificationDbEntities();
            var cat0 = _db.MaterialCategory0.Find(CategoryID0);
            CategoryName0 = CategoryID0 == 0 ? "" : cat0.CategoryName;

            var cat1 = _db.MaterialCategory1.Find(CategoryID1);
            CategoryName1 = CategoryID1 == 0 ? "" : cat1.CategoryName;

            var cat2 = _db.MaterialCategory2.Find(CategoryID2);
            CategoryName2 = CategoryID2 == 0 ? "" : cat2.CategoryName;

            var cat3 = _db.MaterialCategory3.Find(CategoryID3);
            CategoryName4 = CategoryID3 == 0 ? "" : cat3.CategoryName;

            var cat4 = _db.MaterialCategory4.Find(CategoryID4);
            CategoryName4 = CategoryID4 == 0 ? "" : cat4.CategoryName;

        }

        public void RefreshDropdownlist()
        {
            var temp0 = _db.MaterialCategory0.Where(x => x.CategoryID >= 0)
                .OrderBy(x => x.CategoryName)
                .ToList();
            Categories0 = temp0.Select(x => new SelectListItem
                                                {
                                                    Value = Convert.ToString(x.CategoryID),
                                                    Text = x.CategoryName
                                                });

            var temp1 = _db.MaterialCategory1.Where(x => x.ParentID == CategoryID0 && CategoryID0>0 || x.CategoryID == 0)
                .OrderBy(x => x.CategoryName)
                .ToList();
            Categories1 = temp1.Select(x => new SelectListItem
                                                {
                                                    Value = Convert.ToString(x.CategoryID),
                                                    Text = x.CategoryName
                                                });

            var temp2 = _db.MaterialCategory2.Where(x => x.ParentID == CategoryID1 && CategoryID1 > 0 || x.CategoryID == 0)
                .OrderBy(x => x.CategoryName)
                .ToList();
            Categories2 = temp2.Select(x => new SelectListItem
                                                {
                                                    Value = Convert.ToString(x.CategoryID),
                                                    Text = x.CategoryName
                                                });

            var temp3 = _db.MaterialCategory3.Where(x => x.ParentID == CategoryID2 && CategoryID2 > 0 || x.CategoryID == 0)
                .OrderBy(x => x.CategoryName)
                .ToList();
            Categories3 = temp3.Select(x => new SelectListItem
                                                {
                                                    Value = Convert.ToString(x.CategoryID),
                                                    Text = x.CategoryName
                                                });


            var temp4 =
                _db.MaterialCategory4.Where(x => x.ParentID == CategoryID3 && x.ParentID > 0 && CategoryID3 > 0 || x.CategoryID == 0)
                    .OrderBy(x => x.CategoryName)
                    .ToList();
            Categories4 = temp4.Select(x => new SelectListItem
                                                {
                                                    Value = Convert.ToString(x.CategoryID),
                                                    Text = x.CategoryName
                                                });

            var temp10 = _db.MaterialPriceUnits.Where(x => x.UnitID >= 0).OrderBy(x => x.UnitName).ToList();
            Units = temp10.Select(x => new SelectListItem
                                           {
                                               Value = Convert.ToString(x.UnitID),
                                               Text = x.UnitName
                                           });
        }


        public void RefreshMaterialID()
        {
            MaterialID = 0;
            var material = _db.Materials.FirstOrDefault(x => CategoryID0 > 0 && x.CategoryID0 == CategoryID0 &&
                                                     (CategoryID1 == 0 || x.CategoryID1 == CategoryID1) &&
                                                     (CategoryID2 == 0 || x.CategoryID2 == CategoryID2) &&
                                                     (CategoryID3 == 0 || x.CategoryID3 == CategoryID3) &&
                                                     (CategoryID4 == 0 || x.CategoryID4 == CategoryID4));

            if (material != null)
            {
                MaterialID = material.MaterialID;

                var mp = _db.MaterialPrices.FirstOrDefault(x => x.MaterialID == MaterialID);
                if (mp == null) return;

                UnitName = mp.MaterialPriceUnit.UnitName;
                CurrentPrice = string.Format("{0:C2}", mp.Price);
                Price = MyConvert.ConvertToDouble(mp.Price);
                MaterialName = MaterialCommon.GetMaterialName(material);
            }
        }



      public void Insert()
        {
            //Insert Categories if not exist
            var mc0 = new MyMaterialCategory0();
            CategoryID0 = mc0.CreateCategoryID(CategoryID0, 0, CategoryName0);

            var mc1 = new MyMaterialCategory1();
            CategoryID1 = mc1.CreateCategoryID(CategoryID1, CategoryID0, CategoryName1);

            var mc2 = new MyMaterialCategory2();
            CategoryID2 = mc2.CreateCategoryID(CategoryID2, CategoryID1, CategoryName2);

            var mc3 = new MyMaterialCategory3();
            CategoryID3 = mc3.CreateCategoryID(CategoryID3, CategoryID2, CategoryName3);

            var mc4 = new MyMaterialCategory4();
            CategoryID4 = mc4.CreateCategoryID(CategoryID4, CategoryID3, CategoryName4);


            //1. Insert material
            var material = new SpecDomain.Model.Material();
            MyReflection.Copy(this, material);
            _db.Materials.Add(material);
            _db.SaveChanges();


            //2. Insert Price
            var price = new MaterialPrice
                            {
                                MaterialID = material.MaterialID,
                                UnitID =UnitID,
                                Price = Price,
                                InvoicePriceUnitID =UnitID,
                                VenderID = 0,
                                Active = true
                            };
            _db.MaterialPrices.Add(price);
            _db.SaveChanges();
    
            //3. Update Price
            material.PriceID = price .PriceID;
            material.UnitID =price .UnitID ;
            material.Price =Price;
            _db.Entry(material).State = EntityState.Modified;
            _db.SaveChanges();

        }

    

      public void Update()
        {
            IsNewCategoryCreated = false;
            var material = _db.Materials.Find(MaterialID);

            //Insert Categories if not exist

            if (!MyConvert.IsNullString(CategoryName0) )
            {
                var mc0 = new MyMaterialCategory0();
                CategoryID0 = mc0.FindCategoryID(0, CategoryName0);                
            }

            var mc1 = new MyMaterialCategory1();
            CategoryID1 = mc1.FindCategoryID(CategoryID0, CategoryName1);

            var mc2 = new MyMaterialCategory2();
            CategoryID2 = mc2.FindCategoryID(CategoryID1, CategoryName2);

            var mc3 = new MyMaterialCategory3();
            CategoryID3 = mc3.FindCategoryID(CategoryID2, CategoryName3);
          
            var mc4 = new MyMaterialCategory4();
            CategoryID4 = mc4.FindCategoryID(CategoryID3, CategoryName4);
         
            //Update material
            material.CategoryID0 = CategoryID0;
            material.CategoryID1 = CategoryID1;
            material.CategoryID2 = CategoryID2;
            material.CategoryID3 = CategoryID3;
            material.CategoryID4 = CategoryID4;
            material.Remark = Remark;
            material.MaterialCode = MaterialCode;

            _db.Entry(material).State = EntityState.Modified;
            _db.SaveChanges();


        }

      public void Copy(int id)
      {
          //Base
          var material = _db.Materials.Find(id);
          var newMaterial = new SpecDomain .Model .Material();
          MyReflection.Copy(material, newMaterial);
          newMaterial.Remark = "Copy -- " + newMaterial.Remark;
          _db.Materials.Add(newMaterial);
          _db.SaveChanges();

          //Price
          var price = _db.MaterialPrices.Find(newMaterial.PriceID);
          var newPrice = new MaterialPrice();
          MyReflection.Copy(price, newPrice);
          newPrice.MaterialID = newMaterial.MaterialID;
          _db.MaterialPrices.Add(newPrice);
          _db.SaveChanges();

          //Update New Material Price
          newMaterial.PriceID = newPrice.PriceID;
          _db.Entry(newMaterial).State = EntityState.Modified;
          _db.SaveChanges();


      }
      
    }
}

  


