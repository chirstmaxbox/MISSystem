using System.Data.Entity;
using System.Linq;
using SpecDomain.Model;

namespace SpecDomain.BLL.CostDb
{
    public class MyEstCostConfiguration
    {
        public EST_Cost_Configuration Value { get; set; }
        
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
      
        public MyEstCostConfiguration(int estRevID)
        {
            Value = _db.EST_Cost_Configuration.OrderBy(x=>x.ConfigID).First(x=>x.EstRevID==estRevID );
            
        }

        public MyEstCostConfiguration()
        {
        }
        public void Update()
        {
            _db.Entry(Value).State = EntityState.Modified;
            _db.SaveChanges();
            var hello = "World";
        }

    }


}