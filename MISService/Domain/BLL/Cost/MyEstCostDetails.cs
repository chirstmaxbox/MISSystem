using System.Collections.Generic;
using System.Linq;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.Cost
{
    public class MyEstCostDetails
    {
        public List<MyCostItem> MyValues { get; set; }

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();


        public MyEstCostDetails(List<long> estItemIDs)
        {
            MyValues = new List<MyCostItem>();

            if (!estItemIDs.Any()) return;

                var estCosts = _db.EST_Cost.Where(x => estItemIDs.Contains( x.EstItemID) &  
                                                       x.EST_Item.ItemPurposeID == (int)NEstItemPurpose.ForEstimation)
                    .ToList();

                if (!estCosts.Any()) return;

                var mycosts = CostCommon.GetCostItems(estCosts);

                MyValues.AddRange(mycosts);
         

            SetOrders();

        }

        private void SetOrders()
        {
            var estCostTypes = _db.EST_Cost_Type.Where(x => x.TypeID > 0)
                .OrderBy(x => x.CategoryID)
                .ThenBy(x => x.OrderNumber)
                .ToList();

            int i = 1;
            foreach (var estCostType in estCostTypes)
            {
                var costsByType = MyValues.Where(x => x.TypeID == estCostType.TypeID).ToList();
                if (costsByType.Any())
                {
                    foreach (var cost in costsByType)
                    {
                        cost.OrderNumber = i++;
                    }
                }
            }

            MyValues = MyValues.OrderBy(x => x.OrderNumber).ToList();
        }
    }

}