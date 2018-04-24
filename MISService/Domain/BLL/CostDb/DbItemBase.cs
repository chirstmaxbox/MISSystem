namespace SpecDomain.BLL.CostDb
{
    public class DbItemBase
    {
        public int DisplayNumber { get; set; }

        public int DbItemID { get; set; }

        public string Name { get; set; }
        public string Unit { get; set; }
        public double UnitPrice { get; set; }
        public string UnitPriceString { get; set; }

        public int EstCostTypeID { get; set; }

        public int PositionID { get; set; }
    }
}