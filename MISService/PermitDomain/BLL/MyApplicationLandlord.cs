using System;
using System.Collections.Generic;
using System.Linq;
using MyCommon;
using PermitDomain.Model;

namespace PermitDomain.BLL
{
    public class MyApplicationLandlord
    {
     //   public int NewlyInsertedID { get; private set; }

		public int NewlyInsertedContactID { get; set; }

		public List<MyKeyValuePair> Contacts {
			get { return GetContacts(); } 
		}

        private readonly PermitDbEntities _db = new PermitDbEntities();
    	private readonly int _landlordID;
    	
		public MyApplicationLandlord(int landlordID)
		{
			_landlordID = landlordID;
		}

        public void CreateContact()
        {
			var contact = new PermitLandlordContact()
        	              	{
        	              		ROWID = _landlordID,
        	              		CONTACT_ACTIVE = true,
        	              	};

			_db.PermitLandlordContacts.Add(contact);
			_db.SaveChanges();
        	NewlyInsertedContactID = contact.CONTACT_ID;

        }

		private List<MyKeyValuePair> GetContacts()
		{
			var myContacts = _db.PermitLandlordContacts.Where(x => x.ROWID == _landlordID | x.CONTACT_ID == 0).ToList();
			return myContacts.Select(contact => new MyKeyValuePair {Key = contact.CONTACT_ID, Value = GetFullName(contact)}).ToList();
		}

		private string GetFullName(PermitLandlordContact contact)
		{
			string s1 ="";
			string s2 = "";
			string s3 = "";
			if( MyString.IsStringLengthLongerThan (1,contact.CONTACT_HONORIFIC ))
			{
				s1 =contact.CONTACT_HONORIFIC+ " ";
			}

			if( MyString.IsStringLengthLongerThan (1,contact.CONTACT_FIRST_NAME))
			{
				s2= contact.CONTACT_FIRST_NAME+" ";
			}

			if (MyString.IsStringLengthLongerThan(1, contact.CONTACT_LAST_NAME))
			{
				s3 = contact.CONTACT_LAST_NAME;
			}

			return s1 + s2 + s3;
		}


    }
}

