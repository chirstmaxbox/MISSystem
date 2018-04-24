using System.Linq;
using CustomerDomain.Model;

namespace CustomerDomain.BLL
{
    public class CustomerMall
    {
        public int MallID { get; set; }

        public CustomerMall (int customerID)
        {
            var mc = new MyCustomer(customerID);
            MallID = GetMallID(mc.Value);
        }

        private int GetMallID(CUSTOMER customer)
        {
            var dc = new MallDataContext();
            var mall = dc.VenderMalls.FirstOrDefault(x => x.Address.Contains(customer.ADDR_1));
            var mallID = 0;
            if (mall!=null )
            {
                mallID = mall.MallID;
            }
            return mallID;
        }

    }
}