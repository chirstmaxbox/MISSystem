using System.Collections.Generic;
using System.Linq;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.Cost
{
    public class MyEstCostSummaryVmByEstItemID
    {
        public List<CR_Cost_Summary> PriceAs { get; set; }
        public List<CR_Cost_Summary> PriceBs { get; set; }
        public List<CR_Cost_Summary> PriceExtras { get; set; }

        public double PriceATotal { get; set; }
        public double PriceBTotal { get; set; }
        public double PriceExtraTotal { get; set; }

        public List<MyCostItem> CostItems { get; set; }

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        private readonly long _estItemID;

        public MyEstCostSummaryVmByEstItemID(long estItemID)
        {
            _estItemID = estItemID;
        }

        public void Refresh()
        {
            CostItems = new List<MyCostItem>();

            var cp = GetCostParameter();
            SetCostItems();

            var pa = new MyCostSummaryPriceA
                         {
                             CostParameter = cp,
                             CostItems = CostItems,
                         };
            pa.Refresh();
            PriceAs = pa.Values;
            PriceATotal = pa.TotalPrice;

            var pb = new MyCostSummaryPriceB
                         {
                             CostParameter = cp,
                             CostItems = CostItems,
                         };
            pb.Refresh();
            PriceBs = pb.Values;
            PriceBTotal = pb.TotalPrice;

            var pExtra = new MyCostSummaryExtra
                             {
                                 CostParameter = cp,
                                 CostItems = CostItems,
                             };
            pExtra.Refresh();
            PriceExtras = pExtra.Values;
        }
   

        public  void SetCostItems()
        {
            var estCosts = _db.EST_Cost.Where(x => x.EstItemID == _estItemID).ToList();
            CostItems=CostCommon.GetCostItems(estCosts);
        }


        public  CostParameter GetCostParameter()
        {
            var estItem = _db.EST_Item.Find(_estItemID);
            var estRevID = estItem.EstRevID;
          
            var  costParameter = new CostParameter
                                     {
                                         CostReportTypeID = (int)NCostReportTypeID.EstItemID,
                                         ProjectID = 0,
                                         EstRevID = (int)estRevID,
                                         EstItemID = _estItemID,
                                         WorkorderID = 0,
                                         WorkorderItemID = 0,
                                         Configuration = _db.EST_Cost_Configuration.First(x => x.EstRevID == estRevID),
                                     };
            return costParameter;
        }
    }
}