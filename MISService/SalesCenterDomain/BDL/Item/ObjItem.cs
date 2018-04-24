using System.Data;
using System.Data.SqlClient;
using MyCommon.MyEnum;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Item
{
    public abstract class ObjItem
    {
        public int ItemID;
        public int ItemNameDetailsID;
        public string ItemNameText = "";

        public abstract void Update();
    }

    public class ObjItemEstimation : ObjItem
    {
        public override void Update()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [EST_Item] SET [estItemNameText] = @estItemNameText, [NameDetailsID] = @NameDetailsID WHERE [estItemID] = @estItemID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);
                UPdateCommand.Parameters.Add("@estItemID", SqlDbType.Int).Value = ItemID;
                UPdateCommand.Parameters.Add("@NameDetailsID", SqlDbType.Int).Value = ItemNameDetailsID;
                UPdateCommand.Parameters.Add("@estItemNameText", SqlDbType.NVarChar, 300).Value = ItemNameText;

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
    }

    public class ObjItemQuote : ObjItem
    {
        public override void Update()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Quote_Item] SET  [NameDetailsID] = @NameDetailsID,  [qiItemTitle] = @qiItemTitle  WHERE [quoteItemID] = @quoteItemID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@quoteItemID", SqlDbType.Int).Value = ItemID;
                UPdateCommand.Parameters.Add("@NameDetailsID", SqlDbType.Int).Value = ItemNameDetailsID;
                UPdateCommand.Parameters.Add("@qiItemTitle", SqlDbType.NVarChar, 300).Value = ItemNameText;

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
    }


    public class ObjItemWorkorder : ObjItem
    {
        private int _itemRequirement = (int) NWorkorderRequirement.Installation;

        public int ItemRequirement
        {
            set
            {
                _itemRequirement = value;
                UpdateRequirement();
            }
        }

        public override void Update()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [WO_Item] SET [estItemNameText] = @estItemNameText, [NameDetailsID] = @NameDetailsID  WHERE [woItemID] = @woItemID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);
                UPdateCommand.Parameters.Add("@woItemID", SqlDbType.Int).Value = ItemID;
                UPdateCommand.Parameters.Add("@NameDetailsID", SqlDbType.Int).Value = ItemNameDetailsID;
                UPdateCommand.Parameters.Add("@estItemNameText", SqlDbType.NVarChar, 300).Value = ItemNameText;

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


        private void UpdateRequirement()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE [WO_Item] SET  [Requirement]=@Requirement WHERE [woItemID] = @woItemID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);
                UPdateCommand.Parameters.Add("@woItemID", SqlDbType.Int).Value = ItemID;
                UPdateCommand.Parameters.Add("@Requirement", SqlDbType.Int).Value = _itemRequirement;

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
    }
}