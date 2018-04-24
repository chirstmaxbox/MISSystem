using System.Collections.Generic;

namespace MyCommon.SideMenu
{
    public class TreeViewPopulateNull : TreeViewPopulate
    {
        public override IEnumerable<SideTreeContent> GetParents()
        {
            return null;
        }

        public override IEnumerable<SideTreeContent> GetChildren(int parentValue)
        {
            return null;
        }
    }
}