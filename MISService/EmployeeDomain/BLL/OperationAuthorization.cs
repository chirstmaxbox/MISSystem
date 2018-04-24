
using MyCommon;
using MyCommon.MyEnum;

namespace EmployeeDomain.BLL
{
#region  ----------------- Definition --------------------
        public interface IOperationAuthorization
    {
        bool IsAuthorized { get; set; }
        bool IsAuthorizedEditEnabled { get; set; }
        bool IsAuthorizedPageSpecial01 { get; set; }
        bool IsTeamSelectEnabled { get; set; }
        bool IsAttachDocumentEnabled { get; set; }
    }

    public class OperationAuthorizationBase : IOperationAuthorization
    {
        public bool IsAuthorized { get; set; }
        public bool IsAuthorizedEditEnabled { get; set; }
        public bool IsAuthorizedPageSpecial01 { get; set; }
        public bool IsTeamSelectEnabled { get; set; }
        public bool IsAttachDocumentEnabled { get; set; }

        public OperationAuthorizationBase()
        {
            IsAuthorized = false;
            IsAuthorizedEditEnabled = false;

            IsAuthorizedPageSpecial01 = false;

            IsTeamSelectEnabled = false;
            IsAttachDocumentEnabled = false;
        }
    }

#endregion 

    //Factory
    public class OperationAuthorization
    {
        public IOperationAuthorization Authorizor { get; set; }

        public OperationAuthorization(int pageID, int employeeID)
        {
            switch (pageID)
            {
                case (int) NPageID.ProjectDefault:
                case (int) NPageID.ProjectDetails: 
                case (int) NPageID.ProjectBIQ :
                case (int) NPageID.ProjectFollowUp: 
                case (int) NPageID.WipPublic :
                case (int) NPageID.WipDefault  :
                case (int) NPageID.WipProject:           
                case (int) NPageID.Quotation:

                    Authorizor = new OAProject(employeeID);
                    break;
                case (int) NPageID.Estimation:
                   Authorizor = new OAEstimation(employeeID);
                    break;
     
                case (int) NPageID.InvoiceDetail:
                case (int) NPageID.InvoiceDefault:
                    Authorizor = new OAInvoice(employeeID);
                    break;
                
                case (int) NPageID.CustomerDefault: 
                case (int) NPageID.CustomerDetail: 
                case (int) NPageID.CustomerOrganize:
                    Authorizor = new OACustomer(employeeID);
                    break;

                case (int) NPageID.WorkorderDefault: 
                case (int) NPageID.WorkorderDetail: 
                case (int) NPageID.WorkorderItem:
                case (int) NPageID.WorkorderDoc: 
                case (int) NPageID.WorkorderInstruction: 
                case (int) NPageID.WorkorderValidation: 
				    Authorizor = new OAWorkorder(employeeID);
                    break;
				case 156:
					Authorizor = new OAWorkorderAddressLabel(employeeID);

                    break;

                case (int) NPageID.EmployeeBasic :
                    Authorizor = new OAEmployee(employeeID);
                    break;

                case (int) NPageID.CommissionInvoice :
                case (int) NPageID.CommissionList:
                    Authorizor = new OACommission(employeeID);
                    break;

                case (int) NPageID.ResponseDefault :
                    Authorizor = new OAResponse(employeeID);
                    break;

                case (int) NPageID.Configuration  :
                    Authorizor = new OAAllowAll(employeeID);
                    break;

                case (int) NPageID.SiteCheckSchedule :
                    Authorizor = new OASiteCheckSchedule(employeeID);
                    break;

                case (int) NPageID.SubContract  :
                    Authorizor = new OASubContract(employeeID);
                    break;
                case (int) NPageID.WorkorderApproval :
                    Authorizor = new OAWorkorderApproval(employeeID);
                    break;
               
                default:
                    //OperationAuthorizationClass= new OADenied();
                    break;
            }
        }
    }




#region  ################### Project #########################
        public class OAProject : OperationAuthorizationBase
    {

        public OAProject(int employeeID)
        {

            var role = new EmployeeRole(employeeID);
            if (role.IsInRoles(FsMembershipAuthorizationArray.PageProjectReadonly))
            {
                IsAuthorized = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageProjectTeamSelectEnabled))
            {

                IsAuthorized = true;
                IsTeamSelectEnabled = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageProjectEditEnabled))
            {
                IsAuthorized = true;
                IsAuthorizedEditEnabled = true;
            }
        }
    }

 
