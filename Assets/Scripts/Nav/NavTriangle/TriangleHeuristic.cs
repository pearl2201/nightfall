


using Pika.Base.Mathj.Geometry;
using System;


/**navmesh 启发式消耗预估
 * <br>
 * @author jsjolund
 */
public class TriangleHeuristic : Heuristic<Triangle> {

	private static Vector3 A_AB = new Vector3();
	private static Vector3 A_BC = new Vector3();
	private static Vector3 A_CA = new Vector3();
	private static Vector3 B_AB = new Vector3();
	private static Vector3 B_BC = new Vector3();
	private static Vector3 B_CA = new Vector3();

	/**
	 * Estimates the distance between two triangles, by calculating the distance
	 * between their edge midpoints.
	 *
	 * @param node
	 * @param endNode
	 * @return
	 */

	public float estimate(Triangle node, Triangle endNode) {
		float dst2;
		float minDst2 = float.PositiveInfinity;
		A_AB.set(node.a).add(node.b).scl(0.5f);
		A_BC.set(node.b).add(node.c).scl(0.5f);
		A_CA.set(node.c).add(node.a).scl(0.5f);

		B_AB.set(endNode.a).add(endNode.b).scl(0.5f);
		B_BC.set(endNode.b).add(endNode.c).scl(0.5f);
		B_CA.set(endNode.c).add(endNode.a).scl(0.5f);

		if ((dst2 = A_AB.dst2(B_AB)) < minDst2)
			minDst2 = dst2;
		if ((dst2 = A_AB.dst2(B_BC)) < minDst2)
			minDst2 = dst2;
		if ((dst2 = A_AB.dst2(B_CA)) < minDst2)
			minDst2 = dst2;

		if ((dst2 = A_BC.dst2(B_AB)) < minDst2)
			minDst2 = dst2;
		if ((dst2 = A_BC.dst2(B_BC)) < minDst2)
			minDst2 = dst2;
		if ((dst2 = A_BC.dst2(B_CA)) < minDst2)
			minDst2 = dst2;

		if ((dst2 = A_CA.dst2(B_AB)) < minDst2)
			minDst2 = dst2;
		if ((dst2 = A_CA.dst2(B_BC)) < minDst2)
			minDst2 = dst2;
		if ((dst2 = A_CA.dst2(B_CA)) < minDst2)
			minDst2 = dst2;

		return (float) Math.Sqrt(minDst2);
	}

}
