
using PermitDomain.Model;


namespace PermitDomain.BLL
{
    public class MyApplicationDocument
    {
      
		public int NewlyInsertedID { get; private set; }
	      private readonly PermitDbEntities _db = new PermitDbEntities();
  
		public MyApplicationDocument()
        {
    
        }


		public void Create(PermitDocument doc)
		{
			_db.PermitDocuments.Add(doc);
			_db.SaveChanges();
			NewlyInsertedID = doc.DocID ;
         
		}


    }
}

