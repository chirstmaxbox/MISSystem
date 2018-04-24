using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.Cost
{
    public class MyEstCostItemPrint
    {

        private List<CR_CostItem> _items;

        private readonly int _printingEmployeeID;
        private readonly int _estRevID;
        private readonly long _estItemID;
 
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public MyEstCostItemPrint(int estRevID, long estItemID, int employeeID, bool isBiddingJob)
        {
            _printingEmployeeID = employeeID;
            _estRevID = estRevID;
            _estItemID = estItemID;
        
            GetCostItems();

        if (isBiddingJob)
        {
            SetParametersBid();
        }
        else
        {
            SetParameters();
        }
            

        }

        private void GetCostItems()
        {
            _items = new List<CR_CostItem>();
            //For all Cost type, if there are not items, add Type only
            // Category.Installation vs. Category.Locall Installer, only one can exist


            var originalCosts = _db.EST_Cost.Where(x => x.EstItemID == _estItemID)
                              .OrderBy(x => x.EST_Cost_Type.CategoryID)
                             .ThenBy(x => x.EST_Cost_Type.OrderNumber)
                                 .ThenBy(x => x.OrderNumber)
                                 .ToList();

            //
            var isShowInstallationCost = true;
            var countlFsInstallation =originalCosts.Count(x => x.EST_Cost_Type.CategoryID == (int) NEstCostTypeCategory.InstallationCostItem);
            if (countlFsInstallation == 0)
            {
                var countLocalInstaller = originalCosts.Count(x => x.EST_Cost_Type.CategoryID == (int)NEstCostTypeCategory.LocalInstallerCost);
                if(countLocalInstaller>0 )
                {
                    isShowInstallationCost = false;
                }
            }

            var estCostTypes = _db.EST_Cost_Type.Where(x => 
                                                       x.TypeID > 0 & 
                                                       (x.CategoryID ==(int)NEstCostTypeCategory.ShopCostItem |
                                                        isShowInstallationCost & x.CategoryID ==(int)NEstCostTypeCategory.InstallationCostItem |
                                                        !isShowInstallationCost & x.CategoryID == (int)NEstCostTypeCategory.LocalInstallerCost 
                                                       )
                ).OrderBy(x => x.CategoryID)
                .ThenBy(x => x.OrderNumber)
                .ToList();

      
            int i = 1;
            foreach (var estCostType in estCostTypes)
            {
                var costsByType = originalCosts.Where(x => x.TypeID == estCostType.TypeID).ToList();
                if (costsByType.Any())
                {
                    int j = 1;
                    foreach (var cost in costsByType)
                    {
                        var crCostItem = GetCrCostItem(cost,i,j++);
                        _items.Add(crCostItem);
                    }
                }
                else
                {
                    var crCostItem = GetCrCostItem(estCostType, i,1);
                    _items.Add(crCostItem);
                }
                i++;
            }
        }

        private CR_CostItem GetCrCostItem(EST_Cost cost, int typeOrderNumber, int itemOrderNumber)
        {
            var crCostItem = new CR_CostItem()
                                 {
                                     //public long CostItemID { get; set; }
                                     ProjectID = 0,
                                     EstRevID = _estRevID,
                                     EstItemID = _estItemID,
                                     WorkorderID = 0,
                                     WorkorderItemID = 0,
                                     PrintingEmployeeID = _printingEmployeeID,
                                     TypeOrderNumber = typeOrderNumber,
                                     TypeID = cost.TypeID,
                                     TypeName = cost.EST_Cost_Type.Name,
                                     ItemOrderNumber = itemOrderNumber,
                                     CosItemName = cost.Name,
                                     Unit = cost.Unit,
                                     UnitPrice = cost.UnitPrice,
                                     Qty = cost.Qty,
                                     SubTotal = Math.Round(cost.UnitPrice * cost.Qty),
                                 };
            return crCostItem;
        }

        private CR_CostItem GetCrCostItem(EST_Cost_Type estCostType, int typeOrderNumber, int itemOrderNumber)
        {
            var crCostItem = new CR_CostItem()
                                 {
                                     //public long CostItemID { get; set; }
                                     ProjectID = 0,
                                     EstRevID = _estRevID,
                                     EstItemID = _estItemID,
                                     WorkorderID = 0,
                                     WorkorderItemID = 0,
                                     PrintingEmployeeID = _printingEmployeeID,
                                     TypeOrderNumber = typeOrderNumber,
                                     TypeID =estCostType.TypeID,
                                     TypeName =estCostType.Name,
                                     ItemOrderNumber = itemOrderNumber ,
                                     CosItemName = estCostType.Name,
                                     Unit ="",
                                     UnitPrice =0,
                                     Qty = 0,
                                     SubTotal =0,
                                 };
            return crCostItem;
        }

        
        private void SetParameters()
        {
            var parameter = _db.EST_Cost_Configuration.FirstOrDefault( x=>x.EstRevID ==_estRevID);
            if (parameter == null) return;

            foreach (var item in _items)
            {
                if (item.TypeID == (int) NEstCostType.ShopMaterail)
                {
                    item.TypeName += "(x Markup " + parameter.MarkupShopMaterial.ToString("") + ")";
                }
                if (item.TypeID == (int) NEstCostType.InstallationMaterail)
                {
                    item.TypeName += "(x Markup " + parameter.MarkupInstallationMaterial .ToString("") + ")";
                }

                if (item.TypeID == (int) NEstCostType.ShopStandItem )
                {
                    item.TypeName += "(x" + parameter.MarkupStandardItem.ToString("") + ")";
                }
            }

        }

        //Public overrider
        //Material Wastage
        private void SetParametersBid()
        {
            var parameter = _db.EST_Cost_Configuration.FirstOrDefault(x => x.EstRevID == _estRevID);
            if (parameter == null) return;

            foreach (var item in _items)
            {
                if (item.TypeID == (int)NEstCostType.ShopMaterail)
                {
                    item.TypeName += "(x Markup " + parameter.MarkupShopMaterial.ToString("") + ")";
                    if (parameter.MaterialWastageRate >0)
                    {
                        item.TypeName += "(x Wastage " + parameter.MaterialWastageRate.ToString("") + ")";
                    }
                }
                if (item.TypeID == (int)NEstCostType.InstallationMaterail)
                {
                    item.TypeName += "(x Markup " + parameter.MarkupInstallationMaterial.ToString("") + ")";
                    if (parameter.MaterialWastageRate > 0)
                    {
                        item.TypeName += "(x Wastage " + parameter.MaterialWastageRate.ToString("") + ")";
                    }
                }

                if (item.TypeID == (int)NEstCostType.ShopStandItem)
                {
                    item.TypeName += "(x" + parameter.MarkupStandardItem.ToString("") + ")";
                }
            }

        }

        
        public void Refresh()
        {
            var existingItems = _db.CR_CostItem.Where(x => x.PrintingEmployeeID == _printingEmployeeID).ToList();
            if (existingItems.Any())
            {
                foreach (var ei in existingItems)
                {
                    _db.Entry(ei).State = EntityState.Deleted;
                }
                _db.SaveChanges();
            }

            foreach (var item in _items)
            {
                _db.CR_CostItem.Add(item);
            }
            _db.SaveChanges();
        }
    }
}