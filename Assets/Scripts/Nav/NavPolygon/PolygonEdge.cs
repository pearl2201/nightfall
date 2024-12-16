

using Newtonsoft.Json;
using Pika.Ai.Nav.NavPolygon;
using Pika.Base.Mathj.Geometry;


/**
 * 多边形共享边
 * @author JiangZhiYong
 * @mail 359135103@qq.com
 */
public class PolygonEdge : Connection<Polygon>
{

    /** 右顶点 */
    public Vector3 rightVertex;
    public Vector3 leftVertex;

    /** 源多边形*/
    public Polygon fromNode;
    /** 指向的多边形 */
    public Polygon toNode;
    /**两多边形中心点间的距离*/
    private float cost;

    public PolygonEdge(Polygon fromNode, Polygon toNode, Vector3 rightVertex, Vector3 leftVertex)
    {
        this.fromNode = fromNode;
        this.toNode = toNode;
        this.rightVertex = rightVertex;
        this.leftVertex = leftVertex;
    }


    public float getCost()
    {
        if (cost == 0)
        {
            cost = fromNode.center.dst(toNode.center);
        }
        return cost;
    }


    public Polygon getFromNode()
    {
        return fromNode;
    }


    public Polygon getToNode()
    {
        return toNode;
    }

    
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }


}
