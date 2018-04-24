using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyCommon;
using SpecDomain.Model;


namespace SpecDomain.BLL.Material
{
    public class MaterialCategoryMaintenanceVm:SpecDomain .Model .Material 
    {
        public IEnumerable<SelectListItem> Categories0 { get; set; }
        public IEnumerable<SelectListItem> Categories1 { get; set; }
        public IEnumerable<SelectListItem> Categories2 { get; set; }
        public IEnumerable<SelectListItem> Categories3 { get; set; }
        public IEnumerable<SelectListItem> Categories4 { get; set; }
        
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public MaterialCategoryMaintenanceVm()
        {
            var material = _db.Materials.Find(0);
            MyReflection.Copy(material, this);
        }

        public void RefreshDropdownlist()
        {
            var temp0 = _db.MaterialCategory0.Where(x => x.CategoryID >= 0)
                                             .OrderBy(x => x.CategoryName)
                                             .ToList();
            Categories0 = temp0.Select(x => new SelectListItem
            {
                Value = Convert.ToString(x.CategoryID),
                Text = x.CategoryName
            });

            var temp1 = _db.MaterialCategory1.Where(x => x.ParentID  == CategoryID0 || x.CategoryID == 0)
                                             .OrderBy(x => x.CategoryName)
                                             .ToList();
            Categories1 = temp1.Select(x => new SelectListItem
            {
                Value = Convert.ToString(x.CategoryID),
                Text = x.CategoryName
            });

            var temp2 = _db.MaterialCategory2.Where(x => x.ParentID == CategoryID1 || x.CategoryID == 0)
                                             .OrderBy(x => x.CategoryName)
                                             .ToList();
            Categories2 = temp2.Select(x => new SelectListItem
            {
                Value = Convert.ToString(x.CategoryID),
                Text = x.CategoryName
            });

            var temp3 = _db.MaterialCategory3.Where(x => x.ParentID == CategoryID2 || x.CategoryID == 0)
                                             .OrderBy(x => x.CategoryName)
                                             .ToList();
            Categories3 = temp3.Select(x => new SelectListItem
            {
                Value = Convert.ToString(x.CategoryID),
                Text = x.CategoryName
            });

            var temp4 = _db.MaterialCategory4.Where(x => x.ParentID == CategoryID3 || x.CategoryID == 0)
                                       .OrderBy(x => x.CategoryName)
                                       .ToList();
            Categories4 = temp4.Select(x => new SelectListItem
            {
                Value = Convert.ToString(x.CategoryID),
                Text = x.CategoryName
            });

        }


        public void Update(int categoryTypeID, List<MyKeyValuePair> tobeUpdatedItems)
        {
            var foo = new MaterialCategoryFactory(categoryTypeID);
            var cat = foo.Value;
            var parentID = GetParentID(categoryTypeID);
            foreach (var kvp in tobeUpdatedItems)
            {
                 cat.Update(kvp.Key, parentID, kvp.Value, 1);
            }
        }

        public void Create(int categoryTypeID, List<MyKeyValuePair> tobeInsertedNewItems)
        {
            var foo = new MaterialCategoryFactory(categoryTypeID);
            var cat = foo.Value;
            var parentID = GetParentID(categoryTypeID );
            foreach (var kvp in tobeInsertedNewItems)
            {
                cat.CreateCategoryID( 0,parentID, kvp.Value);
            }
        }

        private int GetParentID(int categoryTypeID)
        {
            var i = 0;
            switch (categoryTypeID)
            {
                case 0:
                    i = 0;
                    break;
                case 1:
                    i = CategoryID0; 
                    break;
                case 2:
                    i = CategoryID1; 
                    break;
                case 3:
                     i = CategoryID2 ;
                    break;
                default:
                    i = CategoryID3; 
                    break;
            }
            return i;
        }
 
    }

