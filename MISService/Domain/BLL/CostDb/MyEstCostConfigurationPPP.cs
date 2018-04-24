using System.Data.Entity;
using System.Linq;
using SpecDomain.Model;

namespace SpecDomain.BLL.CostDb
{
    public class MyEstCostConfigurationPpp
    {
        //
        private const int EstCostConfigurationPppRowID = 1;

        public EST_Cost_Configuration Value { get; set; }
        
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
      
        public MyEstCostConfigurationPpp()
        {
            Value = _db.EST_Cost_Configuration.Find(EstCostConfigurationPppRowID);
        }

        public void Update()
        {
            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();

        }

    }


}