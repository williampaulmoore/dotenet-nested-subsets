using BenchmarkDotNet.Attributes;

namespace nested_sets.common.benchmarks;

[MemoryDiagnoser(true)]
public class NestedSetBuilderBenchmarks
{
    private readonly Random rnd = new();

    // maxdepth = 8, 299,000 nodes roughly
    // maxdepth = 7, 137,000 nodes roughly
    // maxdepth = 6,  55,000 nodes roughly
    private readonly TreeGenerator treeGenerator = new(6);
    private TreeNode tree;
    private NestedSet ns;

    public NestedSetBuilderBenchmarks()
    {
        var sut = new NestedSetBuilder();
        tree = treeGenerator.CreateFixedSizeTree(5, () => rnd.Next());
        ns = sut.Build(tree);
    }


    [Benchmark]
    public NestedSet BuildNestedSet()
    {
        var sut = new NestedSetBuilder();
        return sut.Build(tree);
    }

    [Benchmark]
    public List<int> BuildNestedAndEnumerateTree()
    {
        var l = new List<int>();

        var sut = new NestedSetBuilder();
        var nestedSet = sut.Build(tree);
        var enumerator = nestedSet.GetEnumerator();


        while (!enumerator.Eot)
        {
            if(rnd.Next(0, 100) > 75) {
                enumerator.GetSubTree(l.Add);
            }
            enumerator.Next();
        }
        return l;
    }

    [Benchmark]
    public List<int> EnumerateTree()
    {
        var l = new List<int>();
        var e = ns.GetEnumerator();

        while(!e.Eot) {

            if(rnd.Next(0, 100) > 75) {
                e.GetSubTree(l.Add);
            }
            e.Next();
        }
        return l;
    }
}

