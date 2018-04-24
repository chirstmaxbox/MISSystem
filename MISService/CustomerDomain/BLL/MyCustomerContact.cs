using CustomerDomain.Model;

namespace CustomerDomain.BLL
{
	public class MyCustomerContact
	{
		public CUSTOMER_CONTACT Value { get; private set; }
	    private readonly CustomerDbEntities _db = new CustomerDbEntities();

		public MyCustomerContact(int contactID)
		{
			Value = _db.CUSTOMER_CONTACT.Find(contactID);
		}

		public void InsertNewContact(int customerID)
		{
			var cc = new CUSTOMER_CONTACT
			         	{
			         		CUSTOMER_ROWID = customerID,
			         		CONTACT_ACTIVE = true,
			         		isAccountPayable = false,
			         		isProjectManager = false,
			         		isSiteContact = false,

			         	};
			_db.CUSTOMER_CONTACT.Add(cc);
			_db.SaveChanges();

		}
	}
}