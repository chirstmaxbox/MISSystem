using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyCommon;
using SpecDomain.Model;

namespace SpecDomain.BLL.Material
{
    //Together with Price
    public class MaterialPriceBase : MaterialPrice
    {
        public string CurrentPrice { get; set; }
        public string UnitName { get; set; }
        public string UpdateDate { get; set; }
        public string InvoicingPrice { get; set; }
    
        public void Initialize(MaterialPrice price)
        {
            MyReflection.Copy(price, this);
            CurrentPrice = string.Format("{0:C2}", Price);
            InvoicingPrice = string.Format("{0:C2}", InvoicePrice);
            UnitName = price.MaterialPriceUnit.UnitName;
            UpdateDate = string.Format( "{0:MMM dd, yyyy}", InputDate );
        }
     }

    //
    public class MaterialPriceEdit:MaterialPrice
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        public bool CalculatePriceByInvoice { get; set; }
        public IEnumerable<SelectListItem> Units { get; set; }

        public void Initialize(int priceID)
        {
            var mp =_db.MaterialPrices.Find(priceID) ;
            MyReflection.Copy(mp, this);
            CalculatePriceByInvoice = false;
        }
        
        public void RefreshDropdownlist()
        {
            var temp10 = _db.MaterialPriceUnits.Where(x => x.UnitID >= 0).OrderBy(x => x.UnitName).ToList();
            Units = temp10.Select(x => new SelectListItem
            {
                Value = Convert.ToString(x.UnitID),
                Text = x.UnitName
            });
        }

        public void Update()
        {
            //Price Table It's self
            var mp =_db.MaterialPrices.Find(PriceID) ;
            MyReflection.Copy(this,mp);
            _db.Entry(mp).State = EntityState.Modified;

            //Sync copy of the contens in Table.Material
            var material = _db.Materials.Find(MaterialID);
            material.PriceID = PriceID;
            material.UnitID = UnitID;
            material.Price = MyCommon.MyConvert.ConvertToDouble(Price);
            _db.Entry(material).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }

    public class MaterialPriceCreate
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        public bool CalculatePriceByInvoice { get; set; }
        public IEnumerable<SelectListItem> Units { get; set; }
        public List<MaterialPriceBase> Values { get; set; }
        public MaterialPrice Value { get; set; }

        public void Initialize(int parentID)
        {
            Value = new MaterialPrice();
            Value.MaterialID = parentID;

            CalculatePriceByInvoice = true;
            Values = new List<MaterialPriceBase>();

            var mps = _db.MaterialPrices.Where(x => x.MaterialID == parentID).OrderByDescending(x=>x.InputDate).ThenByDescending(x=>x.MaterialID).ToList();
            if (mps.Count <= 0) return;
            foreach (var mp in mps)
            {
                var mpb = new MaterialPriceBase();
                mpb.Initialize(mp);
                Values.Add(mpb);
            }
        }
        
        public void RefreshDropdownlist()
        {
            var temp10 = _db.MaterialPriceUnits.Where(x => x.UnitID >= 0).OrderBy(x => x.UnitName).ToList();
            Units = temp10.Select(x => new SelectListItem
            {
                Value = Convert.ToString(x.UnitID),
                Text = x.UnitName
            });
        }

        public void Create()
        {
            _db.MaterialPrices.Add(Value);
            _db.SaveChanges();
        }

        public void DeActiveExistingPrices()
        {   
            var mps = _db.MaterialPrices.Where(x => x.MaterialID == Value.MaterialID).ToList();
            if (mps.Count <= 0) return;
            foreach (var mp in mps)
            {
                mp.Active = false;
                _db.Entry(mp).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        public void SyncCopyInTableMaterial()
        {
            var material = _db.Materials.Find(Value .MaterialID );
            material.PriceID = Value .PriceID;
            material.UnitID = Value .UnitID;
            material.Price = MyCommon.MyConvert.ConvertToDouble(Value .Price);
            _db.Entry(material).State = EntityState.Modified;
            _db.SaveChanges();

        }
    }

}