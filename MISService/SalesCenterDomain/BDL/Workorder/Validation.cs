using System;
using System.Data;
using System.Data.SqlClient;
using CustomerDomain.BDL;
using MyCommon;
using MyCommon.MyEnum;
using SalesCenterDomain.BDL.Project;
using SalesCenterDomain.BLL;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Workorder
{
    public class WorkorderValidation
    {
        private readonly int _woID;

        private int[] _validateResult;

        public WorkorderValidation(int woID)
        {
            _woID = woID;
            _validateResult = ValidateItems();
        }

        public int[] ValidateResult
        {
            get
            {
                _validateResult = ValidateItems();
                return _validateResult;
            }
        }

        public bool IsValidated
        {
            get { return GetIsValidate(); }
        }

        private bool GetIsValidate()
        {
            int count = _validateResult.Length;
            bool b = true;
            for (int i = 0; i <= count - 1; i++)
            {
                if (_validateResult[i] != (int) NWorkorderValidationError.Validated)
                {
                    b = false;
                }
            }
            return b;
        }


        private int[] ValidateItems()
        {
            var vWo = new WorkorderValidationItem[SalesCenterConstants.WORKORDER_VALIDATION_ITEM_COUNT_MAX + 1];

            int i = 0;
            vWo[i] = new WorkorderValidationItemDeadline(_woID);
            i += 1;
            vWo[i] = new WorkorderValidationItemIssuedDate(_woID);
            i += 1;
            vWo[i] = new WorkorderValidationItemNoCharge(_woID);
            i += 1;
            vWo[i] = new WorkorderValidationItemRedo(_woID);
            i += 1;
            vWo[i] = new WorkorderValidationItemRush(_woID);
            i += 1;
            vWo[i] = new WorkorderValidationItemRevise(_woID);
            i += 1;
            vWo[i] = new WorkorderValidationItemContract(_woID);
            i += 1;
            vWo[i] = new WorkorderValidationItemSiteContact(_woID);
            i += 1;
            vWo[i] = new WorkorderValidationItemSitePostCode(_woID);
            i += 1;
            vWo[i] = new WorkorderValidationItemInstalltoAddress(_woID);
            i += 1;
            vWo[i] = new WorkorderValidationItemSiteCheckPurpose(_woID);
            i += 1;
            vWo[i] = new WorkorderValidationItemDocuments(_woID);
            i += 1;

            vWo[i] = new WorkorderValidationItemItemName(_woID);
            i += 1;
            vWo[i] = new WorkorderValidationItemItemLeadtime(_woID);
            i += 1;
            vWo[i] = new WorkorderValidationItemSpecialProcedure(_woID);
            i += 1;

            vWo[i] = new WorkorderValidationItemInstructionForRequirement(_woID,
                                                                          (int)
                                                                          NWorkorderValidationError.
                                                                              InstructionRelocation);
            i += 1;
            vWo[i] = new WorkorderValidationItemInstructionForRequirement(_woID,
                                                                          (int)
                                                                          NWorkorderValidationError.
                                                                              InstructionTakedownAndBringbackStock);
            i += 1;
            vWo[i] = new WorkorderValidationItemInstructionForRequirement(_woID,
                                                                          (int)
                                                                          NWorkorderValidationError.
                                                                              InstructionTakedownAndDelivery);
            i += 1;
            vWo[i] = new WorkorderValidationItemInstructionForRequirement(_woID,
                                                                          (int)
                                                                          NWorkorderValidationError.
                                                                              InstructionTakedownAndDispose);

            //i += 1;
            //vWo[i] = new WorkorderValidationContractAmount(_woID);

            //i=actual validated count

            var rv = new int[i + 1];
            for (int j = 0; j <= i; j++)
            {
                rv[j] = vWo[j].Validate();
            }

            return rv;
        }
    }


    public abstract class WorkorderValidationItem
    {
        //'?? Conventation
        public int _itemValidationNo;
        public DataRow _row;
        public int _woID;

        public WorkorderValidationItem(int woID)
        {
            _row = WorkorderShared.GetWorkorderInfo(woID);
            _woID = woID;
        }

        public WorkorderValidationItem(int woID, int itemValidationNo)
        {
            _row = WorkorderShared.GetWorkorderInfo(woID);
            _woID = woID;
            _itemValidationNo = itemValidationNo;
        }

        public virtual int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;
            return v;
        }
    }

    #region "Date"

    public class WorkorderValidationItemDeadline : WorkorderValidationItem
    {
        public WorkorderValidationItemDeadline(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            //deadline
            var v = (int) NWorkorderValidationError.Validated;
            if (Convert.IsDBNull(_row["Deadline"]))
            {
                v = (int) NWorkorderValidationError.DeadlineRequired;
            }
            else if (!MyConvert.IsDate(_row["DeadLine"]))
            {
                v = (int) NWorkorderValidationError.DeadlineRequired;
            }
            else
            {
                DateTime dtDeadline = Convert.ToDateTime(_row["DeadLine"]);
                if (dtDeadline < DateTime.Today)
                {
                    v = (int) NWorkorderValidationError.DelalineIsLatterThanToday;
                }
            }
            return v;
        }
    }


    public class WorkorderValidationItemIssuedDate : WorkorderValidationItem
    {
        public WorkorderValidationItemIssuedDate(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            //issue date
            var v = (int) NWorkorderValidationError.Validated;

            if (Convert.IsDBNull(_row["issuedDate"]))
            {
                v = (int) NWorkorderValidationError.IssuedateisInvalid;
            }
            else if (!MyConvert.IsDate(_row["issuedDate"]))
            {
                v = (int) NWorkorderValidationError.IssuedateisInvalid;
            }
            else if (Convert.ToDateTime(_row["issuedDate"]) < DateTime.Today)
            {
                v = (int) NWorkorderValidationError.IssuedateisInvalid;
            }

            return v;
        }
    }

    #endregion

    #region "Reason"

    public class WorkorderValidationItemNoCharge : WorkorderValidationItem
    {
        public WorkorderValidationItemNoCharge(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;
            //No charge: service/product job order
            if (Convert.ToInt32(_row["woType"]) == (int) NWorkorderType.Production ||
                Convert.ToInt32(_row["woType"]) == (int) NWorkorderType.Service ||
                Convert.ToInt32(_row["woType"]) == (int) NWorkorderType.Takedown)
            {
                if (Convert.ToInt32(_row["PayMethods"]) >= 40)
                {
                    if (_row["redoReason"].ToString().Length <= SalesCenterConstants.StringLengthOfReason)
                    {
                        v = (int) NWorkorderValidationError.ReasonofRedoRequired;
                    }
                }
            }

            return v;
        }
    }


    public class WorkorderValidationItemRedo : WorkorderValidationItem
    {
        public WorkorderValidationItemRedo(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;

            //redo
            if (Convert.ToBoolean(_row["reDo"]))
            {
                if (_row["redoReason"].ToString().Length < SalesCenterConstants.StringLengthOfReason)
                {
                    v = (int) NWorkorderValidationError.ReasonofRedoRequired;
                }
            }

            return v;
        }
    }

    public class WorkorderValidationItemRush : WorkorderValidationItem
    {
        public WorkorderValidationItemRush(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;
            //rush
            if (Convert.ToBoolean(_row["rush"]))
            {
                if (_row["rushReason"].ToString().Length < SalesCenterConstants.StringLengthOfReason)
                {
                    v = (int) NWorkorderValidationError.ReasonofRushRequired;
                }
            }
            return v;
        }
    }


    public class WorkorderValidationItemRevise : WorkorderValidationItem
    {
        public WorkorderValidationItemRevise(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;
            //revise
            if (Convert.ToBoolean(_row["revise"]))
            {
                if (_row["revisedReason"].ToString().Length < SalesCenterConstants.StringLengthOfReason)
                {
                    v = (int) NWorkorderValidationError.ReasonofReviseRequired;
                }
            }

            return v;
        }
    }

    #endregion

    #region "Contract"

    public class WorkorderValidationItemContract : WorkorderValidationItem
    {
        public WorkorderValidationItemContract(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;

            //No charge: service/product job order
            int jobID = Convert.ToInt32(_row["jobID"]);
            if (Convert.ToInt32(_row["woType"]) == (int) NWorkorderType.Production)
            {
                if (!IsThisProjectContracted(jobID))
                {
                    var p = new ProjectDetails(jobID);
                    string s = p.ReasonOfNoContract;
                    if (s.Length < SalesCenterConstants.StringLengthOfReason)
                    {
                        v = (int) NWorkorderValidationError.ReasonofNoContract;
                    }
                }
            }
            return v;
        }


        private bool IsThisProjectContracted(int jobID)
        {
            bool b = false;
            var pc = new ProjectChildren(jobID);
            DataTable tbl = pc.Quote;
            if (tbl != null)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    if (Convert.ToInt32(row["QuoteStatus"]) == (int) NJobStatus.win)
                    {
                        b = true;
                    }
                }
            }
            return b;
        }
    }

    #endregion

    #region "Contact and Address"

    public class WorkorderValidationItemSiteContact : WorkorderValidationItem
    {
        public WorkorderValidationItemSiteContact(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;

            if (Convert.ToInt32(_row["woType"]) == (int) NWorkorderType.Production)
            {
                int contact = Convert.ToInt32(_row["Contact1"]);
                //ContactID less than begin id

                if (contact >= SalesCenterConstants.BEGIN_CUSTOMER_CONTACT_ID)
                {
                    var cc = new FsCustomerContactSelect(contact);
                    DataRow rowContact = cc.Row;
                    //Contact Was deleted
                    if (rowContact != null)
                    {
                        //Name
                        if (MyString.IsStringLengthLongerThan(2, rowContact["CONTACT_NAME"]))
                        {
                            //Phone
                            bool b1 = IsPhoneNumberValidated(rowContact["CONTACT_PHONE"]);
                            bool b2 = IsPhoneNumberValidated(rowContact["CONTACT_MOBILE"]);
                            if (b1 == false & b2 == false)
                            {
                                v = (int) NWorkorderValidationError.SiteContactnameandPhoneOrCellisRequired;
                            }
                        }
                        else
                        {
                            v = (int) NWorkorderValidationError.SiteContactnameandPhoneOrCellisRequired;
                        }
                    }
                    else
                    {
                        v = (int) NWorkorderValidationError.SiteContactnameandPhoneOrCellisRequired;
                    }
                }
                else
                {
                    v = (int) NWorkorderValidationError.SiteContactnameandPhoneOrCellisRequired;
                }
            }

            return v;
        }

        private bool IsPhoneNumberValidated(object phoneNumber)
        {
            bool b = true;
            if (MyConvert.IsNullString(phoneNumber))
            {
                b = false;
            }
            else
            {
                string pn = Convert.ToString(phoneNumber);
                if (pn.Length < 10)
                {
                    b = false;
                }
            }
            return b;
        }
    }


    public class WorkorderValidationItemSitePostCode : WorkorderValidationItem
    {
        public WorkorderValidationItemSitePostCode(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;


            if (IsThisOrderRequireAddressAccordingToItem())
            {
                int company = Convert.ToInt32(_row["Company1"]);
                //ContactID less than begin id

                if (company >= SalesCenterConstants.BEGIN_CUSTOMER_ID)
                {
                    var cc = new CustomerDetails(company);
                    string s1 = cc.ZipCode;
                    string s2 = cc.Intersection;

                    bool b1 = MyString.IsStringLengthLongerThan(4, s1);
                    bool b2 = MyString.IsStringLengthLongerThan(7, s2);
                    if (!(b1 | b2))
                    {
                        v = (int) NWorkorderValidationError.MainIntersectionOrConnerorPostCode;
                    }
                }
                else
                {
                    v = (int) NWorkorderValidationError.MainIntersectionOrConnerorPostCode;
                }
            }

            return v;
        }

        private bool IsThisOrderRequireAddressAccordingToItem()
        {
            //55 Fabrication	
            //80 Shipping	
            //90 Pickup	
            int[] NotRequiredItems = {
                                         55,
                                         80,
                                         90
                                     };

            bool b = true;
            DataTable tbl = WorkorderShared.getExistingWorkorderItems(_woID);

            if (tbl != null)
            {
                bool b1 = false;

                foreach (DataRow Row in tbl.Rows)
                {
                    int requirement = Convert.ToInt32(Row["requirement"]);
                    bool b2Contains = false;
                    for (int i = 0; i <= NotRequiredItems.Length - 1; i++)
                    {
                        if (NotRequiredItems[i] == requirement)
                        {
                            b2Contains = true;
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }

                    if (!b2Contains)
                    {
                        b1 = true;
                        //Exit For
                    }
                }

                b = b1;
            }

            return b;
        }
    }

    public class WorkorderValidationItemInstalltoAddress : WorkorderValidationItem
    {
        public WorkorderValidationItemInstalltoAddress(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;

            int isInstalltoID = GetIsInstallto();
            var ci = new CustomerDetails(isInstalltoID);


            string address = ci.Address;

            string city = ci.City;


            if (!(MyString.IsStringLengthLongerThan(3, address) & MyString.IsStringLengthLongerThan(3, city)))
            {
                v = (int) NWorkorderValidationError.InstalltoAddressOrCityRequired;
            }

            return v;
        }


        private int GetIsInstallto()
        {
            int installToID = 0;
            DataRow row = WorkorderShared.GetWorkorderInfo(_woID);
            installToID = Convert.ToInt32(row["Company1"]);
            return installToID;
        }
    }

    #endregion

    #region "Site Check Purpose"

    public class WorkorderValidationItemSiteCheckPurpose : WorkorderValidationItem
    {
        public WorkorderValidationItemSiteCheckPurpose(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;

            //No charge: service/product job order
            if (Convert.ToInt32(_row["woType"]) == (int) NWorkorderType.Sitecheck)
            {
                DataRow row = GetWorkorderSitecheckPurpose(_woID);
                //No record
                if (row == null)
                {
                    v = (int) NWorkorderValidationError.SiteCheckPurposeRequired;
                }
                else
                {
                    //No purpose checked
                    bool b1 = Convert.ToBoolean(row["scPurpose1"]);
                    bool b2 = Convert.ToBoolean(row["scPurpose2"]);
                    bool b3 = Convert.ToBoolean(row["scPurpose3"]);
                    bool b4 = Convert.ToBoolean(row["scPurpose4"]);

                    bool b5 = false;
                    if (!MyConvert.IsNullString(row["scPurposeOther"]))
                    {
                        string s = Convert.ToString(row["scPurposeOther"]);
                        if (s.Length >= 10)
                        {
                            b1 = true;
                        }
                    }

                    if ((b1 || b2 || b3 || b4 || b5) == false)
                    {
                        v = (int) NWorkorderValidationError.SiteCheckPurposeRequired;
                    }
                }
            }
            return v;
        }


        private DataRow GetWorkorderSitecheckPurpose(int woID)
        {
            int NumRowsAffected = 0;
            DataRow titleInfoRow = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [WO_Sitecheck_Purpose] WHERE ([woID] = @woID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woID", SqlDbType.Int).Value = woID;
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");

                if (NumRowsAffected != 0)
                {
                    titleInfoRow = ds1.Tables["t1"].Rows[0];
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

            return titleInfoRow;
        }
    }

    #endregion

    #region "Documents"

    public class WorkorderValidationItemDocuments : WorkorderValidationItem
    {
        public WorkorderValidationItemDocuments(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;

            var fWoType = new WorkorderFieldWoType(_woID);
            int woType = Convert.ToInt32(fWoType.FieldValue);

            if (woType == (int) NWorkorderType.Production | woType == (int) NWorkorderType.Service)
            {
                var wpc = new WorkorderPropertyChecklist(0, _woID);
                bool applicable = wpc.ChecklistApplicable;
                if (applicable)
                {
                    if (!IsThereChecklist())
                    {
                        v = (int) NWorkorderValidationError.ChecklistDocumentsAttachedRequired;
                    }
                }
            }

            return v;
        }


        private bool IsThereChecklist()
        {
            bool b = false;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [WO_WORKORDER_CHECKLIST_DATATABLE] WHERE ([woID] = @woID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");

                if (NumRowsAffected > 0)
                {
                    b = true;
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

            return b;
        }
    }

    #endregion

    #region "Instruction according item requirement"

    public class WorkorderValidationItemInstructionForRequirement : WorkorderValidationItem
    {
        public WorkorderValidationItemInstructionForRequirement(int woID, int ItemValidationNo)
            : base(woID, ItemValidationNo)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;

            DataRow row = GetDefineRow(_itemValidationNo);
            if (row == null) return v;
            int requirement = Convert.ToInt32(row["Requirement"]);
            int instruction = Convert.ToInt32(row["instruction"]);
            int errorCode = Convert.ToInt32(row["errorCode"]);

            if (IsThisRequirementExist(requirement))
            {
                if (!IsThisInstructionExist(instruction))
                {
                    v = errorCode;
                }
            }
            return v;
        }

        private DataRow GetDefineRow(int vNumber)
        {
            DataRow rowResult = null;
            DataSet dt = Read.GetValidateItemRequirementXML();


            //Dim dr() As DataRow = tbl.Select(selectString):
            //'??? QuickFix
            int groupID = 142;

            if (Convert.ToInt32(_row["woType"]) != (int) NWorkorderType.Production)
            {
                groupID = 123;
            }

            foreach (DataRow row in dt.Tables[0].Rows)
            {
                int i = Convert.ToInt32(row["Value"]);
                int j = Convert.ToInt32(row["groupID"]);
                if (vNumber == i && groupID == Convert.ToInt32(row["groupID"]))
                {
                    rowResult = row;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }

            return rowResult;
        }

        private bool IsThisRequirementExist(int requirement)
        {
            bool b = false;

            DataTable tbl = WorkorderShared.getExistingWorkorderItems(_woID);
            if (tbl != null)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    if (requirement == Convert.ToInt32(row["requirement"]))
                    {
                        b = true;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }

            return b;
        }

        private bool IsThisInstructionExist(int instruction)
        {
            bool b = false;

            string SqlSelectString = "SELECT * FROM [WO_Instruction_Datatable] WHERE ([woID] = @woID)";
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            var SelectCommand1 = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int affectedRows = adapter1.Fill(ds1, "t1");
                if (affectedRows > 0)
                {
                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        if (instruction == Convert.ToInt32(row["InstructionID"]))
                        {
                            b = true;
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
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


            return b;
        }
    }

    #endregion

    #region "Item Name and leadtime"

    public class WorkorderValidationItemItemName : WorkorderValidationItem
    {
        public WorkorderValidationItemItemName(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;
            DataTable tbl = WorkorderShared.getExistingWorkorderItems(_woID);
            //Should Handling no item error here
            if (tbl != null)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    string itemName = Convert.ToString(row["estItemNameText"]);
                    if (itemName == "New Item")
                    {
                        v = (int) NWorkorderValidationError.ItemNameRequired;
                        break; // 
                    }
                }
            }

            return v;
        }
    }


    public class WorkorderValidationItemSumOfPrice : WorkorderValidationItem
    {
        public WorkorderValidationItemSumOfPrice(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;
            if (Convert.ToInt32(_row["woType"]) == (int) NWorkorderType.Production)
            {
                DataTable tbl = WorkorderShared.getExistingWorkorderItems(_woID);
                //Should Handling no item error here
                double sum = 0;
                if (tbl != null)
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        sum += Convert.ToDouble(row["qiAmount"]);
                    }
                }

                if (sum == 0)
                {
                    v = (int) NWorkorderValidationError.ItemPriceSumEqualsToZero;
                }
            }

            return v;
        }
    }


    public class WorkorderValidationItemItemLeadtime : WorkorderValidationItem
    {
        public WorkorderValidationItemItemLeadtime(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;
            DataTable tbl = WorkorderShared.getExistingWorkorderItems(_woID);
            //any item has a zero leadtie will cause the error

            int leadtime = 0;

            if (tbl != null)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    if (!Convert.IsDBNull(row["Leadtime"]))
                    {
                        leadtime = MyConvert.ConvertToInteger(row["Leadtime"]);
                        if (leadtime == 0)
                        {
                            v = (int) NWorkorderValidationError.ItemLeadtimeRequired;
                            break;
                        }
                    }
                }
            }
            return v;
        }
    }

    #endregion

    #region "Special Procedure"

    public class WorkorderValidationItemSpecialProcedure : WorkorderValidationItem
    {
        public WorkorderValidationItemSpecialProcedure(int woID)
            : base(woID)
        {
        }

        public override int Validate()
        {
            var v = (int) NWorkorderValidationError.Validated;

            if (specialprocedureApplicable())
            {
                if (!isThereSpecialProcedure())
                {
                    v = (int) NWorkorderValidationError.IsSpecialProcedureRequired;
                }
            }

            return v;
        }

        private bool specialprocedureApplicable()
        {
            bool b = Convert.ToBoolean(_row["SpecialProcedureApplicable"]);
            return b;
        }

        private bool isThereSpecialProcedure()
        {
            bool b = false;
            DataTable tbl = WorkorderShared.getExistingWorkorderItems(_woID);
            if (tbl != null)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    int woItemID = Convert.ToInt32(row["woItemID"]);
                    if (isThereSpecialProcedureOfThisWoIte(woItemID))
                    {
                        b = true;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }
            return b;
        }

        private bool isThereSpecialProcedureOfThisWoIte(int woItemID)
        {
            bool b = false;
            DataTable tbl = GetSpecialProcedureDatatable(woItemID);
            if (tbl != null)
            {
                if (tbl.Rows.Count > 0)
                {
                    b = true;
                }
            }

            return b;
        }


        private DataTable GetSpecialProcedureDatatable(int woItemID)
        {
            DataTable tbl = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [WO_Item_SpecialProcedure] WHERE ([woItemID] = @woItemID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woItemID", SqlDbType.Int).Value = woItemID;
            var ds1 = new DataSet();

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

    #endregion

    #region "Contract Amount"

    public class WorkorderValidationContractAmount : WorkorderValidationItem
    {
        private readonly int _workorderID;

        public WorkorderValidationContractAmount(int woID) : base(woID)
        {
            _workorderID = woID;
        }

        public override int Validate()
        {
            var mwv = new MyWorkorderValidationContractAmount(_workorderID);
            return mwv.Result;
        }
    }

    #endregion
}