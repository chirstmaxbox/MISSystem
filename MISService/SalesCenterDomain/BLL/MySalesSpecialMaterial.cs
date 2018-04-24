using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using ProjectDomain;

namespace SalesCenterDomain.BLL
{
    public class MySalesSpecialMaterial
    {
        private readonly ProjectModelDbEntities _db = new ProjectModelDbEntities();
        private readonly int _jobID;

        public MySalesSpecialMaterial(int jobID)
        {
            _jobID = jobID;
        }

        public List<Sales_SpecialMaterial> SpecialMaterials
        {
            get { return GetSpecialMaterials(); }
        }

        //public Sales_SpecialMaterial Value  { get; set; }
        //Set by Create
        public int ID { get; private set; }

        private List<Sales_SpecialMaterial> GetSpecialMaterials()
        {
            return _db.Sales_SpecialMaterial.Where(x => x.JobID == _jobID).ToList();
        }


        public void Create()
        {
            var specialMaterial = new Sales_SpecialMaterial
                                      {
                                          JobID = _jobID,
                                          StatusID = 0,
                                          LsmID = 0,
                                          Leadtime = 0,
                                          ItemNum = 0,
                                          HandlingProcess = "Choose"
                                      };

            _db.Sales_SpecialMaterial.Add(specialMaterial);

            try
            {
                //Check Validation Errors
                IEnumerable error = _db.GetValidationErrors();
                _db.SaveChanges();
                ID = specialMaterial.JobSpecialMaterialID;
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
            }
        }
    }


    public class SpecialMaterialAndProcedure
    {
        private readonly int _jobID;

        public SpecialMaterialAndProcedure(int jobID)
        {
            _jobID = jobID;
        }

        public List<short> TaskCategories
        {
            get { return GetCategories(); }
        }

        private List<short> GetCategories()
        {
            bool b201 = false;
            bool b501 = false;
            bool b551 = false;

            var msm = new MySalesSpecialMaterial(_jobID);
            foreach (Sales_SpecialMaterial sm in msm.SpecialMaterials)
            {
                if (sm.HandlingProcess == "Estimation")
                {
                    b201 = true;
                }

                if (sm.HandlingProcess == "Graphic Drw.")
                {
                    b501 = true;
                }

                if (sm.HandlingProcess == "Structural Drw.")
                {
                    b551 = true;
                }
            }

            var categories = new List<short>();
            if (b201)
            {
                categories.Add(201);
            }

            if (b501)
            {
                categories.Add(501);
            }

            if (b551)
            {
                categories.Add(551);
            }

            return categories;
        }
    }
}