    public class MaterialCategoryCleanup
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        //public MaterialCategoryCleanup()
        //{
        //    //Level 0:
        //    var cat0s = _db.MaterialCategory0.Where(x => x.CategoryID > 0).OrderBy(x => x.CategoryID).ToList();
        //    foreach (var cat0 in cat0s)
        //    {
        //        if (cat0.Materials.Any()) continue;
        //        var cat1s = cat0.MaterialCategory1.ToList();
        //       foreach (var cat1 in cat1s)
        //        {
        //            if (cat1.Materials.Any()) continue;
        //            var cat2s = cat1.MaterialCategory2.ToList();
        //            foreach (var cat2 in cat2s)
        //            {
        //                if (cat2.Materials.Any()) continue;
        //                var cat3s = cat2.MaterialCategory3.ToList();
        //                foreach (var cat3 in cat3s)
        //                {
        //                    if (cat3.Materials.Any()) continue;
        //                    var cat4s = cat3.MaterialCategory4.ToList();
        //                    foreach (var cat4 in cat4s)
        //                    {
        //                        if (cat4.Materials.Any()) continue;
        //                        _db.Entry(cat4).State = EntityState.Deleted;
        //                    }
        //                    _db.SaveChanges();
        //                    _db.Entry(cat3).State = EntityState.Deleted;
        //                }
        //                _db.SaveChanges();

        //                _db.Entry(cat2).State = EntityState.Deleted;
        //            }
        //            _db.SaveChanges();

        //            _db.Entry(cat1).State = EntityState.Deleted;
        //        }
        //        _db.SaveChanges();

        //        _db.Entry(cat0).State = EntityState.Deleted;
        //        _db.SaveChanges();
        //    }
        //}


        public MaterialCategoryCleanup()
        {
            DeleteCategor4();
            DeleteCategor3();
            DeleteCategor2();
            DeleteCategor1();
            DeleteCategor0();

        }

        //private bool DeleteCategory()
        //{
        //    var isRefresh = false;
        //    var cat0s = _db.MaterialCategory0.Where(x => x.CategoryID > 0).OrderBy(x => x.CategoryID).ToList();
        //    foreach (var cat0 in cat0s)
        //    {
        //        var cat1s = cat0.MaterialCategory1.ToList();
        //        foreach (var cat1 in cat1s)
        //        {
        //            var cat2s = cat1.MaterialCategory2.ToList();
        //            foreach (var cat2 in cat2s)
        //            {
        //                var cat3s = cat2.MaterialCategory3.ToList();
        //                foreach (var cat3 in cat3s)
        //                {
        //                    var cat4s = cat3.MaterialCategory4.ToList();
        //                    foreach (var cat4 in cat4s)
        //                    {
        //                        if (cat4.Materials.Any()) continue;
        //                        _db.Entry(cat4).State = EntityState.Deleted;
        //                        _db.SaveChanges();
        //                        isRefresh = true;
        //                        break;
        //                    }
        //                    if (isRefresh) break;
        //                    if (cat3.Materials.Any()) continue;
        //                    _db.Entry(cat3).State = EntityState.Deleted;
        //                    _db.SaveChanges();
        //                    isRefresh = true;
        //                    break;
        //                }
        //                if (isRefresh) break;
        //                if (cat2.Materials.Any()) continue;
        //                _db.Entry(cat2).State = EntityState.Deleted;
        //                _db.SaveChanges();
        //                isRefresh = true;
        //                break;
        //            }
        //            if (isRefresh) break;
        //            if (cat1.Materials.Any()) continue;
        //            _db.Entry(cat1).State = EntityState.Deleted;
        //            _db.SaveChanges();
        //            isRefresh = true;
        //            break;
        //        }
        //        if (isRefresh) break;
        //        if (cat0.Materials.Any()) continue;
        //        _db.Entry(cat0).State = EntityState.Deleted;
        //        _db.SaveChanges();
        //        isRefresh = true;
        //        break;
        //    }
        //    return isRefresh;
        //}


