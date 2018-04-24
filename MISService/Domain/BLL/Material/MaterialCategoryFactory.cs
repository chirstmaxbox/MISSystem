using System;

using System.Data.Entity;
using System.Linq;
using MyCommon;
using SpecDomain.Model;

namespace SpecDomain.BLL.Material
{
    public class MaterialCategoryFactory
    {
        public IMaterialCategory Value { get; private set; }
        public MaterialCategoryFactory(int categoryType)
        {
            switch (categoryType )
            {
                case 0:
                    Value =new MyMaterialCategory0();
                    break;
                case 1:
                    Value =new MyMaterialCategory1();
                    break;
                case 2:
                    Value = new MyMaterialCategory2();
                    break;
                case 3:
                    Value = new MyMaterialCategory3();
                    break;
                default :
                    Value =new MyMaterialCategory4();
                    break;
            }
        }
    }


    public interface IMaterialCategory
    {
        void Update(int categoryID, int parentID, string categoryName, Int16 statusID);
        //if categoryID=0 return newID
        int CreateCategoryID(int categoryID, int parentID, string categoryName);
        //if CategoryName does not exist return newID
        int FindCategoryID(int parentID, string categoryName);   
        //void Delete();
    }


    public class MyMaterialCategory0 : IMaterialCategory
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        //ID did not change
        public void Update(int categoryID, int parentID, string categoryName, Int16 statusID)
        {
            var cat = _db.MaterialCategory0.Find(categoryID);
            cat.ParentID = parentID;
            cat.CategoryName = categoryName;
            cat.Status = statusID;
            _db.Entry(cat).State = EntityState.Modified;
            _db.SaveChanges();
        }

        //ByID
        public int CreateCategoryID(int categoryID, int parentID, string categoryName)
        {
            return categoryID > 0 ? categoryID : CreateNewCategory(parentID, categoryName);
        }

        private int CreateNewCategory(int parentID, string categoryName)
        { 
            var newCat = new MaterialCategory0      //Differ
            {
                CategoryName = categoryName,
                ParentID = parentID,
            };

            _db.MaterialCategory0.Add(newCat);        //Differ
            _db.SaveChanges();

            return newCat.CategoryID;
        }


