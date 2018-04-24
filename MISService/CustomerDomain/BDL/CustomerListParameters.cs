
namespace CustomerDomain.BDL
{
    public class CustomerListParameters
    {

        public bool CbxHeadoffice { get; set; }
        public bool CbxFranchisee { get; set; }
        public bool CbxIndividualStore { get; set; }
        public bool CbxPartner { get; set; }

        public int TeamID { get; set; }
        public int SalesID { get; set; }

        public string FirstCharacterOfName { get; set; }
        public int IndustryID { get; set; }
        public int CustomerrID { get; set; }

        //   public int SelectType { get; set; }     //NCustomerSelectType

        public int ContactOption { get; set; }

        public CustomerListParameters()
        {
            ContactOption = 0;
            
            CbxHeadoffice = false;
            CbxFranchisee = false;
            CbxIndividualStore = false;
            CbxPartner = false;

            SalesID = 0;
            TeamID = 0;

            FirstCharacterOfName = "All";
            IndustryID = 0;

            CustomerrID = 0;
        }
    }
}