using System.Collections.Generic;
using System.Web.Mvc;

namespace SpecDomain.BO
{
    public interface  ISelecListItems
    {
        IEnumerable<SelectListItem> Values { get;set; }
    }
}