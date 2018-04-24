//using System.Linq;

using System.Linq;

namespace CustomerDomain.Model
{

    public class MallDAL
{
    private readonly int _mallID;
    private readonly MallDataContext _dc;

    public VenderMall MallDetail
    {
        get { return _dc.VenderMalls.SingleOrDefault(c => c.MallID  == _mallID); }
    }

    public MallDAL(int mallID)
    {
        _mallID = mallID;
        _dc = new MallDataContext();
    }

}
}
