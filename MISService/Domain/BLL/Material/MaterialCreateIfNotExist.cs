using System;

using System.Data.Entity;
using System.Linq;
using SpecDomain.Model;

namespace SpecDomain.BLL.Material
{
    public class MaterialCreateIfNotExist
    {
        public bool IsValidated { get; private set; }
        public string ResultMessage { get; set; }
        public int NewMaterialID { get; private set; }

        private  Model.Material _myMaterial;
        private MaterialPrice _myPrice;
        private readonly int _unitID;
        private readonly string _tobeinsertedMaterialCategory2OrCategory3;
        private readonly double _unitPrice;

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public MaterialCreateIfNotExist(Model.Material material, string tobeinsertedMaterialCategory2OrCategory3, string unitName, double unitPrice)
        {
            IsValidated = true;
    
            if (material.CategoryID0 ==0)
            {
                IsValidated = false;
                ResultMessage += "Please Choose a Category;  "+ Environment .NewLine ;
            }
            if (material.CategoryID1 ==0)
            {
                IsValidated = false;
                ResultMessage += "Please Choose a Sub-Category 1;  " + Environment.NewLine;
            }
            _unitID = GetUnitID(unitName);
            if (_unitID==0)
            {
                IsValidated = false;
                ResultMessage += "Can not find the Unit Name; " + Environment.NewLine;
            }
            if (!IsValidated) return;


            _tobeinsertedMaterialCategory2OrCategory3 = tobeinsertedMaterialCategory2OrCategory3; 
            _myMaterial = material;
            _unitPrice = unitPrice;
        }

        public void Insert()
        {
            if (!IsValidated) return;
            InsertMaterial(_tobeinsertedMaterialCategory2OrCategory3);
            InsertPrice(_unitID, _unitPrice);
            UpdateMaterialPrice();
        }



        private int GetUnitID(string unitName)
        {
            unitName = unitName.ToUpper().Trim();
            var materialUnit = _db.MaterialPriceUnits.FirstOrDefault(x => x.UnitName.ToUpper() == unitName);
            return materialUnit == null ? 0 : materialUnit.UnitID;
        }

        //1. Insert material
        private void InsertMaterial( string tobeinsertedMaterialCategory2OrCategory3)
        {
            if (!IsValidated) return;

            if (_myMaterial.CategoryID2 == 0)
            {
                var mc2 = new MyMaterialCategory2();
                _myMaterial.CategoryID2 = mc2.CreateCategoryID(_myMaterial.CategoryID2, _myMaterial.CategoryID1, tobeinsertedMaterialCategory2OrCategory3);
            }
            else
            {
                var mc3 = new MyMaterialCategory3();
                _myMaterial.CategoryID3 = mc3.CreateCategoryID(_myMaterial.CategoryID3, _myMaterial.CategoryID2, tobeinsertedMaterialCategory2OrCategory3);
            }

            _db.Materials.Add(_myMaterial);
            _db.SaveChanges();
        }

        //2. Insert Price
        private void InsertPrice(int unitID, double unitPrice)
        {
            _myPrice = new MaterialPrice
                          {
                              MaterialID = _myMaterial.MaterialID,
                              UnitID = unitID,
                              Price = unitPrice,
                              InvoicePriceUnitID = unitID,
                              VenderID = 0,
                              Active = true
                          };

            _db.MaterialPrices.Add(_myPrice);
            _db.SaveChanges();
        }
      
        //3. Update Price
        private void UpdateMaterialPrice()
        {
            _myMaterial.PriceID = _myPrice.PriceID;
            _myMaterial.UnitID = _myPrice.UnitID;
            _myMaterial.Price =Convert.ToDouble(  _myPrice.Price);
            _db.Entry(_myMaterial).State = EntityState.Modified;
            _db.SaveChanges();
        }

    }
}