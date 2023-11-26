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

        // assert
        using(new AssertionScope()) {
            enumerator.CurrentTree.Should().BeEquivalentTo(new int[] { 1, 2, 3, 4, 5, 6 });
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


        // assert
        using(new AssertionScope()) {
            enumerator.CurrentTree.Should().BeEquivalentTo(new int[] { 2, 3, 4 });
            enumerator.Eot.Should().BeFalse();
            enumerator.Value.Should().Be(5);
        }
    }
}
