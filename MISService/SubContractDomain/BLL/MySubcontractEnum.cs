using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubContractDomain.BLL
{
	public enum NSubcontractRequestStatus
	{
		New = 0,
		Submit = 911,
		Revised = 913,

	}

	public enum NSubcontractReponseStatus
	{
		NewRequest = 920,
		Working = 925,
		MetProblem = 926,
		SignInstalled = 930,
		SiteChedkDone = 932,

		Canceled = 934,
		Rated = 937,

		SubcontractComplted = 950
	}
}
