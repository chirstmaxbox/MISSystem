using System.Collections.Generic;

namespace MyCommon.SideMenu
{
    public interface IChildMenu
    {
        IEnumerable<SideTreeContentChild> Items { get; }
    }
}