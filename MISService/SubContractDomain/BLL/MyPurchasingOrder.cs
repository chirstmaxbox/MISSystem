using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;

using SubContractDomain.Model;
using System.Data.Entity;


namespace SubContractDomain.BLL
{

	public class MyPurchasingOrder
	{
		private readonly SubcontractResponse _response;
		private readonly int _subcontractID;
		private readonly SubContractDbEntities _db = new SubContractDbEntities();
		
		public MyPurchasingOrder(int subcontractID)
		{
			_subcontractID = subcontractID;
			_response = _db.SubcontractResponses.FirstOrDefault(x => x.SubcontractID == _subcontractID);
		
		}

		public string CopyTo(int destinationSubcontractID)
		{
			string result = CopyTitle(destinationSubcontractID);
			if (result != "ok") return result;

			result = CopyItem(destinationSubcontractID);
			if (result != "ok") return result;

			result = CopyNote(destinationSubcontractID);
			return result;
		}

		private String CopyTitle(int destinationSubcontractID)
		{
		
			var desResponse = _db.SubcontractResponses.FirstOrDefault(x => x.SubcontractID == destinationSubcontractID );
			if (desResponse == null) return "No Destination Found.";
			desResponse.InstallerID  = _response.InstallerID;	

			try
			{
				//Check Validation Errors
				_db.Entry(desResponse).State = EntityState.Modified;
				_db.SaveChanges();

			}
			catch (DbEntityValidationException dbEx)
			{
				var s = dbEx.Message;
			}
		

			return "ok";
		}
		private String CopyItem(int destinationSubcontractID)
		{
			var oriItems = _db.SubcontractItems.Where(x => x.SubcontractID == _subcontractID).ToList();
			if (oriItems .Count >0)
			{
				foreach (var item in oriItems )
				{
					var newItem = new SubcontractItem()
					              	{
					              		Description = item.Description,
					              		OrderNumber = item.OrderNumber,
					              		Quantity = item.Quantity,
					              		SubcontractID = destinationSubcontractID,
					              		Title = item.Title,
					              		TotalCost = item.TotalCost,
					              		UnitCost = item.UnitCost


					              	};
					try
					{
						//Check Validation Errors
						_db.SubcontractItems.Add(newItem);
						_db.SaveChanges();
					}
					catch (DbEntityValidationException dbEx)
					{
						var s = dbEx.Message;
					}

					
				}
	
			}

			return "ok";
		}
		private String CopyNote(int destinationSubcontractID)
		{
			var oriNotes = _db.SubcontractNotes .Where(x => x.SubcontractID == _subcontractID).ToList();
			if (oriNotes.Count > 0)
			{
				foreach (var note in oriNotes)
				{
					var newNote = new SubcontractNote()
					{
						Description = note.Description,
						OrderNumber = note.OrderNumber,
						SubcontractID = destinationSubcontractID,

					};
					try
					{
						//Check Validation Errors
						_db.SubcontractNotes.Add(newNote);
						_db.SaveChanges();
					}
					catch (DbEntityValidationException dbEx)
					{
						var s = dbEx.Message;
					}
				}
			}

			return "ok";
		}


	}
}