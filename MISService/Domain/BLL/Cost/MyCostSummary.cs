using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using System.Linq;
using MyCommon;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.Cost
{
    public abstract class MyCostSummary
    {
        //Input
        public CostParameter CostParameter { get; set; }
        public List<MyCostItem> CostItems { get; set; }

        //Output
        public List<CR_Cost_Summary> Values { get; set; }
        public double TotalPrice { get; set; }

 //       public double TotalBeforePriceBProfit { get; set; }

        protected MyCostSummary()
        {
            Values = new List<CR_Cost_Summary>();
        }
    }

    public class CostParameter
    {
        public int CostReportTypeID { get; set; }   //Project vs. EstRevID vs. EstItemID vs. ...

        public int ProjectID { get; set; }
        public int EstRevID { get; set; }
        public long EstItemID { get; set; }
        public int WorkorderID { get; set; }
        public long WorkorderItemID { get; set; }

        //Factors
        public EST_Cost_Configuration Configuration { get; set; }
    }

    public class CostParameterProject : CostParameter 
    {
        public CostParameterProject(int projectID)
        {
            var db = new SpecificationDbEntities();

            var estRev = db.Sales_JobMasterList_EstRev.First(x => x.JobID == projectID);
            var estRevID = estRev.EstRevID;
            CostReportTypeID = (int) NCostReportTypeID.EstRevID;
            ProjectID = projectID;
            EstRevID = estRevID;
            EstItemID = 0;
            WorkorderID = 0;
            WorkorderItemID = 0;
            Configuration = db.EST_Cost_Configuration.First(x => x.EstRevID == estRevID);

        }
    }

    public class MyCostSummaryPriceA : MyCostSummary
    {

        //13227

       /// <summary>
        /// Collumn : Name
        /// Collumn 0: Rate
        /// Collumn 1: SupplyOnly
        /// Collumn 2: Installation
        /// </summary>
        public void Refresh()
        {
            if (CostItems==null) return;
            if (!CostItems.Any()) return;
            //-------------------------------------------------
            //Shop Material Cost=Material + 
            var t00 = CostItems.Where(x => x.TypeID == (int) NEstCostType.ShopMaterail).ToList();

            var marterialCostShopOriginal = t00.Count == 0 ? 0 : t00.Sum(x => x.SubTotal);
            marterialCostShopOriginal = marterialCostShopOriginal * CostParameter.Configuration.MarkupShopMaterial;


            marterialCostShopOriginal = marterialCostShopOriginal * CostParameter.Configuration.MaterialWastageRate;

            //Other and Subcontract
            var t00a = CostItems.Where(x => x.TypeID == (int)NEstCostType.ShopOther  ||
                                            x.TypeID == (int)NEstCostType.ShopSubcontract)
                                            .ToList();
            var materialCostShopSubcontractAndOther= t00a.Count == 0 ? 0 : t00a.Sum(x => x.SubTotal);

            var materialCostShop =( marterialCostShopOriginal +materialCostShopSubcontractAndOther) *
                                   CostParameter.Configuration.MaterialMarkupPercentage;
           var name01 = "Shop Material Cost (x" +
                        MyConvert.ConvertToString(CostParameter.Configuration.MaterialMarkupPercentage) +
                        ")";
           name01 += " (x";
           name01 += MyConvert.ConvertToString(CostParameter.Configuration.MarkupShopMaterial) +
                       ")";

            var summary01 = new CR_Cost_Summary
                                {
                                    Name =name01,

                                    ProjectID = CostParameter.ProjectID,
                                    EstRevID = CostParameter.EstRevID,
                                    EstItemID = CostParameter.EstItemID,
                                    WorkorderID = CostParameter.WorkorderID,
                                    WorkorderItemID = CostParameter.WorkorderItemID,

                                    OrderNumber = 1,
                                    Column0 = "",
                                    Column1 = "",
                                    Column2 = (materialCostShop).ToString("C0"),
                                    TypeID = (int) NEstCostSummaryType.PriceA,
                                };


            //-------------------------------------------------
            var t01 = CostItems.Where(x => x.TypeID == (int) NEstCostType.ShopLabour).ToList();
            var labourHourShop = t01.Count == 0 ? 0 : t01.Sum(x => x.Qty);
            var labourCostShop = CostParameter.Configuration.LabourRateShop*labourHourShop;

            var summary02 = new CR_Cost_Summary
                                {
                                    Name ="Shop Labour Cost ($" +Convert.ToString(CostParameter.Configuration.LabourRateShop) + ")",

                                    ProjectID = CostParameter.ProjectID,
                                    EstRevID = CostParameter.EstRevID,
                                    EstItemID = CostParameter.EstItemID,
                                    WorkorderID = CostParameter.WorkorderID,
                                    WorkorderItemID = CostParameter.WorkorderItemID,

                                    OrderNumber = 2,
                                    Column0 = "",
                                    Column1 = "",
                                    Column2 = (labourCostShop).ToString("C0"),
                                    TypeID = (int) NEstCostSummaryType.PriceA,
                                };

            //-------------------------------------------------
           var modifyRate = Convert.ToDouble(CostParameter.Configuration.DiscountRate);
           var modifyRateString = " (" + MyConvert.ConvertDoubleToPercentage(CostParameter.Configuration.DiscountRate) + ")";


            var t0b = CostItems.Where(x => x.TypeID == (int) NEstCostType.ShopStandItem).ToList();
            var shopStandardItem = t0b.Count == 0 ? 0 : t0b.Sum(x => x.SubTotal);
            shopStandardItem = shopStandardItem*CostParameter.Configuration.MarkupStandardItem;
            //
           var name1 = "Supply Only Cost";
            if (Math.Abs(modifyRate - 0) > 0.001 )
            {
                name1 += modifyRateString  ;
            }

            var supplyOnlyCost = (materialCostShop + labourCostShop)*(1 + CostParameter.Configuration.DiscountRate) +shopStandardItem;
            var summary11 = new CR_Cost_Summary
                                {
                                    Name = name1,
                                    ProjectID = CostParameter.ProjectID,
                                    EstRevID = CostParameter.EstRevID,
                                    EstItemID = CostParameter.EstItemID,
                                    WorkorderID = CostParameter.WorkorderID,
                                    WorkorderItemID = CostParameter.WorkorderItemID,

                                    OrderNumber = 11,
                                    Column0 = "",
                                    Column1 = "",
                                    Column2 = (supplyOnlyCost).ToString("C0"),
                                    TypeID = (int) NEstCostSummaryType.PriceA,
                                };

            //-------------------------------------------------
            //Installation
            var t2A = CostItems.Where(x => x.TypeID == (int) NEstCostType.InstallationTraveling).ToList();
            var travellingHours = t2A.Count == 0 ? 0 : t2A.Sum(x => x.Qty);
            var travellingCost =MyConvert.ConvertToDouble(travellingHours*CostParameter.Configuration.LabourRateInstallation*
                                          (1 + CostParameter.Configuration.DiscountRate));

            var summary12Name = "Travelling Cost"+ "($" + Convert.ToString(CostParameter.Configuration.LabourRateInstallation) + ")";
            if (Math.Abs(modifyRate - 0) > 0.001 )
            {
                summary12Name += modifyRateString  ;
            }

            var summary12 = new CR_Cost_Summary
                                {
                                    Name = summary12Name,
                                    ProjectID = CostParameter.ProjectID,
                                    EstRevID = CostParameter.EstRevID,
                                    EstItemID = CostParameter.EstItemID,
                                    WorkorderID = CostParameter.WorkorderID,
                                    WorkorderItemID = CostParameter.WorkorderItemID,

                                    OrderNumber = 12,
                                    Column0 = "",
                                    Column1 = "",
                                    Column2 = travellingCost.ToString("C0"),
                                    TypeID = (int) NEstCostSummaryType.PriceA,
                                };

            //-------------------------------------------------
            var t3A = CostItems.Where(x => x.TypeID == (int) NEstCostType.InstallationLabour).ToList();
            var installHours = t3A.Count == 0 ? 0 : t3A.Sum(x => x.Qty);
           var installHoursCost =MyConvert.ConvertToDouble(installHours*CostParameter.Configuration.LabourRateInstallation);
                         

            //Material
            var t3B = CostItems.Where(x =>x.TypeID == (int) NEstCostType.InstallationMaterail).ToList();
            var installationMaterialOriginalCost = t3B.Count == 0 ? 0 : t3B.Sum(x => x.SubTotal);
           installationMaterialOriginalCost = installationMaterialOriginalCost*
                                              CostParameter.Configuration.MarkupInstallationMaterial;
           installationMaterialOriginalCost = installationMaterialOriginalCost * CostParameter.Configuration.MaterialWastageRate;

            var t3c = CostItems.Where(x =>x.TypeID == (int)NEstCostType.InstallationOther).ToList();
            var installationOtherCost = t3c.Count == 0 ? 0 : t3c.Sum(x => x.SubTotal);


            var t3D = CostItems.Where(x =>x.TypeID == (int)NEstCostType.InstallationEquipment ).ToList();
            var installationEquipmentCost = t3D.Count == 0 ? 0 : t3D.Sum(x => x.SubTotal);

            var installationCost = installHoursCost + (installationMaterialOriginalCost + installationOtherCost) * CostParameter.Configuration.MaterialMarkupPercentage *
                                   (1 + CostParameter.Configuration.DiscountRate) + installationEquipmentCost ;

            var summary13Name = "Installation Cost" +"($" + Convert.ToString(CostParameter.Configuration.LabourRateInstallation) + ")";
            if (Math.Abs(modifyRate - 0) > 0.001)
            {
                summary13Name += modifyRateString;
            }
  
            var summary13 = new CR_Cost_Summary
                                {
                                    Name = summary13Name,
                                    ProjectID = CostParameter.ProjectID,
                                    EstRevID = CostParameter.EstRevID,
                                    EstItemID = CostParameter.EstItemID,
                                    WorkorderID = CostParameter.WorkorderID,
                                    WorkorderItemID = CostParameter.WorkorderItemID,

                                    OrderNumber = 13,
                                    Column0 = "",
                                    Column1 = "",
                                    Column2 = (installationCost).ToString("C0"),
                                    TypeID = (int) NEstCostSummaryType.PriceA,
                                };

            //-------------------------------------------------
            var targetPrice = supplyOnlyCost + travellingCost + installationCost;
            var summary14 = new CR_Cost_Summary
                                {
                                    Name = "Target Price A",

                                    ProjectID = CostParameter.ProjectID,
                                    EstRevID = CostParameter.EstRevID,
                                    EstItemID = CostParameter.EstItemID,
                                    WorkorderID = CostParameter.WorkorderID,
                                    WorkorderItemID = CostParameter.WorkorderItemID,

                                    OrderNumber = 14,
                                    Column0 = "",
                                    Column1 = "",
                                    Column2 = (targetPrice).ToString("C0"),
                                    TypeID = (int) NEstCostSummaryType.PriceA,
                                };

            Values = new List<CR_Cost_Summary>() {summary01, summary02, summary11, summary12, summary13, summary14};

            //To Update DbTable
            TotalPrice = targetPrice;

        }     
    }

    public class MyCostSummaryPriceB : MyCostSummary
    {
        public void Refresh()
        {
            if (CostItems == null) return;
            if (!CostItems.Any()) return;

            // Shop Total=Material+ Material+Subcontract +ShopOther, not included standard Item ******
            // Material
            var t0a = CostItems.Where(x => x.TypeID == (int)NEstCostType.ShopMaterail).ToList();
            var shopMaterial = t0a.Count == 0 ? 0 : t0a.Sum(x => x.SubTotal);

            shopMaterial =shopMaterial*CostParameter.Configuration.MarkupShopMaterial;
            shopMaterial = shopMaterial * CostParameter.Configuration.MaterialWastageRate;

            
            //Labour and SubContract and Other
            var t0 = CostItems.Where(x => x.TypeID == (int)NEstCostType.ShopLabour |
                                          x.TypeID == (int)NEstCostType.ShopSubcontract |
                                          x.TypeID == (int)NEstCostType.ShopOther |
                                          x.TypeID == (int)NEstCostType.PermitCost )
                                          .ToList();
            var shopLabourAndSubcontractAndOther = t0.Count == 0 ? 0 : t0.Sum(x => x.SubTotal);
           
            //Markup Material
            var shopDirectCost = shopLabourAndSubcontractAndOther + shopMaterial;
            
            //Installation Total=Material+ Labour+travelling+Other, not included Equipment
            var t1a = CostItems.Where(x =>x.TypeID == (int)NEstCostType.InstallationMaterail)
                                            .ToList();
            var installationMaterial = t1a.Count == 0 ? 0 : t1a.Sum(x => x.SubTotal);
            installationMaterial=installationMaterial * CostParameter.Configuration.MarkupShopMaterial;
            installationMaterial = installationMaterial * CostParameter.Configuration.MaterialWastageRate;

            var t1 = CostItems.Where(x => x.TypeID == (int)NEstCostType.InstallationLabour |
                                          x.TypeID == (int)NEstCostType.InstallationOther 
                                            )
                                            .ToList();
            var installationLabourTravellingAndOther = t1.Count == 0 ? 0 : t1.Sum(x => x.SubTotal);

            var installationDirectCost = installationLabourTravellingAndOther + installationMaterial;
                                     

            var summary01 = new CR_Cost_Summary
            {
                Name = "Direct Cost",

                ProjectID = CostParameter.ProjectID,
                EstRevID = CostParameter.EstRevID,
                EstItemID = CostParameter.EstItemID,
                WorkorderID = CostParameter.WorkorderID,
                WorkorderItemID = CostParameter.WorkorderItemID,

                OrderNumber = 1,
                Column0 = "",
                Column1 = (shopDirectCost).ToString("C0"),
                Column2 = (installationDirectCost).ToString("C0"),
                TypeID = (int)NEstCostSummaryType.PriceB,
            };


            var summary02 = new CR_Cost_Summary
            {
                Name = "Overhead Cost" +" ("+  MyConvert.ConvertDoubleToPercentage(CostParameter.Configuration.OverheadRate) + ")",

                ProjectID = CostParameter.ProjectID,
                EstRevID = CostParameter.EstRevID,
                EstItemID = CostParameter.EstItemID,
                WorkorderID = CostParameter.WorkorderID,
                WorkorderItemID = CostParameter.WorkorderItemID,

                OrderNumber = 2,
                Column0 ="",
                Column1 = (shopDirectCost * CostParameter.Configuration.OverheadRate).ToString("C0"),
                Column2 = (installationDirectCost * CostParameter.Configuration.OverheadRate).ToString("C0"),
                TypeID = (int)NEstCostSummaryType.PriceB,
            };


            var totalCostShop = shopDirectCost * (1 + CostParameter.Configuration.OverheadRate);
            var totalCostInstallation = installationDirectCost * (1 + CostParameter.Configuration.OverheadRate);

            var summary03 = new CR_Cost_Summary
            {
                Name = "Total Cost",

                ProjectID = CostParameter.ProjectID,
                EstRevID = CostParameter.EstRevID,
                EstItemID = CostParameter.EstItemID,
                WorkorderID = CostParameter.WorkorderID,
                WorkorderItemID = CostParameter.WorkorderItemID,

                OrderNumber = 3,
                Column0 = "",
                Column1 = (totalCostShop).ToString("C0"),
                Column2 = (totalCostInstallation).ToString("C0"),
                TypeID = (int)NEstCostSummaryType.PriceB,
            };



            //-------------------------------------------------
            var modifyRate = Convert.ToDouble(CostParameter.Configuration.DiscountRate);
            var modifyRateString = " (" + MyConvert.ConvertDoubleToPercentage(CostParameter.Configuration.DiscountRate) + ")";
            //

            //+Standard Item
            var t0b = CostItems.Where(x => x.TypeID == (int)NEstCostType.ShopStandItem).ToList();
            var shopStandardItem = t0b.Count == 0 ? 0 : t0b.Sum(x => x.SubTotal);
            shopStandardItem = shopStandardItem * CostParameter.Configuration.MarkupStandardItem;

            var supplyOnlyCost = totalCostShop / (1 - CostParameter.Configuration.TargetProfitRate) * (1 + CostParameter.Configuration.DiscountRate) + shopStandardItem;

            var name1 = "Supply Only Cost";
            if (Math.Abs(modifyRate - 0) > 0.001)
            {
                name1 += modifyRateString;
            }


            var summary11 = new CR_Cost_Summary
            {
                Name = name1,

                ProjectID = CostParameter.ProjectID,
                EstRevID = CostParameter.EstRevID,
                EstItemID = CostParameter.EstItemID,
                WorkorderID = CostParameter.WorkorderID,
                WorkorderItemID = CostParameter.WorkorderItemID,

                OrderNumber = 11,
                Column0 ="",
                Column1 = supplyOnlyCost.ToString("C0"),
                Column2 = "n/a",
                TypeID = (int)NEstCostSummaryType.PriceB,
            };

            var t12 = CostItems.Where(x => x.TypeID == (int)NEstCostType.InstallationTraveling).ToList();
            var travellingCostOriginal = t12.Count == 0 ? 0 : t12.Sum(x => x.SubTotal);
            var travellingCost = travellingCostOriginal + travellingCostOriginal * CostParameter.Configuration.OverheadRate / (1 - CostParameter.Configuration.TargetProfitRate) * (1 + CostParameter.Configuration.DiscountRate);

            var name2 = "Travelling Cost";
            if (Math.Abs(modifyRate - 0) > 0.001)
            {
                name2 += modifyRateString;
            }

            var summary12 = new CR_Cost_Summary
            {
                Name = name2, 

                ProjectID = CostParameter.ProjectID,
                EstRevID = CostParameter.EstRevID,
                EstItemID = CostParameter.EstItemID,
                WorkorderID = CostParameter.WorkorderID,
                WorkorderItemID = CostParameter.WorkorderItemID,

                OrderNumber = 12,
                Column0 = "",
                Column1 = "n/a",
                Column2 = (travellingCost).ToString("C0"),
                TypeID = (int)NEstCostSummaryType.PriceB,
            };

            var t13a = CostItems.Where(x => x.TypeID == (int)NEstCostType.InstallationEquipment).ToList();
            var installationEquipment = t13a.Count == 0 ? 0 : t13a.Sum(x => x.SubTotal);
            var installationCost = totalCostInstallation / (1 - CostParameter.Configuration.TargetProfitRate) * (1 + CostParameter.Configuration.DiscountRate) + installationEquipment;

            var name3 = "Installation Cost";
            if (Math.Abs(modifyRate - 0) > 0.001)
            {
                name3 += modifyRateString;
            }
            
            var summary13 = new CR_Cost_Summary
            {
                Name = name3, 

                ProjectID = CostParameter.ProjectID,
                EstRevID = CostParameter.EstRevID,
                EstItemID = CostParameter.EstItemID,
                WorkorderID = CostParameter.WorkorderID,
                WorkorderItemID = CostParameter.WorkorderItemID,

                OrderNumber = 13,
                Column0 = "",
                Column1 = "n/a",
                Column2 = (installationCost).ToString("C0"),
                TypeID = (int)NEstCostSummaryType.PriceB,
            };


            var targetPrice = supplyOnlyCost + travellingCost + installationCost;
            var summary14 = new CR_Cost_Summary
            {
                Name = "Target Price B",

                ProjectID = CostParameter.ProjectID,
                EstRevID = CostParameter.EstRevID,
                EstItemID = CostParameter.EstItemID,
                WorkorderID = CostParameter.WorkorderID,
                WorkorderItemID = CostParameter.WorkorderItemID,

                OrderNumber = 14,
                Column0 = "",
                Column1 = "",
                Column2 = (targetPrice).ToString("C0"),
                TypeID = (int)NEstCostSummaryType.PriceB,
            };

            //TargetProfit
            var summary04 = new CR_Cost_Summary
            {
                Name = "Target Profit" + " (" + MyConvert.ConvertDoubleToPercentage(CostParameter.Configuration.TargetProfitRate)+ ")",

                ProjectID = CostParameter.ProjectID,
                EstRevID = CostParameter.EstRevID,
                EstItemID = CostParameter.EstItemID,
                WorkorderID = CostParameter.WorkorderID,
                WorkorderItemID = CostParameter.WorkorderItemID,

                OrderNumber = 4,
                Column0 ="",
                Column1 = (supplyOnlyCost - totalCostShop).ToString("C0"),
                Column2 = (installationCost - totalCostInstallation).ToString("C0"),
                TypeID = (int)NEstCostSummaryType.PriceB,
            };

       //     TotalBeforePriceBProfit = targetPrice + installationCost - totalCostInstallation;

            Values  = new List<CR_Cost_Summary>() { summary01, summary02, summary03, summary04, summary11, summary12, summary13, summary14 };
            TotalPrice = targetPrice;


#region ****************** Extra Cost *******************************
        
       
 


#endregion
        }

    }

    public class MyCostSummaryExtra : MyCostSummary
    {
        //Crating
        //Shipping
        //Local Installer
        public void Refresh()
        {
            if (CostItems == null) return;
            if (!CostItems.Any()) return;

            var t01 = CostItems.Where(x => x.TypeID == (int) NEstCostType.CratingCost).ToList();
            var cratingCost = t01.Count == 0 ? 0 : t01.Sum(x => x.SubTotal);

            var t02 = CostItems.Where(x => x.TypeID == (int) NEstCostType.ShippingCost).ToList();
            var shippingCost = t02.Count == 0 ? 0 : t02.Sum(x => x.SubTotal);

            var t03 = CostItems.Where(x => x.CategoryID == (int) NEstCostTypeCategory.LocalInstallerCost).ToList();
            var localInstallerCost = t03.Count == 0 ? 0 : t03.Sum(x => x.SubTotal);


            var summary01 = new CR_Cost_Summary
                                {
                                    Name = "Crating Cost(C.)",

                                    ProjectID = CostParameter.ProjectID,
                                    EstRevID = CostParameter.EstRevID,
                                    EstItemID = CostParameter.EstItemID,
                                    WorkorderID = CostParameter.WorkorderID,
                                    WorkorderItemID = CostParameter.WorkorderItemID,

                                    OrderNumber = 1,
                                    Column0 = "",
                                    Column1 = (Math.Round(cratingCost, 0)).ToString("C0"),
                                    Column2 = "",
                                    TypeID = (int) NEstCostSummaryType.PriceExtra,
                                };

            var summary02 = new CR_Cost_Summary
                                {
                                    Name = "Shipping Cost(S.)",

                                    ProjectID = CostParameter.ProjectID,
                                    EstRevID = CostParameter.EstRevID,
                                    EstItemID = CostParameter.EstItemID,
                                    WorkorderID = CostParameter.WorkorderID,
                                    WorkorderItemID = CostParameter.WorkorderItemID,

                                    OrderNumber = 2,
                                    Column0 = "",
                                    Column1 = (Math.Round(shippingCost, 0)).ToString("C0"),
                                    Column2 = "",
                                    TypeID = (int) NEstCostSummaryType.PriceExtra,
                                };


            var summary03 = new CR_Cost_Summary
                                {
                                    Name = "Local Installer Cost(L.)",

                                    ProjectID = CostParameter.ProjectID,
                                    EstRevID = CostParameter.EstRevID,
                                    EstItemID = CostParameter.EstItemID,
                                    WorkorderID = CostParameter.WorkorderID,
                                    WorkorderItemID = CostParameter.WorkorderItemID,

                                    OrderNumber = 3,
                                    Column0 = "",
                                    Column1 = (Math.Round(localInstallerCost, 0)).ToString("C0"),
                                    Column2 = "",
                                    TypeID = (int) NEstCostSummaryType.PriceExtra,
                                };

            TotalPrice = MyConvert.ConvertToDouble(cratingCost + shippingCost + localInstallerCost);
            var summary04 = new CR_Cost_Summary
            {
                Name = "C.S.L.",

                ProjectID = CostParameter.ProjectID,
                EstRevID = CostParameter.EstRevID,
                EstItemID = CostParameter.EstItemID,
                WorkorderID = CostParameter.WorkorderID,
                WorkorderItemID = CostParameter.WorkorderItemID,

                OrderNumber = 4,
                Column0 = "",
                Column1 = (Math.Round(TotalPrice, 0)).ToString("C0"),
                Column2 = "",
                TypeID = (int)NEstCostSummaryType.PriceExtra,
            };

            Values = new List<CR_Cost_Summary>() {summary01, summary02, summary03, summary04 };
        }

    }

    public class MyCostSummaryActualExtra : MyCostSummaryExtra
    {
        //1. Remove Crating
        //2. Add Permits
        //3. Modify Total

        //**********  After Refresh();
        public void RefreshPermitTotal(double permitTotal)
        {
            if (Values.Count == 0) return;
            var summary01 = Values.First(x => x.OrderNumber == (int)NEstCostSummaryExtra.Crating);
            var cratingTotal = MyConvert.ConvertAccountingFormatStringToDouble(summary01.Column1);
            summary01.Name = "Permits Cost(P.)";
            summary01.Column1 = (Math.Round(permitTotal, 0)).ToString("C0");
            var difference = permitTotal - cratingTotal;

            var summary04 = Values.First(x => x.OrderNumber == (int)NEstCostSummaryExtra.TotalOfCSL);

            summary04.Name = "P.S.L.";
            var cslTotal = MyConvert.ConvertAccountingFormatStringToDouble(summary04.Column1);
            TotalPrice = cslTotal + difference;
            summary04.Column1 = (Math.Round(TotalPrice, 0)).ToString("C0");

        }

    }

}