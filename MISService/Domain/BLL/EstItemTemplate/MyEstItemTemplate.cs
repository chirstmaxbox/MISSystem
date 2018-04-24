using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using SpecDomain.Model;

namespace SpecDomain.BLL.EstItemTemplate
{
    public class MyEstItemTemplate
    {
        public EST_Item_Specification_Template Value { get; set; }
        public string OptionName { get; set; }

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public MyEstItemTemplate(int templateID)
        {
            Value = _db.EST_Item_Specification_Template.Find(templateID);
        }


        public void UpdateDefaultValue(int optionID)
        {
            if (Value == null) return;

            var option = _db.EST_Item_Specification_Template_Option.Find(optionID);
            OptionName = option.OptionName;

            Value.DefaultValue = optionID;
            Value.DefaultValueString = OptionName;
            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }



        public void UpdateIsMandatory(bool value)
        {
            if (Value == null) return;

            Value.IsMandatory = value;
            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
        }


    }

}
