using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using EmployeeDomain.Models;
using MyCommon;
using SpecDomain.Model;

namespace SpecDomain.BLL.EstItem
{
    public class MyCrEstimationRequisitionItem
    {
        public List<CR_EstimationRequisitionItem> Items { get; set; }

        private readonly int _printingEmployeeID;
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public MyCrEstimationRequisitionItem(int estRevID, long estItemID,  int employeeID)
        {
            _printingEmployeeID = employeeID;
            Items = new List<CR_EstimationRequisitionItem>();

            if (estItemID>0 )
            {
                var estItem = _db.EST_Item.Find(estItemID);
                Items.AddRange(GetItems(estItem));
                estItem.Size = GetSize(estItem.EstItemID, estItem.Product.CategoryID);
                estItem.ReportDescription = GetReportDescription(estItem);
                _db.Entry(estItem).State = EntityState.Modified;
            }
            else
            {
                var estItems = _db.EST_Item.Where(x => x.EstRevID == estRevID).ToList();
                foreach (var estItem in estItems)
                {
                    Items.AddRange(GetItems(estItem));
                    estItem.Size = GetSize(estItem.EstItemID, estItem.Product.CategoryID);
                    estItem.ReportDescription = GetReportDescription(estItem);
                    _db.Entry(estItem).State = EntityState.Modified;
                }
            }

            _db.SaveChanges();
        }

        private IEnumerable<CR_EstimationRequisitionItem> GetItems(EST_Item estItem)
        {
            //order by ItemID-->CrColumnCount(D)-->orderNumber
            var items = new List<CR_EstimationRequisitionItem>();

            var specialFields = _db.EST_Item_Specification.Where(x => x.EstItemID == estItem.EstItemID).OrderBy(x => x.OrderNumber).ToList();

            if (!specialFields.Any()) return items;
            int i = 1;
            foreach (var sField in specialFields)
            {
                var crSpecial = new CR_EstimationRequisitionItem
                                    {
                                        PrintingEmployeeID = _printingEmployeeID,

                                        EstItemID = estItem.EstItemID,

                                        OrderNumber = i++,
                                        Title = sField == null ? "" : sField.Title,
                                        Contents = sField == null ? "" : sField.Contents,

                                    };

                items.Add(crSpecial);
            }

            return items;
        }

        
        public string GetReportDescription(EST_Item estItem)
        {
            var specialFields = _db.EST_Item_Specification.Where(x => x.EstItemID ==estItem.EstItemID).OrderBy(x => x.OrderNumber).ToList();

            if (!specialFields.Any()) return estItem.Description;
            var s = estItem.Description + System.Environment.NewLine;
            var i = 1;
            foreach (var field in specialFields)
            {
                if (!MyConvert.IsNullString(field.Contents))
                {
                    s += Convert.ToString(i) + ") " + field.Title + ": " + field.Contents + Environment.NewLine;// "<br />";
                    i++;
                }
            }

            return s;
        }


        private string GetSize(long estItemID, int productCategoryID)
            {
                var itemSizes = _db.EST_Item_Specification_Size.Where(x => x.EstItemID == estItemID).OrderBy(x => x.EstItemSizeID).ToList();
                var itemSizeTempalte = _db.ProductCategories.Find(productCategoryID ).ProductSizes.First() ;

                var s = "";
                foreach (var size in itemSizes)
                {
                    if (itemSizeTempalte.IsWidthEnabled)
                    {
                        s += "Width: " + size.WidthFeet +" Feet " + size.WidthInch + " Inches  x  ";
                    }

                    if (itemSizeTempalte.IsHeightEnabled)
                    {
                        s += "  Height: " + size.HeightFeet + " Feet " + size.HeightInch  + " Inches  x  "; ;
                    }

                    if (itemSizeTempalte.IsThicknessEnabled)
                    {
                        s += "  Thickness: " + size.ThicknessFeet  + " Feet " + size.ThicknessInch  + " Inches  x  "; 
                    }

                    if (itemSizeTempalte.IsPcEnabled)
                    {
                        s += "    " + size.Pc.ToString("D1") + " PC(s)";
                    }

                    s += Environment.NewLine;
                }

                return s;

            }

        public void Refresh()
        {
     //          var endTime = DateTime.Now;
     //    var elapsedMillisecs = ((TimeSpan)(endTime - startTime)).TotalMilliseconds;

            //Print Problem
            var existingItems = _db.CR_EstimationRequisitionItem.Where(x => x.PrintingEmployeeID == _printingEmployeeID & x.ID > 10).ToList();
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
                _db.CR_EstimationRequisitionItem.Add(item);
            }
            _db.SaveChanges();

        }

