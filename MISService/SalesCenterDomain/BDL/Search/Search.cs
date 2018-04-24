using System;
using System.Collections.Generic;
using System.Linq;
using MyCommon;
using MyCommon.Search;
using ProjectDomain;
using SalesCenterDomain.BLL;

namespace SalesCenterDomain.BDL.Search
{
    public class SearchFactory
    {
        public SearchFactory(String q, string teamIDStr, string myIDStr)
        {
            switch (SearchType(q))
            {
                case "Project":
                    if (IsQualifiedToSearchProject(q))
                    {
                        SearchClass = new SearchProjectByNumber(q, teamIDStr, myIDStr);

                    }
                    else
                    {
                        SearchClass = new SearchNull();
                    }
                    break;
 
                case "ProjectText":
                    if (IsQualifiedToSearchProject(q))
                    {
                        SearchClass = new SearchProject(q, teamIDStr, myIDStr);
                        
                    }
                    else
                    {
                        SearchClass = new SearchNull(); 
                    }
                    break;

                case "Workorder":

                    if (IsQualifiedToSearchWorkorder(q))
                    {
                        SearchClass = new SearchWorkorder(q, teamIDStr, myIDStr);
                    }
                    else
                    {
                        SearchClass = new SearchNull();
                    }
                    break;

                case "WorkorderByNumber":

                    if (IsQualifiedToSearchWorkorder(q))
                    {
                        SearchClass = new SearchWorkorderByNumber(q, teamIDStr, myIDStr);
                    }
                    else
                    {
                        SearchClass = new SearchNull();
                    }
                    break;

                case "Invoice":

                    if (IsQualifiedToSearchInvoice(q))
                    {
                        SearchClass = new SearchInvoice(q, teamIDStr, myIDStr);
                    }
                    else
                    {
                        SearchClass = new SearchNull();
                    }

                    break;

                case "InvoiceByNumber":

                    if (IsQualifiedToSearchInvoice(q))
                    {
                        SearchClass = new SearchInvoice(q, teamIDStr, myIDStr);
                    }
                    else
                    {
                        SearchClass = new SearchNull();
                    }

                    break;

                default:
                    SearchClass = new SearchNull();
                    break;
            }
        }
        private bool IsQualifiedToSearchProject(string q)
        {
            q = q.Trim();
            if (q.Length < 4) return false;

            const char p = 'p';
            const char P = 'P';
            var ca =q.ToCharArray();
            if (MyConvert.IsNumeric(ca[0]) & MyConvert.IsNumeric(ca[1]) & (ca[2]==p | ca[2]==P) )
            {
                if (q.Length < 7) return false;
            }

            return true;
        }

        private bool IsQualifiedToSearchWorkorder(string q)
        {
            q = q.Trim();
            if (q.Length < 4) return false;

            const char p = 'j';
            const char P = 'J';
            var ca = q.ToCharArray();
            if (MyConvert.IsNumeric(ca[0]) & MyConvert.IsNumeric(ca[1]) & (ca[2] == p | ca[2] == P))
            {
                if (q.Length < 6) return false;
            }

            return true;
        }

        private bool IsQualifiedToSearchInvoice(string q)
        {
            q = q.Trim();
            if (q.Length < 4) return false;

            const char p = 'v';
            const char P = 'V';
            var ca = q.ToCharArray();
            if (MyConvert.IsNumeric(ca[0]) & MyConvert.IsNumeric(ca[1]) & (ca[2] == p | ca[2] == P))
            {
                if (q.Length < 6) return false;
            }

            return true;
        }


        public ISearch SearchClass { get; set; }

        private string SearchType(string q)
        {

            string searchType = "ProjectText";
            string s1 = MyString.Left(q, 2);
            if (MyConvert.ConvertToInteger(s1) >= 10)
            {
                string s2 = MyString.Left(q, 3);
                if (s2.ToUpper().Contains("P"))
                {
                    searchType = "Project";
                }
                if (s2.ToUpper().Contains("J"))
                {
                    searchType = "WorkorderByNumber";
                }

                if (s2.ToUpper().Contains("V"))
                {
                    searchType = "InvoiceByNumber";
                }
            }

            return searchType;
        }
    }

    #region  ---------------------- Objects -------------------

