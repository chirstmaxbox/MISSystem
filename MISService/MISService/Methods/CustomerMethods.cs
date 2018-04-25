using CustomerDomain.BLL;
using CustomerDomain.Model;
using MISService.Method;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* that is company information */
namespace MISService.Methods
{
    public class CustomerMethods
    {

        private readonly CustomerDbEntities _db = new CustomerDbEntities();

        /// <summary>
        /// Create a new Customer which is company information
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="jcID">it is jcID in [Sales_JobMasterList_Customer]</param>
        public void CreateCustomer(CUSTOMER customer, int jcID)
        {
            try
            {
                var mc = new MyCustomer();
                mc.Insert(customer);
                if (jcID > 0) {
                    mc.UpdateSalesJobMasterListCustomerID(jcID);
                }
                LogMethods.Log.Debug("CreateCustomer:Debug:" + "DONE");
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("CreateCustomer:Crash:" + e.Message);
            }
        }

        /// <summary>
        /// Edit customer
        /// </summary>
        /// <param name="customer"></param>
        public void EditCustomer(CUSTOMER customer)
        {
            try
            {
                _db.Entry(customer).State = EntityState.Modified;
                _db.SaveChanges();
                LogMethods.Log.Debug("EditCustomer:Debug:" + "DONE");
            }
            catch (Exception e)
            {
                LogMethods.Log.Error("EditCustomer:Crash:" + e.Message);
            }
        }

    }
}