        private void DeleteCategor4()
        {
            var cat0s = _db.MaterialCategory0.Where(x => x.CategoryID > 0).OrderBy(x => x.CategoryID).ToList();
            foreach (var cat0 in cat0s)
            {
                var cat1s = cat0.MaterialCategory1.ToList();
                foreach (var cat1 in cat1s)
                {
                    var cat2s = cat1.MaterialCategory2.ToList();
                    foreach (var cat2 in cat2s)
                    {
                        var cat3s = cat2.MaterialCategory3.ToList();
                        foreach (var cat3 in cat3s)
                        {
                            var cat4s = cat3.MaterialCategory4.ToList();
                            foreach (var cat4 in cat4s)
                            {
                                if (cat4.Materials.Any()) continue;
                                _db.Entry(cat4).State = EntityState.Deleted;
                            }
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }


        private void DeleteCategor3()
        {
            var cat0s = _db.MaterialCategory0.Where(x => x.CategoryID > 0).OrderBy(x => x.CategoryID).ToList();
            foreach (var cat0 in cat0s)
            {
                var cat1s = cat0.MaterialCategory1.ToList();
                foreach (var cat1 in cat1s)
                {
                    var cat2s = cat1.MaterialCategory2.ToList();
                    foreach (var cat2 in cat2s)
                    {
                        var cat3s = cat2.MaterialCategory3.ToList();
                        foreach (var cat3 in cat3s)
                        {
                            if (cat3.Materials.Any()) continue;
                            _db.Entry(cat3).State = EntityState.Deleted;
                        }
                        _db.SaveChanges();
                    }
                }
            }
        }


        private void DeleteCategor2()
        {
            var cat0s = _db.MaterialCategory0.Where(x => x.CategoryID > 0).OrderBy(x => x.CategoryID).ToList();
            foreach (var cat0 in cat0s)
            {
                var cat1s = cat0.MaterialCategory1.ToList();
                foreach (var cat1 in cat1s)
                {
                    var cat2s = cat1.MaterialCategory2.ToList();
                    foreach (var cat2 in cat2s)
                    {
                            if (cat2.Materials.Any()) continue;
                            _db.Entry(cat2).State = EntityState.Deleted;
                        
                    }
                    _db.SaveChanges();
                }
            }
        }


        private void DeleteCategor1()
        {
            var cat0s = _db.MaterialCategory0.Where(x => x.CategoryID > 0).OrderBy(x => x.CategoryID).ToList();
            foreach (var cat0 in cat0s)
            {
                var cat1s = cat0.MaterialCategory1.ToList();
                foreach (var cat1 in cat1s)
                {

                        if (cat1.Materials.Any()) continue;
                        _db.Entry(cat1).State = EntityState.Deleted;
                }
                _db.SaveChanges();

            }
        }



        private void DeleteCategor0()
        {
            var cat0s = _db.MaterialCategory0.Where(x => x.CategoryID > 0).OrderBy(x => x.CategoryID).ToList();
            foreach (var cat0 in cat0s)
            {

                    if (cat0.Materials.Any()) continue;
                    _db.Entry(cat0).State = EntityState.Deleted;
                }
                                   _db.SaveChanges();
            }

 

        }


    

    #region Reference Only, Not in actual use

    public class MaterialCategoryDelete
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public MaterialCategoryDelete()
        {
            var cat0s = _db.MaterialCategory0.Where(x => x.CategoryID > 0).ToList();
            foreach (var cat0 in cat0s)
            {
                var cat1s = cat0.MaterialCategory1.ToList();
                foreach (var cat1 in cat1s)
                {
                    var cat2s = cat1.MaterialCategory2.ToList();
                    foreach (var cat2 in cat2s)
                    {
                        var cat3s = cat2.MaterialCategory3.ToList();
                        foreach (var cat3 in cat3s)
                        {
                            var b = false;
                            var cat4s = cat3.MaterialCategory4.ToList();
                            foreach (var cat4 in cat4s)
                            {
                                if (cat4.Materials.Any()) continue;
                                //cat4.MaterialCategory3.MaterialCategory2.MaterialCategory1.MaterialCategory0.
  
                                _db.Entry(cat4).State = EntityState.Deleted;
                                b = true;
                            }
                            if (b)
                            {
                                _db.SaveChanges();
                            }
                        }
                    }
                }
            }
        }
    }

    public class MaterialCategoryDelete0
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        private readonly MaterialCategory0 _category0;

        public bool IsDeletable { get; set; }
        public MaterialCategoryDelete0(MaterialCategory0 category0 )
        {
            _category0 = category0;
            IsDeletable = !_category0.Materials.Any();
        }

        public void Delete()
        {
            if (!IsDeletable) return;

            var cat1s = _category0.MaterialCategory1.ToList();
            foreach (var cat1 in cat1s)
            {
                var cat2s = cat1.MaterialCategory2.ToList();
                foreach (var cat2 in cat2s)
                {
                    var cat3s = cat2.MaterialCategory3.ToList();
                    foreach (var cat3 in cat3s)
                    {
                        var cat4s = cat3.MaterialCategory4.ToList();
                        foreach (var cat4 in cat4s)
                        {
                            _db.Entry(cat4).State = EntityState.Deleted;
                        }
                        _db.SaveChanges();

                        _db.Entry(cat3).State = EntityState.Deleted;
                    }
                    _db.SaveChanges();

                    _db.Entry(cat2).State = EntityState.Deleted;
                }
                _db.SaveChanges();

                _db.Entry(cat1).State = EntityState.Deleted;
            }
            _db.SaveChanges();

            _db.Entry(_category0).State = EntityState.Deleted;
            _db.SaveChanges();
        }

    }

    public class MaterialCategoryDelete1
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        private readonly MaterialCategory1 _category1;

        public bool IsDeletable { get; set; }
        public MaterialCategoryDelete1(MaterialCategory1 category1)
        {
            _category1 = category1;
            IsDeletable = !_category1.Materials.Any();
        }

        public void Delete()
        {
            if (!IsDeletable) return;

                var cat2s = _category1 .MaterialCategory2.ToList();
                foreach (var cat2 in cat2s)
                {
                    var cat3s = cat2.MaterialCategory3.ToList();
                    foreach (var cat3 in cat3s)
                    {
                        var cat4s = cat3.MaterialCategory4.ToList();
                        foreach (var cat4 in cat4s)
                        {
                            _db.Entry(cat4).State = EntityState.Deleted;
                        }
                        _db.SaveChanges();

                        _db.Entry(cat3).State = EntityState.Deleted;
                    }
                    _db.SaveChanges();

                    _db.Entry(cat2).State = EntityState.Deleted;
                }
                _db.SaveChanges();

   
            _db.Entry(_category1).State = EntityState.Deleted;
            _db.SaveChanges();
        }

    }

