using System;
using SalesCenterDomain.BO;

//Operation:
//   Insert
//   Select
//   Update
//   Delete
//   Copy


//Contents:
//   Title
//   ---Document
//   ---Item
//   -------Item Drawing
//   -------Item Component
//   ---Service

namespace SalesCenterDomain.BDL
{

    #region "Base Title"

    public abstract class BaseObjCopy
    {
        private readonly int _myParentID;

        private readonly int _newParentID;
        private int _myID;
        //
        private int _newID;


        public BaseObjCopy(int myParentID, int newParentID, int myID)
        {
            _myParentID = myParentID;
            _newParentID = newParentID;
            _myID = myID;
        }

        public int MyID
        {
            set { _myID = value; }
        }

        public int NewID
        {
            get { return _newID; }
        }

        public void Copy()
        {
            //Parameter=0 if not applicable
            _newID = CopyTitle(_myParentID, _newParentID, _myID);
            CopyTitleRelatedDocuments(_myParentID, _newParentID, _myID, _newID);
            CopyServices(_myParentID, _newParentID, _myID, _newID);
            CopyItems(_myParentID, _newParentID, _myID, _newID);
            CopyNotes(_myParentID, _newParentID, _myID, _newID);
        }

        #region "Copy"

        public virtual int CopyTitle(int myParentID, int newParentID, int myID)
        {
            //return Newly inserted Recorded ID
            return 0;
        }


        public virtual void CopyTitleRelatedDocuments(int myParentID, int newParentID, int myID, int newID)
        {
        }


        public virtual void CopyItems(int myParentID, int newParentID, int myID, int newID)
        {
        }


        public virtual void CopyNotes(int myParentID, int newParentID, int myID, int newID)
        {
        }


        public virtual void CopyServices(int myParentID, int newParentID, int myID, int newID)
        {
        }

        #endregion
    }

    public abstract class BaseObjDelete
    {
        public int DeleteResult { get; set; }

        public void Delete()
        {
            DeleteResult = GetDeleteValidationResult();
            if (DeleteResult == (int) NValidationErrorValue.OK)
            {
                ReleaseNumber();
                DeleteDocuments();
                DeleteItems();
                DeleteServices();
                DeleteNotes();
                DeleteTitle();
            }
        }

        #region "Delete"

        public virtual int GetDeleteValidationResult()
        {
            return Convert.ToInt32(NValidationErrorValue.OK);
        }

        public virtual void ReleaseNumber()
        {
        }

        public virtual void DeleteDocuments()
        {
        }


        public virtual void DeleteItems()
        {
        }


        public virtual void DeleteServices()
        {
        }


        public virtual void DeleteNotes()
        {
        }


        public virtual void DeleteTitle()
        {
        }

        #endregion
    }

    #endregion
}