        //private void DeleteExisting()
        //{
        //    //1. Define the destination
        //    var connectionSQL = new SqlConnection(TimeCardConfiguration.ConnectionString);
        //    const string sqlSelectString2 = "SELECT * FROM [CR_TIMECARD] WHERE [printingEmployeeID]=@printingEmployeeID ";
        //    var selectCommand2 = new SqlCommand(sqlSelectString2, connectionSQL);
        //    var adapter2 = new SqlDataAdapter(selectCommand2);
        //    adapter2.SelectCommand.Parameters.Add("@printingEmployeeID", SqlDbType.Int).Value =_printingEmployeeID;

        //    var ds2 = new DataSet();
        //    ds2.Tables.Clear();

        //    try
        //    {
        //        connectionSQL.Open();
        //        adapter2.Fill(ds2, "t2");
        //        //Original Datatble
        //        if (ds2.Tables["t2"] == null) return;
        //        if (ds2.Tables["t2"].Rows.Count <= 0) return;

        //        //delete existing
        //        foreach (DataRow row in ds2.Tables["t2"].Rows)
        //        {
        //            row.Delete();
        //        }

        //        //Write
        //        var cb = new SqlCommandBuilder(adapter2);
        //        adapter2 = cb.DataAdapter;
        //        adapter2.Update(ds2, "t2");


        //    }
        //    catch (SqlException ex)
        //    {
        //        string errorLog = ex.Message;
        //    }
        //    finally
        //    {
        //        connectionSQL.Close();
        //    }

        //}

        //private void CreateNew(List<CR_TimeCard> values)
        //{
         
        //    //1. Define the destination
        //    var connectionSQL = new SqlConnection(TimeCardConfiguration.ConnectionString);
        //    const string sqlSelectString2 = "SELECT * FROM [CR_TIMECARD] WHERE [printingEmployeeID]=-1 ";
        //    var selectCommand2 = new SqlCommand(sqlSelectString2, connectionSQL);
        //    var adapter2 = new SqlDataAdapter(selectCommand2);
        //    var ds2 = new DataSet();
        //    ds2.Tables.Clear();

        //    try
        //    {
        //        connectionSQL.Open();
        //        adapter2.Fill(ds2, "t2");

        //        foreach (var item in values )
        //        {
        //            var rowNew =   ds2.Tables["t2"].NewRow();
        //            //new row
        //            rowNew = GetNewDataRowPresent(rowNew, item);
        //            ds2.Tables["t2"].Rows.Add(rowNew);
        //        }

        //        //Write
        //        var cb = new SqlCommandBuilder(adapter2);
        //        adapter2 = cb.DataAdapter;
        //        adapter2.Update(ds2, "t2");

        //    }
        //    catch (SqlException ex)
        //    {
        //        string errorLog = ex.Message;
        //    }
        //    finally
        //    {
        //        connectionSQL.Close();
        //    }

        //}

        //private DataRow GetNewDataRowPresent(DataRow rowNew, CR_TimeCard lt)
        //{
        //    //rowNew["tcID { get; set; }
        //    //Infor
        //    rowNew["TRANSACTION_ID"] = lt.TRANSACTION_ID;
        //    rowNew["EMPLOYEE_ID"] = lt.EMPLOYEE_ID;
        //    rowNew["DEPARTMENT_ID"] = lt.DEPARTMENT_ID;
        //    rowNew["CLOCK_IN"] = lt.CLOCK_IN;
        //    rowNew["CLOCK_OUT"] = lt.CLOCK_OUT;

        //    rowNew["PrintingEmployeeID"] = _printingEmployeeID;
        //    rowNew["eID"] = lt.eID;
        //    rowNew["eName"] = lt.eName;
        //    rowNew["Hours"] = lt.HOURS_OVERALL;
        //    rowNew["DepartmentName"] = lt.DepartmentName;

        //    rowNew["HOURS_WORKED"] = lt.HOURS_WORKED;
        //    rowNew["HOURS_BREAK"] = lt.HOURS_BREAK; // After Break
        //    rowNew["HOURS_PREVIOUS"] = lt.HOURS_PREVIOUS; // Overtime;
        //    rowNew["HOURS_OVERALL"] = lt.HOURS_OVERALL; // Paidhours= WorkedHour-Break +Overtime
        //    rowNew["SETUP_COMPLETED"] = lt.SETUP_COMPLETED; //Y: Normal; N:Abnormal; H: Holiday; S: Substitude;
        //    rowNew["pID"] = lt.pID; //Payroll Number
        //    rowNew["EarningCode"] = lt.EarningCode;
        //    rowNew["SubstitudeID"] = lt.SubstitudeID;
        //    rowNew["OvertimeID"] = lt.OvertimeID;
        //    rowNew["ShiftID"] = lt.ShiftID;
        //    rowNew["PayableClockIn"] = lt.PayableClockIn;
        //    rowNew["PayableClockOut"] = lt.PayableClockOut;
        //    rowNew["WeekNumber"] = lt.WeekNumber;
        //    return rowNew;
        //}




    }

}
