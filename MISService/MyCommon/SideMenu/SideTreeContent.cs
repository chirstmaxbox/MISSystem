namespace MyCommon.SideMenu
{
    public class SideTreeContent
    {
        public int PageID { get; set; }
        public int Value { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
    }

    public class SideTreeContentChild : SideTreeContent

    {
        public int ParentValue { get; set; }
    }
}