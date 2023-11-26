namespace nested_sets.common;

/*
 * a -> b -> c
 *        -> d
 *   -> e -> f
 *
 * [ (1,a,12), (2,b,7), (3,c,4), (5,d,6), (8,e,11), (9,f,10) ]
 *
 */


/// <summary>
/// Represents a node in a tree
/// </summary>
public class TreeNode {
    public int Id { get; set; }
    public IEnumerable<TreeNode> Children { get; set; } = Enumerable.Empty<TreeNode>();
}


/// <summary>
/// Represents a node in a nested set
/// </summary>
public struct SetNode {
    public int Id { get; set; }
    public int Left { get; set; }
    public int Right { get; set; }
}

/// <summary>
/// Enumerator for a nested set
/// </summary>
/// <remarks>
/// Allow multiple consumers to enumerate the same nested set
/// </remarks>
public class NestedSetEnumerator {

    private readonly SetNode[] elements;
    private readonly int numberOfElements;
    private int idx = 0;


    private IEnumerable<int> GetPath(int id) {

        yield return elements[idx].Id;

        // last element in the array
        if (idx == numberOfElements - 1) {
            yield break;
        }

        var root = elements[idx].Right;
        idx++;

        while (elements[idx].Left < root && idx < numberOfElements) {
            yield return elements[idx].Id;
            idx++;
        }
    }

    public NestedSetEnumerator
            ( SetNode[] elements
            , int numberOfElements) {

        this.elements = elements;
        this.numberOfElements = numberOfElements;
    }

    /// <summary>
    /// Move to the next entry in the tree
    /// </summary>
    public int Value => elements[idx].Id;

    /// <summary>
    /// Gets the current tree, all elements reachable from the current node
    /// </summary>
    /// <remarks>
    /// Updates the enumerator to point to the node
    /// that is next in the tree ( next sibling,
    /// parent or terminated )
    ///
    /// i.e. the enmuerator (next,value methods) have skipped the
    /// the nodes in the current tree
    /// </remarks>
    public IEnumerable<int> CurrentTree => GetPath(elements[idx].Id);

    /// <summary>
    /// Move to the next entry in the tree
    /// </summary>
    public void Next() => idx++;

    /// <summary>
    /// Identifies if the current node is the last node in the tree
    /// </summary>
    public bool Eot => idx >= numberOfElements;

}



/// <summary>
/// Enumerable respresentation of a tree as a nested set
/// </summary>
public class NestedSet {

    private readonly SetNode[] elements;
    private readonly int numberOfElements;
    private int idx = 0;

    // need to deal with the case where the tree is empty

    public NestedSet
            ( SetNode[] elements
            , int numberOfElements) {

        this.elements = elements;
        this.numberOfElements = numberOfElements;
    }


    public NestedSetEnumerator GetEnumerator() {

        return new NestedSetEnumerator(
            elements,
            numberOfElements
        );
    }


    public (int[], int) State() {
        var ms = new List<int>();
        var idx = 0;

        while(idx < numberOfElements) {
            ms.Add(elements[idx].Id);
            idx++;
        }

        return (ms.ToArray(), numberOfElements);
    }
}

public class NestedSetBuilder {

    /// <summary>
    /// Builds a nested set from a tree
    /// </summary>
    /// <param name="tree">The tree to build the nested set from</param>
    /// <returns>The nested set</returns>
    public NestedSet Build(TreeNode root)
    {
        var engine = new Engine();
        return engine.Build(root);
    }


    class Engine {
        private int arrayIdx;
        private int traversalPosition;
        private SetNode[] nestedSet;

        private void PopulateArray(TreeNode root) {

            // Ensure the array is large enough to hold the tree
            if( nestedSet.Length == arrayIdx) {
                Array.Resize(ref nestedSet, nestedSet.Length * 2);
            }

            // cache the current nodes index position
            var nodeIdx = arrayIdx;
            arrayIdx++;

            // add the current node to the array
            nestedSet[nodeIdx] = new SetNode() {
                Id = root.Id,
                Left = traversalPosition++,
            };

            // Add child nodes to the array
            foreach (var child in root.Children) {
                PopulateArray(child);

                Console.WriteLine($"ID: {child.Id}");
            }

            // now the childern have been added the right value can be set
            nestedSet[nodeIdx].Right = traversalPosition++;
        }

        /// <summary>
        /// Builds a nested set from a tree
        /// </summary>
        /// <param name="tree">The tree to build the nested set from</param>
        /// <returns>The nested set</returns>
        public NestedSet Build(TreeNode root) {

            arrayIdx = 0;
            traversalPosition = 1;
            nestedSet = new SetNode[180000];

            PopulateArray(root);

            Console.WriteLine($"Number of elements: {arrayIdx}");

            var idx = 0;

            while(idx<arrayIdx) {
                Console.WriteLine($"Id: {nestedSet[idx].Id}");
                idx++;
            }

            return new NestedSet(
                nestedSet,
                arrayIdx
            );
        }
    }
}

