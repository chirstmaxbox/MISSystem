using System.Data;

namespace SalesCenterDomain.BDL.Quote
{
    public class QuoteChildren
    {
        //recordType = "Q";
        private readonly int _myID;

        public QuoteChildren(int quoteRevID)
        {
            _myID = quoteRevID;
        }

        public DataTable ItemList(string recordType, bool isFinalOnly)
        {
            var items = new QuoteChildrenItemTable(_myID);
            return items.GetItems(recordType, true);
        }
    }
}