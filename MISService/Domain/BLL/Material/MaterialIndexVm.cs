using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SpecDomain.Model;
using SpecDomain.BO;

namespace SpecDomain.BLL.Material
{
    public class MaterialIndexVm
    {
        //Craate
        public MaterialBase Value { get; set; }
        //Index
        public List<MaterialBase> Values { get; set; }

        public void Refresh()
        {
            var db = new SpecificationDbEntities();
            Values = new List<MaterialBase>();

            List<Model.Material> materials;

            switch (Value.TypeID)
            {
                case (int) NMaterialGenerateType.ByMaterialAdministrator:
                    materials = db.Materials.Where(x => Value.CategoryID0 > 0 &&
                                                        Value.CategoryID0 == x.CategoryID0 &&
                                                        (Value.CategoryID1 == 0 || x.CategoryID1 == Value.CategoryID1) &&
                                                        (Value.CategoryID2 == 0 || x.CategoryID2 == Value.CategoryID2) &&
                                                        (Value.CategoryID3 == 0 || x.CategoryID3 == Value.CategoryID3) &&
                                                        x.TypeID ==0
                        ).ToList();
                    break;

                    //case (int)NMaterialGenerateType.ByWorker:
                    //case (int)NMaterialGenerateType.ByEstimator:
                default:
                    materials = db.Materials.Where(x => x.TypeID == Value.TypeID).ToList();
                    break;

            }

            materials = materials.OrderBy(x => x.MaterialCategory1.CategoryName)
                .ThenBy(x => x.MaterialCategory2.CategoryName)
                .ThenBy(x => x.MaterialCategory3.CategoryName)
                .ThenBy(x => x.MaterialCategory4.CategoryName)
                .ToList();

            foreach (var material in materials)
            {
                var mb = new MaterialBase(material);
                Values.Add(mb);
            }

        }
    }


    public class MaterialMultipleEditVm
    {
        public bool IsNewCategoryCreated { get; set; }

        public IEnumerable<SelectListItem> Categories0 { get; set; }
        public SpecDomain.Model.Material Value { get; set; }

        public List<MaterialBase> Values { get; set; }

         public void Refresh(SpecDomain.Model.Material material)
         {
             //Value
             Value = material;

             //Values
            var  db = new SpecificationDbEntities();
            Values = new List<MaterialBase>();
            var materials = db.Materials.Where(x => Value.CategoryID0 > 0 && 
                                                     Value.CategoryID0== x.CategoryID0  &&
                                                    (Value.CategoryID1 == 0 || x.CategoryID1 == Value.CategoryID1) &&
                                                    (Value.CategoryID2 == 0 || x.CategoryID2 == Value.CategoryID2) &&
                                                    (Value.CategoryID3 == 0 || x.CategoryID3 == Value.CategoryID3)
                                                ).OrderBy(x => x.MaterialCategory1.CategoryName)
                                                 .ThenBy(x => x.MaterialCategory2.CategoryName)
                                                 .ThenBy(x => x.MaterialCategory3.CategoryName)
                                                 .ThenBy(x => x.MaterialCategory4.CategoryName)
                                                .ToList();

            foreach (var m in materials)
            {
                var mb = new MaterialBase(m);
                Values.Add(mb);
            }

            //Dropdownlist
            var temp0 = db.MaterialCategory0.Where(x => x.CategoryID >= 0)
                                             .OrderBy(x => x.CategoryName)
                                             .ToList();
            Categories0 = temp0.Select(x => new SelectListItem
            {
                Value = Convert.ToString(x.CategoryID),
                Text = x.CategoryName
            });

        }

        public void Update(List<MaterialBase> toBeUpdatedMaterials)
        {
    
            foreach (var mb in toBeUpdatedMaterials )
            {
                mb.Update();
            }
        }

    }

}



