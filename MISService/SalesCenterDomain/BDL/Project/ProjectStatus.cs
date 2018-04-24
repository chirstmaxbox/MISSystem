namespace SalesCenterDomain.BDL.Project
{
    public class ProjectStatus
    {
        private readonly int _jobID;

        public ProjectStatus(int jobID)
        {
            _jobID = jobID;
        }

        public bool StatusChanged
        {
            set
            {
                var fs = new FieldStatus(_jobID);
                fs.UpdateJobStatusChanged(value);
            }
        }

        public void ChnageTo(int jobStatus, int performer)
        {
            var fs = new FieldStatus(_jobID);
            fs.InsertNewStatus(jobStatus, performer);
            fs.UpdateJobStatusChanged(true);
        }
    }
}