        public int FindCategoryID( int parentID, string categoryName)
        {
            if (MyConvert.IsNullString(categoryName)) return 0;
    
            var categoryID = 0;
            var existingName = MyString.RemoveSpace(categoryName);
            var cats = _db.MaterialCategory0.Where(x => x.ParentID == parentID && x.CategoryID > 0)     //Differ
                                           .OrderBy(x => x.CategoryName)
                                           .ToList();
            if (cats.Count > 0)
            {
                foreach (var cat in cats)
                {
                    if (MyString.RemoveSpace(cat.CategoryName) != existingName) continue;
                    categoryID = cat.CategoryID;
                    break;
                }
            }

            return categoryID > 0 ? categoryID : CreateNewCategory(parentID, categoryName);
        }
        
    }

    public class MyMaterialCategory1 : IMaterialCategory
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        //ID did not change
        public void Update(int categoryID, int parentID, string categoryName, Int16 statusID)
        {
            var cat = _db.MaterialCategory1.Find(categoryID);
            cat.ParentID = parentID;
            cat.CategoryName = categoryName;
            cat.Status = statusID;
            _db.Entry(cat).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public int CreateCategoryID(int categoryID, int parentID, string categoryName)
        {
            return categoryID > 0 ? categoryID : CreateNewCategory(parentID, categoryName);
        }

        private int CreateNewCategory(int parentID, string categoryName)
    {
            var newCat = new MaterialCategory1      //Differ
            {
                CategoryName = categoryName,
                ParentID = parentID,
            };

            _db.MaterialCategory1.Add(newCat);        //Differ
            _db.SaveChanges();

            return newCat.CategoryID;
        }


        public int FindCategoryID(int parentID, string categoryName)
        {
            if (MyConvert.IsNullString(categoryName)) return 0;

            var categoryID = 0;
            var existingName = MyString.RemoveSpace(categoryName);
            var cats = _db.MaterialCategory1.Where(x => x.ParentID == parentID && x.CategoryID > 0)     //Differ
                                           .OrderBy(x => x.CategoryName)
                                           .ToList();
            if (cats.Count > 0)
            {
                foreach (var cat in cats)
                {
                    if (MyString.RemoveSpace(cat.CategoryName) != existingName) continue;
                    categoryID = cat.CategoryID;
                    break;
                }
            }

            return categoryID > 0 ? categoryID : CreateNewCategory(parentID, categoryName);
        }
    }


    public class MyMaterialCategory2 : IMaterialCategory
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        //ID did not change
        public void Update(int categoryID, int parentID, string categoryName, Int16 statusID)
        {
            var cat = _db.MaterialCategory2.Find(categoryID);
            cat.ParentID = parentID;
            cat.CategoryName = categoryName;
            cat.Status = statusID;
            _db.Entry(cat).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public int CreateCategoryID(int categoryID, int parentID, string categoryName)
        {
            return categoryID > 0 ? categoryID : CreateNewCategory(parentID, categoryName);
        }

        private int CreateNewCategory(int parentID, string categoryName)
        {
            if (MyConvert.IsNullString(categoryName)) return 0;
            var newCat = new MaterialCategory2      //Differ
            {
                CategoryName = categoryName,
                ParentID = parentID,
            };

            _db.MaterialCategory2.Add(newCat);        //Differ
            _db.SaveChanges();

            return newCat.CategoryID;
        }


        public int FindCategoryID(int parentID, string categoryName)
        {
            if (MyConvert.IsNullString(categoryName)) return 0;

            var categoryID = 0;
            var existingName = MyString.RemoveSpace(categoryName);
            var cats = _db.MaterialCategory2.Where(x => x.ParentID == parentID && x.CategoryID > 0)     //Differ
                                           .OrderBy(x => x.CategoryName)
                                           .ToList();
            if (cats.Count > 0)
            {
                foreach (var cat in cats)
                {
                    if (MyString.RemoveSpace(cat.CategoryName) != existingName) continue;
                    categoryID  = cat.CategoryID;
                    break;
                }
            }

            return categoryID > 0 ? categoryID : CreateNewCategory(parentID, categoryName);
        }
    }


    public class MyMaterialCategory3 : IMaterialCategory
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        //ID did not change
        public void Update(int categoryID, int parentID, string categoryName, Int16 statusID)
        {
            var cat = _db.MaterialCategory3.Find(categoryID);
            cat.ParentID = parentID;
            cat.CategoryName = categoryName;
            cat.Status = statusID;
            _db.Entry(cat).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public int CreateCategoryID(int categoryID, int parentID, string categoryName)
        {
            return categoryID > 0 ? categoryID : CreateNewCategory(parentID, categoryName);
        }

        private int CreateNewCategory(int parentID, string categoryName)

        {
            if (MyConvert.IsNullString(categoryName)) return 0;

            var newCat = new MaterialCategory3      //Differ
            {
                CategoryName = categoryName,
                ParentID = parentID,
            };

            _db.MaterialCategory3.Add(newCat);        //Differ
            _db.SaveChanges();

            return newCat.CategoryID;
        }


        public int FindCategoryID(int parentID, string categoryName)
        {
            if (MyConvert.IsNullString(categoryName)) return 0;

            var categoryID = 0;
            var existingName = MyString.RemoveSpace(categoryName);
            var cats = _db.MaterialCategory3.Where(x => x.ParentID == parentID && x.CategoryID > 0)     //Differ
                                           .OrderBy(x => x.CategoryName)
                                           .ToList();
            if (cats.Count > 0)
            {
                foreach (var cat in cats)
                {
                    if (MyString.RemoveSpace(cat.CategoryName) != existingName) continue;
                    categoryID  = cat.CategoryID;
                    break;
                }
            }

            return categoryID > 0 ? categoryID : CreateNewCategory(parentID, categoryName);
        }
    }


    public class MyMaterialCategory4 : IMaterialCategory
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        //ID did not change
        public void Update(int categoryID, int parentID, string categoryName, Int16 statusID)
        {
            var cat = _db.MaterialCategory4.Find(categoryID);
            cat.ParentID = parentID;
            cat.CategoryName = categoryName;
            cat.Status = statusID;
            _db.Entry(cat).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public int CreateCategoryID(int categoryID, int parentID, string categoryName)
        {
            return categoryID > 0 ? categoryID : CreateNewCategory(parentID, categoryName);
        }

        private int CreateNewCategory(int parentID, string categoryName)
        {
            if (MyConvert.IsNullString(categoryName)) return 0;
            var newCat = new MaterialCategory4      //Differ
            {
                CategoryName = categoryName,
                ParentID = parentID,
            };

            _db.MaterialCategory4.Add(newCat);        //Differ
            _db.SaveChanges();

            return newCat.CategoryID;
        }

        public int FindCategoryID(int parentID, string categoryName)
        {
            if (MyConvert.IsNullString(categoryName)) return 0;
            var categoryID = 0;
            var existingName = MyString.RemoveSpace(categoryName);
            var cats = _db.MaterialCategory4.Where(x => x.ParentID == parentID && x.CategoryID > 0)     //Differ
                                           .OrderBy(x => x.CategoryName)
                                           .ToList();
            if (cats.Count > 0)
            {
                foreach (var cat in cats)
                {
                    if (MyString.RemoveSpace(cat.CategoryName) != existingName) continue;
                    categoryID  = cat.CategoryID;
                    break;
                }
            }

            return categoryID > 0 ? categoryID : CreateNewCategory(parentID, categoryName);
        }
    }
}