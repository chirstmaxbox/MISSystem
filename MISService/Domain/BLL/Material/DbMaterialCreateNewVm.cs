using System.Collections.Generic;
using SpecDomain.Model;


namespace SpecDomain.BLL.Material
{
    public class DbMaterialCreateNewVm
    {
        public int DbItemID { get; set; }
        public List<SpecDomain.Model.Material> Values { get; set; }

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public DbMaterialCreateNewVm()
        {
            var vm = new FanMaterialCostBases();
            foreach (var fan in vm.Values)
            {
                var material = new SpecDomain.Model.Material() 
                                   {
                                       //public int DbItemID { get; set; }
                                       //TypeID = 120,
                                       //Name = fan.CostItemName,
                                       //Unit = fan.Unit,
                                       //UnitPrice = fan.UnitPrice,
                                   };
                _db.Materials .Add(material);
            }

            _db.SaveChanges();
        }

    }
}



