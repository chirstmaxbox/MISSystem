using System;
using System.Data;

namespace SalesCenterDomain.BDL.Project
{
    public class ProjectChildrenVersion
    {
        private readonly int _jobID;

        public ProjectChildrenVersion(int jobID)
        {
            _jobID = jobID;
        }


        public int NewestEstRev
        {
            get
            {
                var items = new ProjectChildrenItemTable(_jobID);
                return items.GetNewEstimationVersion();
            }
        }


        public int NewestQuoteRev
        {
            get
            {
                var items = new ProjectChildren(_jobID);
                DataTable tbl = items.Quote;

                int newestQuoteRev = 0;

                if (tbl != null)
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        int temp = Convert.ToInt32(row["quoteRev"]);
                        if (temp > newestQuoteRev)
                        {
                            newestQuoteRev = temp;
                        }
                    }
                }

                return newestQuoteRev + 1;
            }
        }
    }
}