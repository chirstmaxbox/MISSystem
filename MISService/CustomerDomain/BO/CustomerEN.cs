namespace CustomerDomain.BO
{
    public class CustomerEN
    {
        #region NCountryCode enum

        public enum NCountryCode : byte
        {
            Canada = 0,
            USA = 1,
            Mexico = 2
        }

        #endregion

        #region NCustomerDefaultID enum

        public enum NCustomerDefaultID
        {
            Choose = 0,
            AddNew = 1
        }

        #endregion

        #region NCustomerSelectType enum

        public enum NCustomerSelectType
        {
            //
            allCompany = 0,
            //Catagory  Chain Store, typeValue="100" Title="Direct customer" />
            CategoryDirectCustomer = 100,
            //Franchisee
            c1Franchisee = 11,
            //c1Branch = 12
            //Name="Franchisee/Branch/Franchiser (e.g.  K.F.C, Golf Town )">  </cType>
            c1Headoffice = 13,

            //Name="Individual Store / Retail (e.g. Starwalk Buffet, Pacific Mall )">   </cType>
            c1IndividualStore = 31,

            //  <Catagory Value="150" typeValue="150" Title="Company that give us businesses" >
            CategoryGiveUsBusiness = 200,
            c2Architecture = 201,
            c2Designer = 202,
            c2GeneralContract = 203,
            c2InsuranceCompany = 204,
            c2SignCompany = 205,
            c2LandlordAndManagement = 206,
            c2Other = 207,

            //Obselete
            //'<Catagory Value="300" typeValue="300" Title="Lead" >
            //CategoryLead = 300
            //c3Contacting = 302

            //  <Catagory Value="400" typeValue="400" Title="Job related company" >
            categoryJobRelatedCompany = 400,
            c4Forwarder = 401,
            c4LandlordAndManagement = 402,
            c4Mall = 403,
            c4SubContractInstaller = 404,
            c4Other = 405
        }

        #endregion

        #region NCustomerTerritory enum

        public enum NCustomerTerritory
        {
            GTA = 1,
            BC = 2,
            //	British Columbia
            AB = 3,
            //Alberta
            SK = 4,
            //Saskatchewan
            US = 5,
            //	U.S. 
            Other = 6,
            //NULL
            MB = 7,
            //Manitoba
            QC = 8,
            //Quebc
            NL = 9,
            //Newfoundland and Labrador
            NB = 10,
            //	New Brunswick
            PE = 11,
            //Prince Edward Island
            NS = 12,
            //Nova Scotia
            NT = 13,
            //Northern Territories
            ONT = 14
            //ON--Ontario
        }

        #endregion

    }
}