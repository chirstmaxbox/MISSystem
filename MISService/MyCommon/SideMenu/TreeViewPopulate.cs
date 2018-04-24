using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace MyCommon.SideMenu
{
    public abstract class TreeViewPopulate
    {
        public IEnumerable<SideTreeContent> Parents { get; set; }
        public IEnumerable<SideTreeContent> Children { get; set; }

        public void PopulateTreeViewNodes(TreeView treeView)
        {
            treeView.Nodes.Clear();

            Parents = GetParents();

            if (Parents == null) return;
            foreach (SideTreeContent parent in Parents)
            {
                var masterNode = new TreeNode
                                     {
                                         Text = parent.Text,
                                         Value =Convert.ToString(parent.Value),
                                         NavigateUrl = parent.Url,
                                         Target = parent.Target
                                     };


                treeView.Nodes.Add(masterNode);
                //if there are children

                Children = GetChildren(parent.Value);
                if (Children == null) continue;

                foreach (SideTreeContent child in Children)
                {
                    var childNode = new TreeNode
                                        {
                                            Text = child.Text,
                                            Value =Convert.ToString(child.Value),
                                            NavigateUrl = child.Url,
                                            Target = child.Target
                                        };


                    masterNode.ChildNodes.Add(childNode);
                }
                masterNode.Expand();
            }
        }

        public abstract IEnumerable<SideTreeContent> GetParents();
        public abstract IEnumerable<SideTreeContent> GetChildren(int parentValue);
    }
}