    public class MaterialCategoryDelete2
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        private readonly MaterialCategory2 _category2;

        public bool IsDeletable { get; set; }
        public MaterialCategoryDelete2(MaterialCategory2 category2)
        {
            _category2 = category2;
            IsDeletable = !_category2.Materials.Any();
        }

        public void Delete()
        {
            if (!IsDeletable) return;

                var cat3s = _category2.MaterialCategory3.ToList();
                foreach (var cat3 in cat3s)
                {
                    var cat4s = cat3.MaterialCategory4.ToList();
                    foreach (var cat4 in cat4s)
                    {
                        _db.Entry(cat4).State = EntityState.Deleted;
                    }
                    _db.SaveChanges();

                    _db.Entry(cat3).State = EntityState.Deleted;
                }
                _db.SaveChanges();

                _db.Entry(_category2).State = EntityState.Deleted;
                _db.SaveChanges();
        }

    }

    public class MaterialCategoryDelete3
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        private readonly MaterialCategory3 _category3;

        public bool IsDeletable { get; set; }
        public MaterialCategoryDelete3(MaterialCategory3 category3)
        {
            _category3 = category3;
            IsDeletable = !_category3.Materials.Any();
        }

        public void Delete()
        {
            if (!IsDeletable) return;
                var cat4s = _category3.MaterialCategory4.ToList();
                foreach (var cat4 in cat4s)
                {
                    _db.Entry(cat4).State = EntityState.Deleted;
                }
                _db.SaveChanges();

           _db.Entry(_category3).State = EntityState.Deleted;
            _db.SaveChanges();
        }

    }

    public class MaterialCategoryDelete4
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
        private readonly MaterialCategory4 _category4;

        public bool IsDeletable { get; set; }
        public MaterialCategoryDelete4(MaterialCategory4 category4)
        {
            _category4 = category4;
            IsDeletable = !_category4.Materials.Any();
        }

        public void Delete()
        {
            if (!IsDeletable) return;
            _db.Entry(_category4).State = EntityState.Deleted;
            _db.SaveChanges();
        }

    }
#endregion

}