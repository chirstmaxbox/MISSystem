
using System.Linq;


namespace ExportDomain.Handler
{
    /// <summary>
    /// Export handler

    /// </summary>
    public class ExportHandlerSelect
    {
        public string UrlA { get { return GetHandlerUrlA(); } }
        public string Text { get { return GetHandlerText(); } }
        
        private readonly ExportHandler _handler;
  
        public ExportHandlerSelect(int exportID)
        {          
          var  dbml = new ExportHandlerDataContext();
           _handler  = dbml.ExportHandlers.SingleOrDefault(x => x.ExportID  ==exportID);
        }
        
    private string GetHandlerUrlA()
     {
        string url = "";
  
        if (_handler!=null)
        {
            url = _handler.UrlA;
        }
        return url;
     }

    private string GetHandlerText()
     {
        string txt = "na";
  
        if (_handler!=null)
        {
            txt = _handler.Text ;
        }
        return txt;
     }

 
    }
    
}
