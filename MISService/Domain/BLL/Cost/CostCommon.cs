using System;
using System.Collections.Generic;
using System.Linq;
using MyCommon;
using SpecDomain.BLL.Material;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.Cost
{
    public class CostCommon
    {
        //Input Estimation Material For Work Order
        //public static List<MyCostItem> GetCostItems(List<EST_Cost> estCosts, int woID)
        //{
        //    var myCosts = new List<MyCostItem>();
        //    int i = 1;
        //    foreach (var ec in estCosts)
        //    {
        //        var mc = new MyCostItem()
        //        {
        //            CostItemID = ec.CostItemID,
        //            CategoryID =(int)NEstCostTypeCategory.ShopCostItem,
        //            TypeID =(int)NEstCostType.ShopMaterail,
        //            TypeName = "Shop Material",
        //            OrderNumber = i++,

        //            Name = ec.Name,
        //            Unit = ec.Unit,
        //            UnitPrice = ec.UnitPrice,
        //            Qty = ec.Qty,
        //            SubTotal = Math.Round(ec.UnitPrice * ec.Qty, 2),
        //            WoID =woID ,
                    
        //        };
        //        myCosts.Add(mc);
        //    }
        //    return myCosts;
        //}

        public static List<MyCostItem> GetCostItems(List<EST_Cost> estCosts)
        {
            var myCosts = new List<MyCostItem>();
            int i = 1;
            foreach (var ec in estCosts)
            {
                var mc = new MyCostItem()
                             {
                                 CostItemID = ec.CostItemID,
                                 CategoryID = ec.EST_Cost_Type.CategoryID,
                                 TypeID = ec.TypeID,
                                 TypeName = ec.EST_Cost_Type.Name,
                                 OrderNumber = i++,

                                 Name = ec.Name,
                                 Unit = ec.Unit,
                                 UnitPrice = ec.UnitPrice,
                                 Qty = ec.Qty,
                                 SubTotal = Math.Round(ec.UnitPrice * ec.Qty, 2),
                             };
                myCosts.Add(mc);
            }
            return myCosts;
        }



        public static List<MyCostItem> GetCostItems(List<JobCostingTransaction> jobCosts)
        {
       
            var myCosts = new List<MyCostItem>();
            int i = 1;
            foreach (var jc in jobCosts)
            {
                var mc = new MyCostItem()
                {
                    CostItemID = jc.TransactionID,
                    CategoryID = jc.EST_Cost_Type.CategoryID,
                    TypeID = jc.EstCostTypeID,
                    TypeName = jc.EST_Cost_Type.Name,
                    OrderNumber = i++,

                    Name = MaterialCommon.GetMaterialName(jc.Material),
                    Unit = jc.Material.MaterialPriceUnit.UnitName,
                    UnitPrice = jc.Material.Price,
                    Qty = jc.Count,
                    TempInt1=jc.WoID,
                };
                mc.SubTotal = Math.Round(mc.UnitPrice * mc.Qty, 2);


                myCosts.Add(mc);
            }
            return myCosts;
        }

        public static List<MyCostItem> GetCostItems(List< JobCostingOutsourcingTransaction> jobCosts)
        {

            var myCosts = new List<MyCostItem>();
            int i = 1;
            foreach (var jc in jobCosts)
            {
                var mc = new MyCostItem()
                {
                    CostItemID = jc.TransactionID,
                    CategoryID = jc.EST_Cost_Type.CategoryID,
                    TypeID = jc.EstCostTypeID,
                    TypeName = jc.EST_Cost_Type.Name,
                    OrderNumber = i++,

                    Name =GetName(jc) ,
                    Unit = "EA",
                    UnitPrice = jc.UnitPrice,
                    Qty = jc.Count,
                    TempInt1 = 0  ,
                };
                mc.SubTotal = Math.Round(mc.UnitPrice * mc.Qty, 2);


                myCosts.Add(mc);
            }
            return myCosts;
        }

        private static string GetName(JobCostingOutsourcingTransaction jc)
        {
            var s = jc.Sales_JobMasterList_WO.WorkorderNumber;
            if (jc.WoID ==0 )
            {
                char[] splitter = { '-' };
                var ss = jc.WorkorderTitle.Split(splitter);
                s = ss[0].Trim();
            }
            return s  + " - "+jc.Note;
        }

    }
}