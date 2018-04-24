
using System.Linq;

namespace ExportDomain.Entrance
{
    /// <summary>
    /// Website Entrance handler
    /// </summary>

    public class EntranceSelect
    {
        public string UrlA { get { return GetHandlerUrlA(); } }
        public string Text { get { return GetHandlerText(); } }
        
        private readonly Entrance _entrance;
  
        public EntranceSelect(int entranceID)
        {          
          var  dbml = new EntranceDataContext();
           _entrance  = dbml.Entrances.SingleOrDefault(x => x.EntranceID  ==entranceID);
        }
        
    private string GetHandlerUrlA()
     {
        string url = "";
  
        if (_entrance!=null)
        {
            url = _entrance.UrlA;
        }
        return url;
     }

    private string GetHandlerText()
     {
        string txt = "na";
  
        if (_entrance!=null)
        {
            txt = _entrance.Text ;
        }
        return txt;
     }

 
    }
    
}
