using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagerDomain.BLL
{
	//DB
	public enum NInterActionMessageCategoryID : int
	{
		System = 0,
		Marketing = 100,
		Sales = 200,
		Estimation = 400,
		GraphicDrawing = 500,
		StructureDrawing = 600,
		EngineerStamping = 700,
		Permit = 800,
		WorkorderApproval = 900,
		SiteCheck = 1000,
		Production = 1100,
		SpecialMaterialPurchasing = 1300,
		Purchasing = 1400,
		JobCosting = 1500,
		MaterialManagement = 1600,
        Subcontract = 1700,
		Installation = 1800,

		Accounting = 2000,
        PermitApplication = 2100
	}

	//DB
	public enum NInterActionMessageTypeID : int
	{
		NA=-1,
		System = 0,
		//100
		LeadQuotionRequestFromMarketing = 101,

		//200
		SalesNewLeadAssignedAE = 201,

		//900
		WorkorderApprovalNoContract=901,
		WorkorderApprovalProductionExceedContractAmount=902,
		//1100		Workorder
		WorkorderChangeDeadlineProductionApplyForChangeWorkorderDeadline = 1101,
		WorkorderChangeDeadlineSalesApprovedDeadlineChange = 1102,
		WorkorderChangeDeadlineSalesApplyForChangeWorkorderDeadline = 1103,
		WorkorderChangeDeadlineProductionApprovedDeadlineChange = 1104,

        WorkorderInstallationMetProblem = 1150,
        WorkorderInstallationFinished = 1160,

		//1900
		SubcontractNewSubcontractRequestFromSales = 1701,
		SubcontractSalesRevisedSubcontractRequest = 1703,
		SubcontractSalesRevisedDrawings = 1705,
		SubcontractOverBudgetApprovalRequired = 1714,
		SubcontractOverBudgetApplicationApproved = 1715,

		SubcontractJobhasMetProblemAttentionRequired = 1740,
		SubcontractSiteCheckJobFinishedReportAttached = 1745,

		SubcontractJobInstalledRatingRequired = 1746,
		SubcontractJobHasBeenCanceledAsperRequired = 1748,

		SubcontractJobHasBeenRated = 1755,
        SubcontractNewChatroomMessage=1760,

        PermitNewRequestFromSales = 2101,
        PermitNewCommunicationMessage = 2111,
        PermitJobStatusChanged = 2121,

	}


	public enum NInterActionMessagePriority : int
	{
		Normal = 0,
		Urgent=10
	}

	public enum NInterActionMessageStatus : int
	{
		New = 0,
		Postponed = 2,
		Dismissed = 100,
	}

	public enum NInterActionEmployeeMessageStatus : int
	{
		NoNewMessage = 0,
		ThereIsNewMessage = 1,
		ThterIsNewUrgentMessage=2,
	}

	public enum NInterActionHandlingItemType : int
	{
		Project=0,
		Lead=100,
		Workorder=500,
		Subcontract = 1700,
		PermitApplication=2100
	}

	

}
