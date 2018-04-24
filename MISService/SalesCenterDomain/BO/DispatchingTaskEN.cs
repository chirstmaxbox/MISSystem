namespace SalesCenterDomain.BO
{
    public class DispatchingTaskEN
    {
        #region NDispatchingCategroyAndPurposeStage enum

        public enum NDispatchingCategroyAndPurposeStage : byte
        {
            All = 0,
            EstimationDrawingRequest = 1,
            WorkorderDrawingRequest = 2,

            EstimationDrawingAttachment = 3,
            WorkorderDrawingAttachment = 4,
            InternalRecord = 5
        }

        #endregion

        #region NDispatchingSubmitTo enum

        public enum NDispatchingSubmitTo
        {
            Choose = 0,
            Artist = 501,
            Estimator = 201,
            EngineerTechnician = 551,
            WorkorderApprover = 720,
        }

        #endregion

        #region NDispatchingTaskPurpose enum

        public enum NDispatchingTaskPurpose : byte
        {
            Choose = 0,
            Estimation = 1,
            PermitDrawing = 2,
            WorkorderDrawing = 3,
            ConceptDesign = 4,
            WorkorderApproval = 5
        }

        #endregion

        #region NDocumentSelectType enum

        public enum NDocumentSelectType
        {
            //? 
            estItemDrawingID = 1,

            estItemID = 2,
            woItemID = 2,

            estRevID = 3
        }

        #endregion

        #region NDocumentType enum

        public enum NDocumentType : byte
        {
            Project = 1,

            EstimationItem = 2,

            WorkorderItem = 3
        }

        #endregion

        #region NResponseTaskCategory enum

        public enum NResponseTaskCategory
        {
            ApprovalWorkorder = 720,
            Estimation = 201,
            GraphicDrawing = 501,
            //art
            StructureDrawing = 551,
            TaskOfInternalRecord = 5001
        }

        #endregion
    }
}