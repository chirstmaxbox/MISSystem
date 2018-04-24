using System;
using System.Data;
using System.Data.SqlClient;
using CustomerDomain.BO;

namespace CustomerDomain.BDL
{
    public class FsCustomerContact
    {
        //?
        //1. Property name, phone, position
        private int _rowID = 0;
        public DataTable contactTbl
        {
            get { return GetContactList(); }
        }


        public FsCustomerContact(int companyRowID)
        {
            _rowID = companyRowID;
        }



        #region "Insert"

        public void InsertContact()
        {
            SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);

            using (ConnectionSQL)
            {
                // As New System.Data.SqlClient.SqlConnection(Configuration.ConnectionString)
                try
                {
                    string InsertString = "INSERT INTO CUSTOMER_CONTACT(CONTACT_ACTIVE, isAccountPayable, isProjectManager, isSiteContact, CUSTOMER_ROWID) VALUES (@CONTACT_ACTIVE, @isAccountPayable, @isProjectManager, @isSiteContact, @CUSTOMER_ROWID)";
                    System.Data.SqlClient.SqlCommand InsertCommand = new System.Data.SqlClient.SqlCommand(InsertString, ConnectionSQL);


                    InsertCommand.Parameters.Add("@CUSTOMER_ROWID", SqlDbType.Int).Value = _rowID;

                    //                InsertCommand.Parameters.Add("@CONTACT_HONORIFIC", SqlDbType.Int).Value = 20           'blank
                    InsertCommand.Parameters.Add("@CONTACT_ACTIVE", SqlDbType.Bit).Value = true;
                    InsertCommand.Parameters.Add("@isAccountPayable", SqlDbType.Bit).Value = false;
                    InsertCommand.Parameters.Add("@isProjectManager", SqlDbType.Bit).Value = false;
                    InsertCommand.Parameters.Add("@isSiteContact", SqlDbType.Bit).Value = false;

                    ConnectionSQL.Open();
                    InsertCommand.ExecuteNonQuery();

                }
                catch (SqlException ex)
                {
                    string errorLog = ex.Message;
                }
                finally
                {
                    ConnectionSQL.Close();
                }

            }

        }



        public static int getAccountPayable(int CustomerID)
        {
            int id = 0;
            SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);
            //aLL contact of this customer
            string SqlSelectString1 = "SELECT CONTACT_ID,isAccountPayable from CUSTOMER_CONTACT WHERE (CUSTOMER_ROWID= @CUSTOMER_ROWID)";
            SqlCommand SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@CUSTOMER_ROWID", SqlDbType.Int).Value = CustomerID;
            DataSet ds1 = new DataSet();
            ds1.Tables.Clear();
            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                // is there contact?
                if (NumRowsAffected > 0)
                {
                    DataRow row = null;
                    // is there account payable
                    foreach (DataRow row_loopVariable in ds1.Tables["t1"].Rows)
                    {
                        row = row_loopVariable;
                        if (Convert.ToBoolean( row["isAccountPayable"] )== true)
                        {
                            id = Convert.ToInt32( row["CONTACT_ID"]);
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
                id = 0;
            }
            finally
            {
                ConnectionSQL.Close();
            }
            // IF THERE IS A PAYABLE, RETUREN THE ID ELSE RETURN 0
            return id;

        }


        #endregion


        public void UpdateContactEmail(int Contact_ID, string email)
        {
            using (System.Data.SqlClient.SqlConnection Connection = new System.Data.SqlClient.SqlConnection(CustomerDomainConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE [CUSTOMER_CONTACT] SET [CONTACT_EMAIL] = @CONTACT_EMAIL WHERE [CONTACT_ID] = @CONTACT_ID";
                System.Data.SqlClient.SqlCommand UPdateCommand = new System.Data.SqlClient.SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@CONTACT_ID", SqlDbType.Int).Value = Contact_ID;
                UPdateCommand.Parameters.Add("@CONTACT_EMAIL", SqlDbType.NVarChar, 100).Value = email;
                try
                {
                    Connection.Open();
                    UPdateCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        private DataTable GetContactList()
        {
            DataTable tbl = null;
            //    Dim StrConnectionSQL As String = GetConfiguration.ConnectionString
            SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT CONTACT_ID, CASE WHEN CONTACT_FIRST_NAME IS NULL THEN '  ' ELSE CONTACT_FIRST_NAME END + '   ' + CASE WHEN CONTACT_LAST_NAME IS NULL THEN ' ' ELSE CONTACT_LAST_NAME END AS quoteContactName FROM CUSTOMER_CONTACT WHERE (CUSTOMER_ROWID = @CUSTOMER_ROWID) OR (CONTACT_ID < 100)";
            SqlCommand SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@CUSTOMER_ROWID", SqlDbType.Int).Value = _rowID;
            DataSet ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected > 0)
                {
                    tbl = ds1.Tables["t1"];
                }

            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }

            return tbl;

        }


    }

    public class FsCustomerContactSelect
    {

        public DataRow Row = null;

        public FsCustomerContactSelect(int myID)
        {
            Row = GetContactInfo(myID);
        }


        private DataRow GetContactInfo(int contactID)
        {
            DataRow row = null;
            //    Dim StrConnectionSQL As String = GetConfiguration.ConnectionString
            SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT CONTACT_POSITION,CONTACT_PHONE,CONTACT_MOBILE, dbo.GetFullName(CONTACT_First_name, Contact_last_Name) as CONTACT_NAME from CUSTOMER_CONTACT WHERE (CONTACT_ID = @CONTACT_ID)";
            SqlCommand SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@CONTACT_ID", SqlDbType.Int).Value = contactID;
            DataSet ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected > 0)
                {
                    row = ds1.Tables["t1"].Rows[0];
                }

            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }
            return row;

        }

    }

    public class CustomerContactDelete
    {
        private int _rowID = 0;
        public CustomerContactDelete(int companyID)
        {
            _rowID = companyID;
        }

        //delete

        public void DeleteContacts()
		{
			string errorLog = "Nothing";
			int NumRowsAffected = 0;

			//2. Define the destination: InvoiceItem
			SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);
			string SqlSelectString1 = "SELECT * FROM [CUSTOMER_CONTACT] WHERE [CUSTOMER_ROWID] = @CUSTOMER_ROWID";
			SqlCommand SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
			SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand1);
			adapter1.SelectCommand.Parameters.Add("@CUSTOMER_ROWID", SqlDbType.Int).Value = _rowID;
			DataSet ds1 = new DataSet();
			ds1.Tables.Clear();

			try {
				ConnectionSQL.Open();
				NumRowsAffected = adapter1.Fill(ds1, "t1");

                if (NumRowsAffected == 0) return;

                        foreach (DataRow row in ds1.Tables["t1"].Rows)
                        {
                            var contactID = Convert.ToInt32(row["CONTACT_ID"]);

                            row.Delete();
                        }
			    //4. Write ds2,  back to DB

					SqlCommandBuilder cb = new SqlCommandBuilder(adapter1);
					adapter1 = cb.DataAdapter;
					NumRowsAffected = adapter1.Update(ds1, "t1");

				
			}catch (SqlException ex) {
				errorLog = ex.Message;
			} finally {
				ConnectionSQL.Close();
			}
			
		}


    }

}