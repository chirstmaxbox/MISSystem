using System.Data;

namespace SalesCenterDomain.BDL.Project
{
    public class ProjectChildren
    {
        private readonly int _jobID;

        public ProjectChildren(int jobID)
        {
            _jobID = jobID;
        }

        public DataTable Estimation

        {
            get
            {
                var items = new ProjectChildrenItemTable(_jobID);
                return items.Estimation();
            }
        }


        public DataTable Quote

        {
            get
            {
                var items = new ProjectChildrenItemTable(_jobID);
                return items.Quote();
            }
        }


        public DataTable Workorder
        {
            get
            {
                var items = new ProjectChildrenItemTable(_jobID);
                return items.Workorder();
            }
        }

        public DataTable Invoice
        {
            get
            {
                var items = new ProjectChildrenItemTable(_jobID);
                return items.Invoice();
            }
        }
    }
}