#endregion 


    
#region  ################### Invoice #########################
        public class OAInvoice : OperationAuthorizationBase
    {
        public OAInvoice(int employeeID)
        {
            var role = new EmployeeRole(employeeID);
            if (role.IsInRoles(FsMembershipAuthorizationArray.PageInvoiceReadOnly))
            {
                IsAuthorized = true;
                IsTeamSelectEnabled = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageInvoiceEdit))
            {
                IsAuthorized = true;
                IsAuthorizedEditEnabled = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.Accounting))
            {
                IsAuthorized = true;
                IsAuthorizedEditEnabled = true;
                IsAuthorizedPageSpecial01 = true;
                IsTeamSelectEnabled = true;
            }
        }
    }

#endregion 


    
#region  ################### Customer  #########################
        public class  OACustomer : OperationAuthorizationBase
    {
        public  OACustomer(int employeeID)
        {
            var role = new EmployeeRole(employeeID);

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageProjectEditEnabled ))
            {
                IsAuthorized = true;
                IsAuthorizedEditEnabled = true;
            }
            
            if (role.IsInRoles(FsMembershipAuthorizationArray.PageClientList  ))
            {
                IsAuthorized = true;
                IsAuthorizedEditEnabled = true;
                IsAuthorizedPageSpecial01 = true;
                IsTeamSelectEnabled = true;

            }
        }

    }

#endregion 


    
#region  ################### Estimaition #########################
        public class OAEstimation : OperationAuthorizationBase
    {

        public OAEstimation(int employeeID)
        {

            var role = new EmployeeRole(employeeID);
            if (role.IsInRoles(FsMembershipAuthorizationArray.PageEstimationReadonly))
            {
                IsAuthorized = true;
             }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageEstimationEstimatorOnlyControlsEnabled))
            {
                IsAuthorized = true;
                IsAuthorizedPageSpecial01 = true;
                IsAttachDocumentEnabled = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageEstimationAttachDocumentsEnabled))
            {
                IsAuthorized = true;
                
                IsAttachDocumentEnabled = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageEstimationModifyContentsEnabled))
            {
                IsAuthorized = true;
                IsAuthorizedEditEnabled = true;
                IsAuthorizedPageSpecial01 = true;
                IsTeamSelectEnabled = true;
                IsAttachDocumentEnabled = true;
            }
        }
    }


#endregion 


        
#region  ################### Workkorder #########################
        public class OAWorkorder : OperationAuthorizationBase
    {

        public OAWorkorder(int employeeID)
        {

            var role = new EmployeeRole(employeeID);
            if (role.IsInRoles(FsMembershipAuthorizationArray.PageWorkorderReadonly))
            {
                IsAuthorized = true;
             }


            if (role.IsInRoles(FsMembershipAuthorizationArray.PageWorkorderAttachDoucmentsEnabled))
            {
                IsAuthorized = true;
                
                IsAttachDocumentEnabled = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageWorkorderModifyContentsEnabled))
            {
                IsAuthorized = true;
                IsAttachDocumentEnabled = true;

                IsAuthorizedEditEnabled = true;
  
            }
        }
    }


		public class OAWorkorderAddressLabel : OperationAuthorizationBase
		{

			public OAWorkorderAddressLabel(int employeeID)
			{
				//?
				var role = new EmployeeRole(employeeID);
				
				//if (role.IsInRoles(FsMembershipAuthorizationArray.PageWorkorderReadonly))
				//{
					IsAuthorized = true;
				//}


				if (role.IsInRoles(FsMembershipAuthorizationArray.PageWorkorderAttachDoucmentsEnabled))
				{
					IsAuthorized = true;

					IsAttachDocumentEnabled = true;
				}

				if (role.IsInRoles(FsMembershipAuthorizationArray.PageWorkorderModifyContentsEnabled))
				{
					IsAuthorized = true;
					IsAttachDocumentEnabled = true;

					IsAuthorizedEditEnabled = true;

				}
			}
		}




#endregion 


            
#region  ################### Employee #########################
        public class OAEmployee : OperationAuthorizationBase
    {

        public OAEmployee(int employeeID)
        {

            var role = new EmployeeRole(employeeID);
            if (role.IsInRoles(FsMembershipAuthorizationArray.PageEmployeeManagement))
            {
                IsAuthorized = true;
                IsAuthorizedEditEnabled = true;
                IsAuthorizedPageSpecial01 = true;
                IsTeamSelectEnabled = true;
                IsAttachDocumentEnabled = true;
            }


        }
    }


