


/**A*算法索引图 
 * <br>
 * A graph for the {@link IndexedAStarPathFinder}.
 * 
 * @param <N> Type of node
 * 
 * @author davebaol */
public interface IndexedGraph<N> : Graph<N>
{

    /** Returns the unique index of the given node.
	 * @param node the node whose index will be returned
	 * @return the unique index of the given node. */
    int getIndex(N node);

    /** Returns the number of nodes in this graph. */
    int getNodeCount();

}
