using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using SpecDomain.Model;

namespace SpecDomain.BLL.Cost
{
    public class MyEstCostSummaryPrint
    {
        public List<CR_Cost_Summary> Items { get; set; }

        private readonly int _printingEmployeeID;
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public MyEstCostSummaryPrint(int estRevID, long estItemID, int employeeID)
        {
            _printingEmployeeID = employeeID;
            Items = new List<CR_Cost_Summary>();

            var ecs = new MyEstCostSummaryVmByEstItemID(estItemID);
            ecs.Refresh();

            Items.AddRange(ecs.PriceAs);
            Items.AddRange(ecs.PriceBs);
            Items.AddRange(ecs.PriceExtras);

            foreach (var sum in Items)
            {
                sum.PrintingEmployeeID = _printingEmployeeID;
                sum.EstRevID = estRevID;
                sum.EstItemID = estItemID;
            }
            _db.SaveChanges();
        }

        public void Refresh()
        {
            var existingItems = _db.CR_Cost_Summary.Where(x => x.PrintingEmployeeID == _printingEmployeeID).ToList();
            if (existingItems.Any())
            {
                foreach (var ei in existingItems)
                {
                    _db.Entry(ei).State = EntityState.Deleted;
                }
                _db.SaveChanges();
            }

            foreach (var item in Items)
            {
                _db.CR_Cost_Summary.Add(item);
            }

            _db.SaveChanges();
            
        }
    }



}