#endregion 

  
#region  ################### Commission #########################
        public class OACommission : OperationAuthorizationBase
    {

        public OACommission(int employeeID)
        {
            var role = new EmployeeRole(employeeID);

            //Accouinting Must Put in the first Place or  IsAuthorizedPageSpecial01 = false;
            if (role.IsInRoles(FsMembershipAuthorizationArray.Accounting))
            {
                IsAuthorized = true;
                IsAuthorizedPageSpecial01 = false;
                IsTeamSelectEnabled = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageSalesCommissionReadonly))
            {
                IsAuthorized = true;
                IsAuthorizedPageSpecial01 = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageSalesCommissionViewAll))
            {
                IsAuthorized = true;
                IsAuthorizedPageSpecial01 = true;
                IsTeamSelectEnabled = true;
        
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageSalesCommissionManupulate))
            {
                IsAuthorized = true;
                IsAuthorizedEditEnabled = true;
                IsAuthorizedPageSpecial01 = true;
                IsTeamSelectEnabled = true;

            }


            

        }
    }


#endregion 

#region  ################### Response #########################
   public class OAResponse : OperationAuthorizationBase
    {

        public OAResponse(int employeeID)
        {
            var role = new EmployeeRole(employeeID);

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageResponse ))
            {
                IsAuthorized = true;
                IsTeamSelectEnabled = true;
             }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageResponseStatusEnabled ))
            {
                IsAuthorized = true;
                IsAuthorizedEditEnabled = true;
                IsTeamSelectEnabled = true;
                IsAttachDocumentEnabled = true;

            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageApprovalWorkorderEnabled))
            {
               IsAuthorized = true;
                IsAuthorizedEditEnabled = true;
                IsTeamSelectEnabled = true;
                IsAttachDocumentEnabled = true;
                IsAuthorizedPageSpecial01 = true;
            }

        }
    }


#endregion 

    #region  ################### Approval Workorder #########################
   public class OAWorkorderApproval : OperationAuthorizationBase
    {

        public OAWorkorderApproval(int employeeID)
        {
            var role = new EmployeeRole(employeeID);
            if (role.IsInRoles(FsMembershipAuthorizationArray.PageApprovalWorkorderEnabled))
            {
                IsAuthorized = true;
             }

        }
    }


#endregion 



    #region  ################### SubContract Installation  #########################
   public class OASubContract : OperationAuthorizationBase
    {

        public OASubContract(int employeeID)
        {
            var role = new EmployeeRole(employeeID);

                IsAuthorized = true;
            
            if (role.IsInRoles(FsMembershipAuthorizationArray.PageSubContractRequestEnabled  ))
            {
                IsAuthorizedEditEnabled = true;
                IsAttachDocumentEnabled = true;

            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageApprovalWorkorderEnabled))
            {
                IsTeamSelectEnabled = true;
                IsAttachDocumentEnabled = true;
                IsAuthorizedPageSpecial01 = true;
            }

        }
    }


#endregion 


 
 #region  ################### Allow All #########################
        public class OAAllowAll : OperationAuthorizationBase
    {

        public OAAllowAll(int employeeID)
        {
                IsAuthorized = true;
                IsAuthorizedEditEnabled = false ;
                IsAuthorizedPageSpecial01 = false ;
                IsTeamSelectEnabled = false ;
                IsAttachDocumentEnabled = false ;
      
        }
    }


#endregion 
 

    #region  ################### SiteCheck Schedule #########################
   public class OASiteCheckSchedule : OperationAuthorizationBase
    {

        public OASiteCheckSchedule(int employeeID)
        {
            var role = new EmployeeRole(employeeID);
           if (role.IsInRoles(FsMembershipAuthorizationArray.PageWorkorderReadonly) || role.IsInRoles( FsMembershipAuthorizationArray.PageWorkorderModifyContentsEnabled ))
          {
                IsAuthorized = true;
             }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageSiteCheckScheduleAttachDocumentsEnabled))
            {
                IsAuthorized = true;
                
                IsAttachDocumentEnabled = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageSiteCheckScheduleModifyContentsEnabled))
            {
                IsAuthorized = true;
                IsAuthorizedEditEnabled = true;
                IsAuthorizedPageSpecial01 = true;
                IsTeamSelectEnabled = true;
                IsAttachDocumentEnabled = true;
            }
        }
    }