    public class SearchProject : ISearch
    {
        public SearchProject(String q, string teamIDStr, string myIDStr)
        {
            int teamID = Convert.ToInt32(teamIDStr);
            int myID = Convert.ToInt32(myIDStr);

            var itemOptions = new List<ItemOption>();

            var dbml = new ProjectModelDbEntities();
            //?
            
            List<Sales_JobMasterList> projects =
                dbml.Sales_JobMasterList.Where(x => (x.FW_Employees.Team == teamID | teamID == 0) &
                                                    (myID == x.sales | myID == x.sa1ID | myID == 5020) |
                                                    myID == x.sales |
                                                    myID == x.sa1ID
                    ).OrderBy(x => x.jobTitle).ToList();

            string[] search = q.Split(' ');

            try
            {
                foreach (Sales_JobMasterList item in projects)
                {
                    if (IsThisJobAlreadSelected(item.jobID, itemOptions)) continue;

                    string namevalue = item.jobTitle + " - " + item.jobNumber;
                    if (!MyString.IsContainsAny(namevalue, search)) continue;


                    var st = new ItemOption
                                 {
                                     Name = namevalue,
                                     SelectedID = Convert.ToString(item.jobID),
                                     Value = namevalue
                                 };
                    itemOptions.Add(st);
                }
            }


            catch (Exception ex)
            {
                string exp = ex.ToString(); //Setup a breakpoint here to verify any exceptions raised.
            }

            //Return
            Items = itemOptions;
        }

        #region ISearch Members

        public List<ItemOption> Items { get; set; }

        #endregion

        private bool IsThisJobAlreadSelected(int jobID, IEnumerable<ItemOption> itemOptions)
        {
            bool b = false;
            foreach (ItemOption item in itemOptions)
            {
                if (jobID == Convert.ToInt32(item.SelectedID))
                {
                    b = true;
                    break;
                }
            }
            return b;
        }
    }

    public class SearchProjectByNumber : ISearch
    {
            public SearchProjectByNumber(String q, string teamIDStr, string myIDStr)
            {
                int teamID = Convert.ToInt32(teamIDStr);
                int myID = Convert.ToInt32(myIDStr);

                var itemOptions = new List<ItemOption>();

                var dbml = new ProjectModelDbEntities();
                string[] search = q.Split(' ');
                var projectNumber = search[0];
                List<Sales_JobMasterList> projects;
                
                if (projectNumber.Length ==8)
                {
                    projects = dbml.Sales_JobMasterList.Where(x => x.jobNumber == projectNumber).ToList();     
                }
                else
                {
                    projects = dbml.Sales_JobMasterList.Where(x => x.jobNumber.Contains(projectNumber)).ToList();        
                }
               

                projects = projects.Where(x => (x.FW_Employees.Team == teamID | teamID == 0) &
                                               (myID == x.sales | myID == x.sa1ID | myID == 5020) |
                                               myID == x.sales |
                                               myID == x.sa1ID
                    ).OrderBy(x => x.jobTitle).ToList();

                if (projects.Any())
                {
                    foreach (Sales_JobMasterList item in projects)
                    {
                        string namevalue = item.jobTitle + " - " + item.jobNumber;
                        var st = new ItemOption
                                     {
                                         Name = namevalue,
                                         SelectedID = Convert.ToString(item.jobID),
                                         Value = namevalue
                                     };
                        itemOptions.Add(st);
                    }
                }
                //Return
                Items = itemOptions;
            }

            #region ISearch Members

        public List<ItemOption> Items { get; set; }

        #endregion

        private bool IsThisJobAlreadSelected(int jobID, IEnumerable<ItemOption> itemOptions)
        {
            bool b = false;
            foreach (ItemOption item in itemOptions)
            {
                if (jobID == Convert.ToInt32(item.SelectedID))
                {
                    b = true;
                    break;
                }
            }
            return b;
        }
    }


    public class SearchWorkorderByNumber : ISearch
    {
        public SearchWorkorderByNumber(String q, string teamIDStr, string myIDStr)
        {
            string[] search = q.Split(' ');
            var workorderNumber = search[0];
            int teamID = Convert.ToInt32(teamIDStr);
            int myID = Convert.ToInt32(myIDStr);
            
            var itemOptions = new List<ItemOption>();

            var dbml = new ProjectModelDbEntities();

            List<Sales_JobMasterList_WO> query;                
                if (workorderNumber.Length ==7)
                {
                   query  = dbml.Sales_JobMasterList_WO.Where(x =>x.WorkorderNumber== workorderNumber).ToList();     
                }
                else
                {
                   query  = dbml.Sales_JobMasterList_WO.Where(x =>x.WorkorderNumber.Contains( workorderNumber)).ToList();     
                }
             query =query.Where(
                x => x.FW_Employees.Team == teamID && myID == 5020 ||
                     (teamID == 0 && myID == x.Sa1ID || myID == x.Sales || myID == 5020) ||
                     myID == x.Sa1ID ||
                     myID == x.Sales
                ).OrderBy(x => x.WorkorderNumber).ToList();

            try
            {
                foreach (Sales_JobMasterList_WO item in query)
                {
                    string namevalue = item.WorkorderNumber + "V" + item.woRev.ToString() + "-" + item.jobTitle;

                    var st = new ItemOption
                    {
                        Name = namevalue,
                        SelectedID = Convert.ToString(item.woID),
                        Value = namevalue
                    };
                    itemOptions.Add(st);
                }
            }

            catch (Exception ex)
            {
                string exp = ex.ToString(); //Setup a breakpoint here to verify any exceptions raised.
            }


            Items = itemOptions;
        }

