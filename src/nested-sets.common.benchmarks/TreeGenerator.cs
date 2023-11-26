using nested_sets.common;


namespace nested_sets.common.benchmarks;

public class TreeGenerator
{
    private readonly int maxChilds;

    public TreeGenerator(int maxChilds)
    {
        this.maxChilds = maxChilds;
    }


    public TreeNode CreateFixedSizeTree(int depth, Func<int> valueGenerator)
    {
        var node     = new TreeNode();
        var children = new List<TreeNode>();

        node.Id = valueGenerator();
        if (depth > 0)
        {
            for (var i = 0; i < maxChilds; ++i)
                children.Add(CreateFixedSizeTree(depth - 1, valueGenerator));
            node.Children = children;
        }
        return node;
    }

}