#endregion 



    
#region  ################### Production #########################

    public class OAProduction
    {
        public bool IsAuthorized { get; set; }

        public bool IsScheduleSubdepartmentEnabled { get; set; }
        public bool IsScheduleInstallationEnabled { get; set; }
        public bool IsAssignNewJobEnabled { get; set; }

        public bool IsChangeDeadlineProductionEnabled { get; set; }
        public bool IsChangeDeadlineSalesEnabled { get; set; }

        public bool IsTimecardOvertimeApplyEnabledInstallation { get; set; }
        public bool IsTimecardOvertimeApplyEnabledWorkshop { get; set; }
        public bool IsTimecardOvertimeApprovalEnabled { get; set; }


        public OAProduction(int employeeID)
        {
            //Initialization
            IsAuthorized = false;
               

            IsScheduleSubdepartmentEnabled = false;
            IsScheduleInstallationEnabled = false;

            IsAssignNewJobEnabled = false;
            IsChangeDeadlineProductionEnabled = false;
            IsChangeDeadlineSalesEnabled = false;

            IsTimecardOvertimeApplyEnabledInstallation=false ;
            IsTimecardOvertimeApplyEnabledWorkshop =false;
            IsTimecardOvertimeApprovalEnabled = false;
            

            var role = new EmployeeRole(employeeID);
            if (role.IsInRoles(FsMembershipAuthorizationArray.PageWorkorderReadonly))
            {
                IsAuthorized = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageWorkshopScheduleSubdepartmentEnabled))
            {
                IsAuthorized = true;
                IsScheduleSubdepartmentEnabled = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageWorkshopScheduleInstallationEnabled))
            {
                IsAuthorized = true;
                IsScheduleInstallationEnabled = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageWorkshopScheduleAssignNewJobEnabled))
            {

                IsAuthorized = true;

                IsScheduleSubdepartmentEnabled = true;
                IsScheduleInstallationEnabled = true;

                IsAssignNewJobEnabled = true;
            }


            if (role.IsInRoles(FsMembershipAuthorizationArray.PageWorkshopScheduleChangeDeadlineEnabled))
            {

                IsAuthorized = true;

               IsScheduleSubdepartmentEnabled = true;
                IsScheduleInstallationEnabled = true;

                IsAssignNewJobEnabled = true;
                IsChangeDeadlineProductionEnabled = true;
            }

            //sales change deadline
            
            if (role.IsInRoles(FsMembershipAuthorizationArray.PageWorkorderModifyContentsEnabled))
            {
                IsAuthorized = true;
                IsChangeDeadlineSalesEnabled = true;
            }

            //Timecard
            if (role.IsInRoles(FsMembershipAuthorizationArray.PageTimecardOvertimeApplyEnabledInstallation) )
            {
                IsAuthorized = true;
                IsTimecardOvertimeApplyEnabledInstallation = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageTimecardOvertimeApplyEnabledWorkshop ))
            {
                IsAuthorized = true;
                IsTimecardOvertimeApplyEnabledWorkshop = true;
            }


            if (role.IsInRoles(FsMembershipAuthorizationArray.PageTimecardOvertimeApprovalEnabled ))
            {
                IsAuthorized = true;
                IsTimecardOvertimeApprovalEnabled = true;
            }


        }

    }



    #endregion 


    #region  ################### Estimaition #########################
    public class OAMaterialMaintenance 
    {
        public bool IsAuthorized { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsInputTemporaryMaterialShopEnabled { get; set; }
        public bool IsInputTemporaryMaterialEstimationEnabled { get; set; }

        public OAMaterialMaintenance(int employeeID)
        {
            IsAuthorized = false ;
            IsAdministrator =false ;
            IsInputTemporaryMaterialShopEnabled =false ;
            IsInputTemporaryMaterialEstimationEnabled = false;


            var role = new EmployeeRole(employeeID);

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageMaterialMaintenaceInputShop))
            {
                IsAuthorized = true;
                IsInputTemporaryMaterialShopEnabled = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageMaterialMaintenaceInputEstimator))
            {
                IsAuthorized = true;
                IsInputTemporaryMaterialEstimationEnabled = true;
            }

            if (role.IsInRoles(FsMembershipAuthorizationArray.PageMaterialMaintenaceAdministrator))
            {
                IsAuthorized = true;
                IsAdministrator = true;
                IsInputTemporaryMaterialShopEnabled = true;
                IsInputTemporaryMaterialEstimationEnabled = true;
            }
        }
    }
    #endregion 


    #region  ################### Estimaition #########################
    public class OAReportInvoiceToBeIssued
    {
        public bool IsAuthorized { get; set; }
        public bool IsCommentsEnabled { get; set; }

        public OAReportInvoiceToBeIssued(int employeeID)
        {
            IsAuthorized = true;
            IsCommentsEnabled = false;
            
            var role = new EmployeeRole(employeeID);

            if (role.IsInRoles(FsMembershipAuthorizationArray.Accounting))
            {
                IsCommentsEnabled = true;
            }

            if (employeeID == 12 | employeeID == 28)
            {
                IsCommentsEnabled = true;
            }
           
        }
    }


    #endregion 

}