        public List<ItemOption> Items { get; set; }

    }


    public class SearchWorkorder : ISearch
    {
        public SearchWorkorder(String q, string teamIDStr, string myIDStr)
        {
            string[] search = q.Split(' ');
            int teamID = Convert.ToInt32(teamIDStr);
            int myID = Convert.ToInt32(myIDStr);

            var itemOptions = new List<ItemOption>();

            var dbml = new ProjectModelDbEntities();
            List<Sales_JobMasterList_WO> query = dbml.Sales_JobMasterList_WO.Where(
                x => x.FW_Employees.Team == teamID && myID == 5020 ||
                     (teamID == 0 && myID == x.Sa1ID || myID == x.Sales || myID == 5020) ||
                     myID == x.Sa1ID ||
                     myID == x.Sales
                ).OrderBy(x => x.WorkorderNumber).ToList();

            try
            {
                foreach (Sales_JobMasterList_WO item in query)
                {
                    string namevalue = item.WorkorderNumber + "V" + item.woRev.ToString() + "-" + item.jobTitle;
                    if (!MyString.IsContainsAny(namevalue, search)) continue;

                    var st = new ItemOption
                    {
                        Name = namevalue,
                        SelectedID = Convert.ToString(item.woID),
                        Value = namevalue
                    };
                    itemOptions.Add(st);
                }
            }

            catch (Exception ex)
            {
                string exp = ex.ToString(); //Setup a breakpoint here to verify any exceptions raised.
            }


            Items = itemOptions;
        }

        public List<ItemOption> Items { get; set; }

    }

    public class SearchInvoice : ISearch
    {
        public SearchInvoice(String q, string teamIDStr, string myIDStr)
        {
            int teamID = Convert.ToInt32(teamIDStr);
            int myID = Convert.ToInt32(myIDStr);

            Items = new List<ItemOption>();
            var mi = new MyInvoice();
            List<Sales_JobMasterList_Invoice> items = mi.GetInvoicesBySalesID(myID);


            try
            {
                string[] search = q.Split(' ');

                if (items.Count == 0) return;
                foreach (Sales_JobMasterList_Invoice item in items)
                {
                    string namevalue = item.invoiceNo + "V" + item.Revision.ToString() + "-" + item.Title;
                    if (!MyString.IsContainsAny(namevalue, search)) continue;

                    var st = new ItemOption
                                 {
                                     Name = namevalue,
                                     SelectedID = Convert.ToString(item.invoiceID),
                                     Value = namevalue
                                 };
                    Items.Add(st);
                }
            }


            catch (Exception ex)
            {
                string exp = ex.ToString(); //Setup a breakpoint here to verify any exceptions raised.
            }
        }

        #region ISearch Members

        public List<ItemOption> Items { get; set; }

        #endregion
    }

    public class SearchInvoiceByNumber : ISearch
    {
        public List<ItemOption> Items { get; set; }

        public SearchInvoiceByNumber(String q, string teamIDStr, string myIDStr)
        {
            Items = new List<ItemOption>();

            int teamID = Convert.ToInt32(teamIDStr);
            int myID = Convert.ToInt32(myIDStr);
            string[] search = q.Split(' ');
            string invNumber = search[0];
            List<Sales_JobMasterList_Invoice> invoices;
            var db = new ProjectModelDbEntities();
            if (invNumber.Length ==7)
            {
                invoices  = db.Sales_JobMasterList_Invoice.Where(x => x.invoiceNo == invNumber).ToList();
            }
            else
            {
                invoices  = db.Sales_JobMasterList_Invoice.Where(x => x.invoiceNo.Contains(invNumber)).ToList();
            }

            invoices =invoices.Where(x => myID == x.Sales || myID == x.SA1 || myID == 5020).OrderBy(x => x.invoiceNo).ToList();

                if (invoices.Count == 0) return;
                foreach (Sales_JobMasterList_Invoice item in invoices)
                {
                    string namevalue = item.invoiceNo + "V" + item.Revision.ToString() + "-" + item.Title;
                
                    var st = new ItemOption
                    {
                        Name = namevalue,
                        SelectedID = Convert.ToString(item.invoiceID),
                        Value = namevalue
                    };
                    Items.Add(st);
                }
            }

    }

    public class SearchNull : ISearch
    {
        public SearchNull()
        {
            Items = new List<ItemOption>();
        }

        #region ISearch Members

        public List<ItemOption> Items { get; set; }

        #endregion
    }

    #endregion
}