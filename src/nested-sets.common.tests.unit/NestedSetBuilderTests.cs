using FluentAssertions;
using FluentAssertions.Execution;
using nested_sets.common;


namespace nested_sets.common.tests.unit;

public class NestedSetBuilderTests
{
    // a -> b -> c
    //        -> d
    //   -> e -> f


    // will load the the array from the tree
    private TreeNode testData = new() {
        // a
        Id = 1,
        Children = new TreeNode[] {
            new() {
                // b
                Id = 2,
                Children = new TreeNode[] {
                    new() { Id = 3 }, // c
                    new() { Id = 4 }, // d
                },
            },
            new() {
                // e
                Id = 5,
                Children = new TreeNode[] {
                    new() { Id = 6 }, // f
                },
            },
        },
    };



    [Fact]
    public void Can_travers_the_full_path()
    {
        // arrange
        var sut = new NestedSetBuilder();


        // act
        var result = sut.Build(testData);
        var enumerator = result.GetEnumerator();

        var tree = new List<int>();
        enumerator.GetSubTree(tree.Add);


        // assert
        using(new AssertionScope()) {

            tree.Should().BeEquivalentTo(new int[] { 1, 2, 3, 4, 5, 6 });
            enumerator.Eot.Should().BeFalse();
            // It remains on the last node and you have to advance
            enumerator.Value.Should().Be(6);
            enumerator.Next();
            enumerator.Eot.Should().BeTrue();
        }
    }


    [Fact]
    public void Can_travers_the_full_path_using_next()
    {
        // arrange
        var sut = new NestedSetBuilder();


        // act
        var result = sut.Build(testData);
        var enumerator = result.GetEnumerator();

        var members = new List<int>();
        while(!enumerator.Eot) {

            members.Add(enumerator.Value);
            enumerator.Next();
        }

        // assert
        members.Should().BeEquivalentTo(new int[] { 1, 2, 3, 4, 5, 6 });
    }


    [Fact]
    public void Can_traverse_a_sub_tree()
    {
        // arrange
        var sut = new NestedSetBuilder();


        // act
        var result = sut.Build(testData);
        var enumerator = result.GetEnumerator();

        enumerator.Next(); // move to the first child (first subtree)

        var firstSubTree = new List<int>();
        enumerator.GetSubTree(firstSubTree.Add); // get the subtree

        // assert
        using(new AssertionScope()) {
            firstSubTree.Should().BeEquivalentTo(new int[] { 2, 3, 4 });
            enumerator.Next();
            enumerator.Value.Should().Be(5);
        }
    }


    [Fact]
    public void Can_traverse_multiple_sub_trees()
    {
        // arrange
        var sut = new NestedSetBuilder();


        // act
        var result = sut.Build(testData);
        var enumerator = result.GetEnumerator();

        var firstSubTree  = new List<int>();
        var secondSubTree = new List<int>();

        enumerator.Next(); // move to the first child (first subtree)
        enumerator.GetSubTree(firstSubTree.Add);

        enumerator.Next(); // move past then last node in the first subtree
        enumerator.GetSubTree(secondSubTree.Add);


        // assert
        using(new AssertionScope()) {
            firstSubTree.Should().BeEquivalentTo(new int[] { 2, 3, 4 });
            secondSubTree.Should().BeEquivalentTo(new int[] { 5, 6 });
        }
    }


    [Fact]
    public void Can_traverse_combining_next_and_current_tree()
    {
        // arrange
        var sut = new NestedSetBuilder();


        // act
        var result = sut.Build(testData);
        var enumerator = result.GetEnumerator();

        var nextNodes     = new List<int>();
        var firstSubTree  = new List<int>();
        var secondSubTree = new List<int>();

        while(!enumerator.Eot) {

            switch (enumerator.Value) {
                case 2 : enumerator.GetSubTree(firstSubTree.Add);  break;
                case 6 : enumerator.GetSubTree(secondSubTree.Add); break;
                default: nextNodes.Add(enumerator.Value);      break;
            };
            enumerator.Next();
        }


        // assert
        using(new AssertionScope()) {
            nextNodes.Should().BeEquivalentTo(new int[] { 1, 5 });
            firstSubTree.Should().BeEquivalentTo(new int[] { 2, 3, 4 });
            secondSubTree.Should().BeEquivalentTo(new int[] { 6 });
        }
    }

}
