
namespace SpecDomain.BLL.Cost
{
    public class MyCostItem
    {

            public int CostItemID { get; set; }
            public int CategoryID { get; set; }
       //     public string CategoryName { get; set; }
            public int TypeID { get; set; }
            public string  TypeName { get; set; }
            public int OrderNumber { get; set; }
            public string Name { get; set; }
            public string Unit { get; set; }
            public double UnitPrice { get; set; }
            public double Qty { get; set; }
            public double SubTotal { get; set; }
            public int WoID { get; set; }

        //For veiw
            public bool Checked { get; set; }

            public int TempInt1 { get; set; }       //Actual.Material.WorkorderNumber 
            public string TempString1 { get; set; }   //Actual.LabourTicket.Department_Name
    }

  }