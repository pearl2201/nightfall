

using Pika.Base.Mathj.Geometry;
using System.Text;


/**
 * 相连接三角形的共享边
 * 
 * @author JiangZhiYong
 * @QQ 359135103 2017年11月7日 下午4:50:11
 */
public class TriangleEdge : Connection<Triangle>
{
    /** 右顶点 */
    public Vector3 rightVertex;
    public Vector3 leftVertex;

    /** 源三角形 */
    public Triangle fromNode;
    /** 指向的三角形 */
    public Triangle toNode;

    public TriangleEdge(Vector3 rightVertex, Vector3 leftVertex) : this(null, null, rightVertex, leftVertex)
    {

    }

    public TriangleEdge(Triangle fromNode, Triangle toNode, Vector3 rightVertex, Vector3 leftVertex)
    {
        this.fromNode = fromNode;
        this.toNode = toNode;
        this.rightVertex = rightVertex;
        this.leftVertex = leftVertex;
    }


    public float getCost()
    {
        return 1;
    }


    public Triangle getFromNode()
    {
        return fromNode;
    }


    public Triangle getToNode()
    {
        return toNode;
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder("Edge{");
        sb.Append("fromNode=").Append(fromNode.index);
        sb.Append(", toNode=").Append(toNode == null ? "null" : toNode.index.ToString());
        sb.Append(", rightVertex=").Append(rightVertex);
        sb.Append(", leftVertex=").Append(leftVertex);
        sb.Append('}');
        return sb.ToString();
    }

}
