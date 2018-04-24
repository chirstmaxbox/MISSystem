using System.Data.Entity.Validation;
using System.Linq;
using MyCommon;
using MyCommon.MyEnum;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.EstItem
{
    public class MyEstItemCopy
    {
        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public EST_Item NewEstItem { get; set; }
        private readonly EST_Item _originalItem;

        public MyEstItemCopy(long originalItemID)
        {
            _originalItem = _db.EST_Item.Find(originalItemID);
        }

        public void Copy()
        {
            try
            {
                NewEstItem = new EST_Item();
                MyCommon.MyReflection.Copy(_originalItem, NewEstItem);
                //NewEstItem.EstItemID = 0;
                NewEstItem.IsHide = false;
                NewEstItem.EstRevID = GetNewEstRevID();
                NewEstItem.SerialID = GetNewSerialID();
                NewEstItem.EstItemNo = GetNewItemNumber();
                NewEstItem.Version = GetNewVersion();
                NewEstItem.ItemOption = GetNewOption();
                NewEstItem.EstPart  = GetNewPartNumber();
                NewEstItem.StatusID = GetNewStatus();
                NewEstItem.ItemPurposeID = GetNewPurposeID();
                NewEstItem.DirectionID = GetNewDirectionID();
                NewEstItem.ProductName = GetNewProductName();
                _db.EST_Item.Add(NewEstItem);
                _db.SaveChanges();

                CopySize();
                CreateNewSpecialFields();
                CopyDrawings();
                CopyCosts();
            }

            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
                throw;
            }
        }
        
        #region ************************ Custom fields for Inherited Class ******************************

        public virtual int GetNewEstRevID()
        {
            return _originalItem.EstRevID;
        }

        public virtual int GetNewSerialID()
        {
            return _originalItem.SerialID;
        }

        public virtual int GetNewPartNumber()
        {
            return _originalItem.EstPart;
        }

        public virtual short GetNewOption()
        {
            return _originalItem.ItemOption;
        }


        public virtual short GetNewItemNumber()
        {
            return _originalItem.EstItemNo;
        }
        public virtual int GetNewVersion()
        {
            return _originalItem.Version;
        }

        public virtual int  GetNewStatus()
        {
            return _originalItem.StatusID;
        }

        public virtual int GetNewPurposeID()
        {
            return _originalItem.ItemPurposeID;
        }

        public virtual int GetNewDirectionID()
        {
            return _originalItem.DirectionID;
        }

        public virtual string GetNewProductName()
        {
            return _originalItem.ProductName ;
        }

        #endregion

        #region ********************** Children ***************************

        private void CopySize()
        {
            var objs = _db.EST_Item_Specification_Size.Where(x => x.EstItemID == _originalItem.EstItemID).ToList();
            foreach (var obj in objs)
            {
                var newObj = new EST_Item_Specification_Size();
                MyReflection.Copy(obj, newObj);
                newObj.EstItemSizeID = 0;
                newObj.EstItemID = NewEstItem.EstItemID;
                _db.EST_Item_Specification_Size.Add(newObj);
            }
            _db.SaveChanges();
        }

        private void CreateNewSpecialFields()
        {
            var objs = _db.EST_Item_Specification.Where(x => x.EstItemID == _originalItem.EstItemID).ToList();
            foreach (var obj in objs)
            {
                var newObj = new EST_Item_Specification();
                MyReflection.Copy(obj, newObj);
                newObj.EstItemSpecificationID  = 0;
                newObj.EstItemID = NewEstItem.EstItemID;
                _db.EST_Item_Specification.Add(newObj);
            }
            _db.SaveChanges();

        }

        private void CopyDrawings()
        {
            var objs = _db.EST_Item_Drawing.Where(x => x.ParentID == _originalItem.EstItemID).ToList();
            foreach (var obj in objs)
            {
                var newObj = new EST_Item_Drawing();
                MyReflection.Copy(obj, newObj);
                newObj.DrawingID  = 0;
                newObj.ParentID  = NewEstItem.EstItemID;
                _db.EST_Item_Drawing.Add(newObj);
            }
            _db.SaveChanges();
        }

        private void CopyCosts()
        {
            var originalCosts = _db.EST_Cost.Where(x => x.EstItemID == _originalItem.EstItemID).ToList();
            foreach (var originalCost in originalCosts)
            {
                var destinationCost = new EST_Cost();
                MyCommon.MyReflection.Copy(originalCost, destinationCost);
                destinationCost.EstItemID = NewEstItem.EstItemID;
                _db.EST_Cost.Add(destinationCost);
            }

            _db.SaveChanges();

        }
        #endregion
    }

    /// <summary>
    /// 1. Original: 1) Status= Submited 
    ///              2) Purpose=Backup
    /// 2. Copy
    ///  3. New :  1) Purpose=Estimation
    ///            2) Direction=Response
    /// </summary>
    public class MyEstItemCopyForEstimation : MyEstItemCopy
    {
        public MyEstItemCopyForEstimation(long originalItemID):base(originalItemID)
        {
        }

        public override int GetNewPurposeID()
        {
            return (short) NEstItemPurpose.ForEstimation;
        }
    }

    public class MyEstItemCopyToNewItem : MyEstItemCopy
    {
        public MyEstItemCopyToNewItem(long originalItemID):base(originalItemID)
        {
        }

        public override int GetNewSerialID()
        {
            return  EstItemCommon.GetNewSerialID(NewEstItem.EstRevID);
        }

        public override  short GetNewItemNumber()
        {
            return (short) EstItemCommon.GetNewNumber(NewEstItem.EstRevID);
        }

        public override  int GetNewVersion()
        {
            return 0;
        }

        public override  short GetNewOption()
        {
            return 1;
        }

        public override int GetNewPartNumber()
        {
            return 1;
        }
    }

    public class MyEstItemToNewOption : MyEstItemCopy
    {
        public MyEstItemToNewOption(long originalItemID)
            : base(originalItemID)
        {
        }
        
        public override short GetNewOption()
        {
            return (short) EstItemCommon.GetNewOption(NewEstItem.EstRevID, NewEstItem.EstItemNo);
        }




    }

    public class MyEstItemToNewPart : MyEstItemCopy
    {
        public MyEstItemToNewPart(long originalItemID)
            : base(originalItemID)
        {
        }


        public override int GetNewPartNumber()
        {
            return (short)EstItemCommon.GetNewPartNumber(NewEstItem.EstRevID, NewEstItem.SerialID);
        }

    }

    public class MyEstItemCopyToAnotherProject : MyEstItemCopy
    {
        private readonly int _destinationEstRevID;

        public MyEstItemCopyToAnotherProject(long originalItemID, int destinationEstRevID)
            : base(originalItemID)
        {
            _destinationEstRevID = destinationEstRevID;
        }
        
        public override  int GetNewVersion()
        {
            return 0;
        }

        public override int GetNewEstRevID()
        {
            return _destinationEstRevID;
        }

        public override int GetNewStatus()
        {
            return (int)NEstItemStatus.New;
        }

        public override int GetNewPurposeID()
        {
            return (int)NEstItemPurpose.ForEstimation; 
        }
 
    }

    public class MyEstItemCopyToBackup : MyEstItemCopy
    {
        public MyEstItemCopyToBackup(long originalItemID)
            : base(originalItemID)
        {
        }

        public override int GetNewPurposeID()
        {
            return (short)NEstItemPurpose.ForBackup ;
        }

        public override string GetNewProductName()
        {
            var s = "Copy of" +
                    " - SN" + NewEstItem.EstItemID.ToString("") + 
                    " - " + NewEstItem.ProductName +
                    " - Ver" + NewEstItem.Version.ToString("");
            return s;
        }
       
    }


}