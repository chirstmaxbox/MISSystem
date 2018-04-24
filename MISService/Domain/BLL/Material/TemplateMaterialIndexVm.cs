using System.Collections.Generic;
using System.Linq;
using SpecDomain.Model;

namespace SpecDomain.BLL.Material
{
    public class TemplateMaterialIndexVm
    {
        private int _productID = 263;
        public List<EST_Cost_Template_Material> Values { get; set; }

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
 
        public TemplateMaterialIndexVm()
        {
            Values = _db.EST_Cost_Template_Material.Where( x => x.DbItemID >=0).ToList();
        }


    }
}