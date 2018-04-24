namespace EmployeeDomain.BLL
{
    // Windows Authentication
    // Custom user Role Provider
    // To do:  
    //       tblAuthorizationArray
    //       PageJobMaster()=getPageJobMaster(), which will get the configured data from db

    public class FsMembershipAuthorizationArray
    {
        public static readonly string[] PageEstimationReadonly = {
                                                                     "ProductionManager",
                                                                     "SalesManager",
                                                                     "SalesViewOnly",
                                                                     "SalesMarketing",
                                                                     "Accounting"
                                                                 };

        public static readonly string[] PageEstimationEstimatorOnlyControlsEnabled = {
                                                                                         "Estimator"
                                                                                     };

        public static readonly string[] PageEstimationAttachDocumentsEnabled = {
                                                                                   "ArtDesigner",
                                                                                   "StructuralDesigner",
                                                                                   "Estimator",
                                                                               };

        public static readonly string[] PageEstimationModifyContentsEnabled = {
                                                                                  "SalesPowerUser",
                                                                                  "Admin"
                                                                              };



        public static readonly string[] PageWorkorderReadonly = {
                                                                    "SalesManager",
                                                                    "SalesViewOnly",
                                                                    "SalesMarketing",
                                                                    "ArtDesigner",
                                                                    "StructuralDesigner",
                                                                    "Estimator",
                                                                    "WorkshopSupervisor",
                                                                    "InputWorkOrder",
                                                                    "SchedulerArtRoom",
                                                                    "SchedulerSiteCheck",
                                                                    "SchedulerInstallation",
                                                                    "SchedulerWorkshop",
                                                                    "SchedulerShopDepartment",
                                                                    "Accounting",
                                                                    "HrClerk"
                                                                };


        public static readonly string[] PageWorkorderModifyContentsEnabled = {
                                                                                 "SalesPowerUser",
                                                                                 "Admin"
                                                                             };

        public static readonly string[] PageWorkorderAttachDoucmentsEnabled = {
                                                                                  "ArtDesigner",
                                                                                  "StructuralDesigner",
                                                                              };


        //Schedule
        public static readonly string[] PageWorkshopScheduleSubdepartmentEnabled = {
                                                                                       "SchedulerWorkshop",
                                                                                       "SchedulerShopDepartment",

                                                                                   };

        public static readonly string[] PageWorkshopScheduleInstallationEnabled = {
                                                                                      "SchedulerInstallation",

                                                                                  };

        public static readonly string[] PageWorkshopScheduleAssignNewJobEnabled = {
                                                                                      "WorkshopSupervisor"
                                                                                  };


        public static readonly string[] PageWorkshopScheduleChangeDeadlineEnabled = {
                                                                                        "ProductionManager",
                                                                                        "Admin"
                                                                                    };


        public static readonly string[] PageHelp = {
                                                       "Admin",
                                                       "DocumentsWriter"
                                                   };

        public static readonly string[] PageEmployeeManagement = {
                                                                     "HrClerk",
                                                                     "HrManager",
                                                                     "Admin"
                                                                 };

        public static readonly string[] PageSalesCommissionReadonly = {
                                                                          "SalesPowerUser",
                                                                      };

        public static readonly string[] PageSalesCommissionViewAll = {

                                                                            "ViewCommissionOfAllSales"
                                                                     };


        public static readonly string[] PageSalesCommissionManupulate = {
                                                                            "HrManager",
                                                                            "Admin"
                                                                        };




        public static readonly string[] PageClientList = {
                                                             "SalesManager",
                                                             "Admin",
                                                             "SalesViewOnly",
                                                             "SalesMarketing",
                                                             "Accounting",
                                                             "SalesPowerUser"
                                                         };

        public static readonly string[] PageClientListSelf = {"SalesPowerUser"};




        public static readonly string[] PageInvoiceReadOnly = {
                                                                  "SalesManager"
                                                              };

        public static readonly string[] PageInvoiceEdit = {

                                                              "SalesPowerUser"
                                                          };

        //Sub Array of PageInvoice
        public static readonly string[] Accounting = {
                                                         "Admin",
                                                         "Accounting"
                                                     };




        public static readonly string[] PageProjectCompany = {
                                                                 "SalesManager",
                                                                 "SalesPowerUser",
                                                                 "Admin",
                                                                 "SalesViewOnly",
                                                                 "SalesMarketing",
                                                                 "Accounting"
                                                             };

        public static readonly string[] PageProjectCompanyEditEnabled = {
                                                                            "SalesPowerUser",
                                                                            "Admin"
                                                                        };

        public static readonly string[] PageProjectReadonly = {
                                                                  "ProductionManager",
                                                                  "SalesManager",
                                                                  "SalesViewOnly",
                                                                  "SalesMarketing",
                                                              };

        public static readonly string[] PageProjectTeamSelectEnabled = {
                                                                           "SalesManager",
                                                                           "SalesViewOnly",
                                                                           "SalesMarketing",
                                                                           "Admin",
                                                                           "Accounting",
                                                                           "ArtDesigner",
                                                                           "StructuralDesigner",
                                                                           "Estimator",
                                                                       };

        public static readonly string[] PageProjectEditEnabled = {
                                                                     "SalesPowerUser",
                                                                     "Admin",
                                                                     "Accounting"
                                                                 };


        public static readonly string[] MasterPageDebug = {"Admin"};

        //Page 180, Response List
        public static readonly string[] PageResponse = {
                                                           "ProductionManager",
                                                           "SalesManager",
                                                           "SalesPowerUser",

                                                       };

        public static readonly string[] PageResponseStatusEnabled = {
                                                                        "ArtDesigner",
                                                                        "StructuralDesigner",
                                                                        "Estimator",
                                                                        "Admin"
                                                                    };

        public static readonly string[] PageApprovalWorkorderEnabled = {
                                                                           "WorkorderApprovalFinal",
                                                                           "Admin"
                                                                       };

        //Page 200
        public static readonly string[] PageReport = {
                                                         "SalesManager",
                                                         "SalesPowerUser",
                                                         "SalesViewOnly",
                                                         "SalesMarketing",
                                                         "ArtDesigner",
                                                         "StructuralDesigner",
                                                         "Estimator",
                                                         "Accounting",
                                                         "HrClerk",
                                                         "WorkshopSupervisor",
                                                         "ReporterPrinter",
                                                         "Admin"
                                                     };


        public static readonly string[] PageSiteCheckScheduleAttachDocumentsEnabled = {

                                                                                          "SchedulerSiteCheck"

                                                                                      };

        public static readonly string[] PageSiteCheckScheduleModifyContentsEnabled = {

                                                                                         "SchedulerSiteCheck",
                                                                                         "SchedulerWorkshop",
                                                                                         "SchedulerInstallation",
                                                                                         "Admin"
                                                                                     };



        public static readonly string[] PageSubContractRequestEnabled = {
                                                                            "SalesPowerUser",


                                                                        };

        public static readonly string[] PageSubContractResponseEnabled = {
                                                                             "SchedulerInstallation",

                                                                             "ProductionManager",
                                                                             "Admin"
                                                                         };


        public static readonly string[] PageTimecardOvertimeApplyEnabledInstallation = {
                                                                                           "SchedulerInstallation",
                                                                                           "TimecardOvertimeApprove",
                                                                                           "HrClerk",
                                                                                           "Admin"
                                                                                       };

        public static readonly string[] PageTimecardOvertimeApplyEnabledWorkshop = {
                                                                                       "SchedulerShopDepartment",
                                                                                       "SchedulerWorkshop",
                                                                                       "TimecardOvertimeApprove",
                                                                                       "HrClerk",
                                                                                       "Admin"
                                                                                   };

        public static readonly string[] PageTimecardOvertimeApprovalEnabled = {
                                                                                  "TimecardOvertimeApprove",
                                                                                  "HrClerk",
                                                                                  "Admin"
                                                                              };

        public static readonly string[] PageMaterialMaintenaceAdministrator = {
                                                                                  "MaterialMaintenaceAdministrator",
                                                                                  "Admin"
                                                                              };

        public static readonly string[] PageMaterialMaintenaceInputShop = {
                                                                              "SchedulerShopDepartment",
                                                                              "SchedulerWorkshop",
                                                                          };

        public static readonly string[] PageMaterialMaintenaceInputEstimator = {
                                                                                   "Estimator"
                                                                               };

    }

}
