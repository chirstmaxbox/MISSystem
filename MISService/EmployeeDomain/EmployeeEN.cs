namespace EmployeeDomain
{
    public class EmployeeEN
    {
        #region NDepartmentID enum

        public enum NDepartmentID
        {
            ArtRoom = 1,
            Metal = 2,
            Painting = 3,
            PlasticA = 4,
            //Obselete
            PlasticB = 5,
            RA3 = 6,
            Installation = 7,
            SiteCheck = 8,
            InstallationB = 9,
            Workshop = 21,
            QD = 22,
            SalesMarketing = 63,
            Purchasing = 54,
            AdminHr = 42,
            Account = 41,
            All = 60,
            //should be 0 or 9999
            NULL = 99,
            VirtualWorkshop = 121,
            VirtualProduction = 122
        }

        #endregion

        #region NEmployeeIDDefault enum

        public enum NEmployeeIDDefault
        {
            //It
            sales = 28,

            //blank
            NullSales110 = 110,
            
            NullSales108 = 108, //NullValueName
            ChooseProduction=109, 
            estimator = 8,
            Artist = 7,
            StructuralEngineering = 3,
            receiptionist = 23,
            //Pamean
            shopsupervisor = 98,
            //Wei
            
            installationsupervisor = 78,
            //Scott
            SiteCheckScheduler = 21,
            //Lam
            allSales = 5000,
            allOperation = 5010,
            UnAssigned = 5050,
            //Marketing 
            All = 5020,
            AllTeam = 5015,
            Other=8886,
            President = 88,
            VP = 68,
            Null = 0,

            //Freda Smarter Center
            SmarterCenter=30,
            SmarterCenterAE = 123,
        }

        #endregion



        #region NRoleList enum

        public enum NRoleGroup
        {

            ReadWrite = 0,
            Designer = 1,
            //Can edit Drawing in Sales Center and Workorder
            ViewOnly = 2,
            Estimator = 3,
            SalesManager = 4,
            Accounting = 5
        }

        public enum NEmployeeRole
        {
            //Table Employee.Role
            General = 0,
            AE = 1,
            OP = 2,
         //   Choose = 3,
         //   SelectAll = 4,
            Marketing = 3,
            Artist = 7,
            Estiator = 8,
            Engineer = 9,
            Management = 10,
            AeLeft=11,
            OpLeft=12,

            InstallationA=20,
            InstallationB = 21,

        }

        public enum NRoleList
        {
            Admin = 1,
            SalesPowerUser = 100,
            SalesManager = 101,
            SalesMarketing = 102,
            SchedulerInstallation = 103,
            SchedulerWorkshop = 104,
            SchedulerSiteCheck = 105,
            SchedulerArtRoom = 106,
            InputerHours = 107,
            InputerMaterial = 108,
            InputerWorkOrder = 109,
            ReporterPrinter = 110,
            Estimator = 111,
            ArtDesigner = 112,
            StructuralDesigner = 113,
            HrClerk = 114,
            WorkshopSupervisor = 115,
            Accounting = 116,
            SalesViewOnly = 117,
            SchedulerShopDepartment = 118
        }

        #endregion
    }
}