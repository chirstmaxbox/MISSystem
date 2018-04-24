using System.Linq;

namespace ExportDomain.Parameter
{
    public class ExportParameterCrud
    {
        public ExportParameter Parameter { get { return _parameter; } }
        
        private ExportParameter _parameter;
        private readonly int _printingEmployeeID;
        private readonly ExportParameterDataContext _db;

        public ExportParameterCrud(int employeeID)
        {
            _printingEmployeeID = employeeID;
            _db = new ExportParameterDataContext();
            _parameter = _db.ExportParameters.SingleOrDefault(x => x.PrintingEmployeeID == _printingEmployeeID);
     
        }

        public void Save(ExportParameter ep)
        {
            if (_parameter == null)
            {
                _parameter = new ExportParameter
                            {
                                PrintingEmployeeID = ep.PrintingEmployeeID,
                                Kam = ep.Kam,
                                KamName = ep.KamName,
                                TeamID = ep.TeamID,
                                ReportID = ep.ReportID,
                                BeginDate = ep.BeginDate,
                                EndDate = ep.EndDate,
                                TeamType = ep.TeamType,
                                Cbx1Checked = ep.Cbx1Checked,
                                Cbx2Checked = ep.Cbx2Checked,
                                JobID = ep.JobID,
                                EstRevID = ep.EstRevID,
                                EstItemID =ep.EstItemID, 
                                QuoteRevID = ep.QuoteRevID,
                                WipTaskID = ep.WipTaskID,
                                WorkorderID = ep.WorkorderID,
                                InvoiceID = ep.InvoiceID,
                                CustomerID = ep.CustomerID,
                                CustomInt1 = ep.CustomInt1 ,
                                CustomInt2 =ep.CustomInt2 ,
                                CustomString1 =ep.CustomString1,
                                CustomString2 =ep.CustomString2 

                            };
                _db.ExportParameters.InsertOnSubmit(_parameter);
            }

            else
            {
                _parameter.Kam = ep.Kam;
                _parameter.KamName = ep.KamName;
                _parameter.TeamID = ep.TeamID;
                _parameter.ReportID = ep.ReportID;
                _parameter.BeginDate = ep.BeginDate;
                _parameter.EndDate = ep.EndDate;
                _parameter.TeamType = ep.TeamType;
                _parameter.Cbx1Checked = ep.Cbx1Checked;
                _parameter.Cbx2Checked = ep.Cbx2Checked;
                _parameter.JobID = ep.JobID;
                _parameter.EstRevID = ep.EstRevID;
                _parameter.EstItemID = ep.EstItemID; 
                _parameter.QuoteRevID = ep.QuoteRevID;
                _parameter.WipTaskID = ep.WipTaskID;
                _parameter.WorkorderID = ep.WorkorderID;
                _parameter.InvoiceID = ep.InvoiceID;
                _parameter.CustomerID = ep.CustomerID;
                _parameter.CustomInt1 = ep.CustomInt1;
                _parameter.CustomInt2 = ep.CustomInt2;
                _parameter.CustomString1 = ep.CustomString1;
                _parameter.CustomString2 = ep.CustomString2;

            }
            _db.SubmitChanges();

        }

    }
}