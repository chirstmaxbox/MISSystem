using System.Collections.Generic;

namespace MyCommon.SideMenu
{
    public interface IParentMenu
    {
        IEnumerable<SideTreeContent> Items { get;  }
    }
}