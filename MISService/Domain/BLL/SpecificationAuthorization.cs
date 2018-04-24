
using System.Data.Entity;
using EmployeeDomain.BLL;
using SpecDomain.BLL.EstTitle;
using System.Linq;
using SpecDomain.Model;

namespace SpecDomain.BLL
{
    public class MySpecificationAuthorization
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        public ParameterSpecification Value { get; set; }

        //Initialize and Then Save
        public MySpecificationAuthorization(int employeeID)
        {
            Value = _db.ParameterSpecifications.FirstOrDefault(x => x.EmployeeID == employeeID);
            if (Value == null)
            {
                Value = new ParameterSpecification
                            {
                                EmployeeID = employeeID, 
                                EstRevID = 0, 
                                ProjectID = 0
                            };
                _db.ParameterSpecifications.Add( Value);
                _db.SaveChanges();
            }
        }

        public void Authorization(int estRevID)
        {

            var employeesEnabledSpecificationTemplateEdit = new int[] {12, 28, 8, 93};
            var employeesEnabledCostTemplateEdit = new int[] {12, 28, 8, 93};

            Value.IsAuthorized = false;
            Value.IsResponseOwner = false;
            Value.IsRequestOwner = false;
            Value.IsLocked = false;
            Value.IsSpecificationTemplateEditEnabled = false;
            Value.IsCostTemplateEditEnabled = false;

            var role = new EmployeeRole(Value.EmployeeID);

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageEstimationEstimatorOnlyControlsEnabled))
            {
                Value.IsAuthorized = true;
                Value.IsResponseOwner = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageEstimationModifyContentsEnabled))
            {
                Value.IsAuthorized = true;
                Value.IsRequestOwner = true;
            }

          //  if (!Value.IsRequestOwner) return;
            
            Value.EstRevID = estRevID;
            var est = _db.Sales_JobMasterList_EstRev.Find(estRevID);
            Value.ProjectID = est.JobID;
            var job = new SpecProjectDetail(est.JobID );
            if (Value.EmployeeID != job.Sales && Value.EmployeeID != job.Sa1ID)
            {
                Value.IsRequestOwner = false;
            }

            if (Value.EmployeeID == 12 || Value.EmployeeID == 28)
            {
                Value.IsRequestOwner = true;
                Value.IsResponseOwner = true;
            }

            //
            Value.IsSpecificationTemplateEditEnabled = employeesEnabledSpecificationTemplateEdit.Contains(Value.EmployeeID);
            Value.IsCostTemplateEditEnabled = employeesEnabledCostTemplateEdit.Contains(Value.EmployeeID);

            _db.Entry(Value).State = EntityState.Modified; 
            _db.SaveChanges();
        }

    }

  
}