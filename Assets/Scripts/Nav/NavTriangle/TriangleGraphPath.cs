

using Pika.Base.Mathj.Geometry;


/**
 * 路径点
 * @author jsjolund
 */
public class TriangleGraphPath : DefaultGraphPath<Connection<Triangle>> {
	/**
	 * The start point when generating a point path for this triangle path
	 */
	public Vector3 start;
	/**
	 * The end point when generating a point path for this triangle path
	 */
	public Vector3 end;
	/**
	 * If the triangle path is empty, the point path will span this triangle
	 */
	public Triangle startTri;

	/**
	 * @return Last triangle in the path.
	 */
	public Triangle getEndTriangle() {
		return (getCount() > 0) ? get(getCount() - 1).getToNode() : startTri;
	}
}
