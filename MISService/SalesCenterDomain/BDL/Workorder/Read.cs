using System.Data;
using System.Web;

namespace SalesCenterDomain.BDL.Workorder
{
    public class Read
    {
        public static DataSet GetValidateItemRequirementXML()
        {
            var dt = new DataSet();
            dt.ReadXml(HttpContext.Current.Server.MapPath("ValidateItemRequirement.xml"));
            return dt;
        }
    }
}