namespace PermitDomain.BLL
{
	public enum NPermitStatus
	{
		New = 0,
		Submit = 621,
		Revised = 631,
		
		Working = 641,
		Problem = 651,
		
		Canceled = 661,
	
		Completed = 691

	}

    public enum NPermitRequirment
    {
        Choose = 0,
        SignPermit = 10,
        StakeOut = 20,
        HoistingPermit = 30,
        SignVariance = 40,
    }


}
