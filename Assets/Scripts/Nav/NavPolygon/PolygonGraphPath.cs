


using Pika.Ai.Nav.NavPolygon;
using Pika.Base.Mathj.Geometry;


/**
 * 多边形路径点
 * 
 * @author JiangZhiYong
 * @date 2018年2月20日
 * @mail 359135103@qq.com
 */
public class PolygonGraphPath : DefaultGraphPath<Connection<Polygon>> {
    public Vector3 start;
    public Vector3 end;
    public Polygon startPolygon;

    public Polygon getEndPolygon() {
        return (getCount() > 0) ? get(getCount() - 1).getToNode() : startPolygon;
    }

}
