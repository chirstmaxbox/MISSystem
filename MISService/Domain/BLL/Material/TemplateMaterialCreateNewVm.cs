using System.Collections.Generic;
using SpecDomain.Model;


namespace SpecDomain.BLL.Material
{
    public class TemplateMaterialCreateNewVm
    {
        public int DbItemID { get; set; }
        public List<SpecDomain.Model.Material> Values { get; set; }

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public TemplateMaterialCreateNewVm()
        {
            var vm = new FanMaterialCostBases();
            var orderNumber = 5;
            var itemID = 1001;
            foreach (var fan in vm.Values)
            {
                var material = new EST_Cost_Template_Material
                                   {
                                       //   public int TemplateID { get; set; }
                                   //    ProductID = 263,
                                       //Halo Effect Channel Letter
                                       TypeID = 120,
                                       OrderNumber = orderNumber,
                                       DbItemID = itemID,
                                       Qty  = 1,
                                       Name = fan.CostItemName,
                                       Unit = fan.Unit,
                                       UnitPrice = fan.UnitPrice,
                                       Active = true,
                                   };

                _db.EST_Cost_Template_Material.Add(material);
                orderNumber += 5;
                itemID += 1;
            }

            _db.SaveChanges();
        }

    }
}



