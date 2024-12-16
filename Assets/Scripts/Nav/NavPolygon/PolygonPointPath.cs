



using Pika.Ai.Nav.NavPolygon;
using Pika.Base.Mathj;
using Pika.Base.Mathj.Geometry;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/**
 *  NavMesh 生成坐标路径点
 * @author JiangZhiYong
 * @date 2018年2月20日 
 * @mail 359135103@qq.com
 */
public class PolygonPointPath : IEnumerable<Vector3>
{
    public static Vector3 V3_UP = Vector3.Y;
    public static Vector3 V3_DOWN = new Vector3(V3_UP).scl(-1);

    /**
	 * 在多边形边上的交叉点 <br>
	 * A point where an edge on the navmesh is crossed.
	 */
    private class EdgePoint
    {
        /**
		 * Polygon which must be crossed to reach the next path point.
		 */
        public Polygon toNode;
        /**
		 * Polygon which was crossed to reach this point.
		 */
        public Polygon fromNode;
        /**
		 * Path edges connected to this point. Can be used for spline generation at some
		 * point perhaps...
		 */
        public List<PolygonEdge> connectingEdges = new List<PolygonEdge>();
        /**
		 * The point where the path crosses an edge.
		 */
        public Vector3 point;

        public EdgePoint(Vector3 point, Polygon toNode)
        {
            this.point = point;
            this.toNode = toNode;
        }
    }

    /**
	 * @ 漏斗算法 <br>
	 * http://blog.csdn.net/yxriyin/article/details/39207709 <br>
	 * Plane funnel for the Simple Stupid Funnel Algorithm
	 */
    private class Funnel
    {

        public Plane leftPlane = new Plane(); // 左平面，高度为y轴
        public Plane rightPlane = new Plane();
        public Vector3 leftPortal = new Vector3(); // 路径左顶点，
        public Vector3 rightPortal = new Vector3(); // 路径右顶点
        public Vector3 pivot = new Vector3(); // 漏斗点，路径的起点或拐点

        /**
		 * 设置左平面
		 * 
		 * @param pivot
		 * @param leftEdgeVertex
		 *            边左顶点
		 */
        public void setLeftPlane(Vector3 pivot, Vector3 leftEdgeVertex)
        {
            Vector3 temp = new Vector3();
            leftPlane.set(pivot, temp.set(pivot).add(V3_UP), leftEdgeVertex);
            leftPortal.set(leftEdgeVertex);
        }

        /**
		 * 设置右平面
		 * 
		 * @param pivot
		 * @param rightEdgeVertex
		 */
        public void setRightPlane(Vector3 pivot, Vector3 rightEdgeVertex)
        {
            Vector3 temp = new Vector3();
            rightPlane.set(pivot, temp.set(pivot).add(V3_UP), rightEdgeVertex); // 高度
            rightPlane.normal.scl(-1); // 平面方向取反
            rightPlane.d = -rightPlane.d;
            rightPortal.set(rightEdgeVertex);
        }

        /**
		 * 设置平面
		 * 
		 * @param pivot
		 * @param edge
		 *            边
		 */
        public void setPlanes(Vector3 pivot, PolygonEdge edge)
        {
            setLeftPlane(pivot, edge.leftVertex);
            setRightPlane(pivot, edge.rightVertex);
        }

        /**
		 * 测试点在左平面内侧还是外侧（前或后）
		 * 
		 * @param point
		 * @return
		 */
        public Plane.PlaneSide sideLeftPlane(Vector3 point)
        {
            return leftPlane.testPoint(point);
        }

        /**
		 * 测试点在右平面内侧还是外侧
		 * 
		 * @param point
		 * @return
		 */
        public Plane.PlaneSide sideRightPlane(Vector3 point)
        {
            return rightPlane.testPoint(point);
        }
    }

    private Plane crossingPlane = new Plane(); // 横跨平面
    private Vector3 tmp1 = new Vector3();
    private Vector3 tmp2 = new Vector3();
    private List<Connection<Polygon>> nodes; // 路径连接点
    private Vector3 start; // 起点
    private Vector3 end; // 终点
    private Polygon startPolygon; // 起始三角形
    private EdgePoint lastPointAdded; // 最后一个边点
    private List<Vector3> vectors = new List<Vector3>(); // 路径坐标点
    private List<EdgePoint> pathPoints = new List<EdgePoint>();
    private PolygonEdge lastEdge; // 最后一个边



    private PolygonEdge getEdge(int index)
    {
        return (PolygonEdge)((index == nodes.Count()) ? lastEdge : nodes.ElementAt(index));
    }

    private int numEdges()
    {
        return nodes.Count() + 1;
    }

    /**
	 * 计算路径点 <br>
	 * Calculate the shortest path through the navigation mesh polygons.
	 *
	 * @param polygonlePath
	 * @param calculateCrossPoint true 计算与多边形边的交叉点，3D高度计算需要，false 只计算拐点
	 */
    public void calculateForGraphPath(PolygonGraphPath polygonlePath, bool calculateCrossPoint)
    {
        clear();
        nodes = polygonlePath.nodes;
        this.start = new Vector3(polygonlePath.start);
        this.end = new Vector3(polygonlePath.end);
        this.startPolygon = polygonlePath.startPolygon;

        //		// 矫正开始坐标
        //		// Check that the start point is actually inside the start triangle, if not,
        //		// project it to the closest
        //		// triangle edge. Otherwise the funnel calculation might generate spurious path
        //		// segments.
        //		Ray ray = new Ray(tmp1.set(V3_UP).scl(1000).add(start), tmp2.set(V3_DOWN)); // 起始坐标从上向下的射线
        //		if (!Intersector.intersectRayTriangle(ray, startPolygon.a, startPolygon.b, startPolygon.c, null)) {
        //			float minDst = float.PositiveInfinity;
        //			Vector3 projection = new Vector3(); // 规划坐标
        //			Vector3 newStart = new Vector3(); // 新坐标
        //			float dst;
        //			// A-B
        //			if ((dst = GeometryUtil.nearestSegmentPointSquareDistance(projection, startPolygon.a, startPolygon.b,
        //					start)) < minDst) {
        //				minDst = dst;
        //				newStart.set(projection);
        //			}
        //			// B-C
        //			if ((dst = GeometryUtil.nearestSegmentPointSquareDistance(projection, startPolygon.b, startPolygon.c,
        //					start)) < minDst) {
        //				minDst = dst;
        //				newStart.set(projection);
        //			}
        //			// C-A
        //			if ((dst = GeometryUtil.nearestSegmentPointSquareDistance(projection, startPolygon.c, startPolygon.a,
        //					start)) < minDst) {
        //				minDst = dst;
        //				newStart.set(projection);
        //			}
        //			start.set(newStart);
        //		}
        if (nodes.Count() == 0)
        { // 起点终点在同一三角形中
            addPoint(start, startPolygon);
            addPoint(end, startPolygon);
        }
        else
        {
            lastEdge = new PolygonEdge(nodes.ElementAt(nodes.Count() - 1).getToNode(), nodes.ElementAt(nodes.Count() - 1).getToNode(), end,
                    end);
            calculateEdgePoints(calculateCrossPoint);
        }
    }

    /**
	 * 清理数据 <br>
	 * Clear the stored path data.
	 */
    public void clear()
    {
        vectors.Clear();
        pathPoints.Clear();
        start = null;
        end = null;
        startPolygon = null;
        lastPointAdded = null;
        lastEdge = null;
    }

    /**
	 * @ 获取寻路向量点 A path point which crosses one or more edges in the navigation
	 * mesh.
	 *
	 * @param index
	 * @return
	 */
    public Vector3 getVector(int index)
    {
        return vectors.ElementAt(index);
    }

    /**
	 * The number of path points.
	 *
	 * @return
	 */
    public int getSize()
    {
        return vectors.Count();
    }

    /**
	 * All vectors in the path.
	 *
	 * @return
	 */
    public List<Vector3> getVectors()
    {
        return vectors;
    }

    /**
	 * The polygon which must be crossed to reach the next path point.
	 *
	 * @param index
	 * @return
	 */
    public Polygon getToPolygon(int index)
    {
        return pathPoints.ElementAt(index).toNode;
    }

    /**
	 * The polygon from which must be crossed to reach this point.
	 *
	 * @param index
	 * @return
	 */
    public Polygon getFromPolygon(int index)
    {
        return pathPoints.ElementAt(index).fromNode;
    }

    /**
	 * The navmesh edge(s) crossed at this path point.
	 *
	 * @param index
	 * @return
	 */
    public List<PolygonEdge> getCrossedEdges(int index)
    {
        return pathPoints.ElementAt(index).connectingEdges;
    }

    /**
	 * 添加坐标点
	 * 
	 * @param point
	 * @param toNode
	 */
    private void addPoint(Vector3 point, Polygon toNode)
    {
        addPoint(new EdgePoint(point, toNode));
    }

    /**
	 * 添加坐标点
	 * 
	 * @param edgePoint
	 */
    private void addPoint(EdgePoint edgePoint)
    {
        vectors.Add(edgePoint.point);
        pathPoints.Add(edgePoint);
        lastPointAdded = edgePoint;
    }

    /**
	 * 根据多边形路径计算最短路径坐标点 <br>
	 * <p>
	 * 1.下一组边与上一个路径点的向量全部在前一组边的范围内，直接把下一组边的两点替换前一组边的两点。<br>
	 * 2.下一组边的右边点与上一个路径点向量在上一组边的范围内，但左边的点不在范围内，把下一组边的右边点替换前一组边的右边点<br>
	 * 3.下一组边的左边点与上一个路径点向量在上一组边的范围内，但右边的点不在范围内，把下一组边的左边点替换前一组边的左边点<br>
	 * 4.下一组边两个点组成的向量都在上一组边的左边，那么上一组边的左边点成为拐点<br>
	 * 5.下一组边两个点组成的向量都在上一组边的右边，那么上一组边的右边点成为拐点<br>
	 * 6.假如下一组边左边的点和上一组边的左边点重合，则把上一组边的左边点成为拐点<br>
	 * 7.假如下一组边右边的点和上一组边的右边点重合，则把上一组边的右边点成为拐点<br>
	 * 8.当寻路达到最后一个多边形，直接判断终点和上一个路径点的向量是否在上一个边两点的中间，假如不是，再增加一个拐点<br>
	 * </p>
	 * http://blog.csdn.net/yxriyin/article/details/39207709 Calculate the shortest
	 * point path through the path triangles, using the Simple Stupid Funnel
	 * Algorithm.
	 *
	 * @return
	 */
    private void calculateEdgePoints(bool calculateCrossPoint)
    {
        PolygonEdge edge = getEdge(0);
        addPoint(start, edge.fromNode);
        lastPointAdded.fromNode = edge.fromNode;

        Funnel funnel = new Funnel();
        funnel.pivot.set(start); // 起点为漏斗点
        funnel.setPlanes(funnel.pivot, edge); // 设置第一对平面

        int leftIndex = 0; // 左顶点索引
        int rightIndex = 0; // 右顶点索引
        int lastRestart = 0;

        for (int i = 1; i < numEdges(); ++i)
        {
            edge = getEdge(i); // 下一条边

            Plane.PlaneSide leftPlaneLeftDP = funnel.sideLeftPlane(edge.leftVertex);
            Plane.PlaneSide leftPlaneRightDP = funnel.sideLeftPlane(edge.rightVertex);
            Plane.PlaneSide rightPlaneLeftDP = funnel.sideRightPlane(edge.leftVertex);
            Plane.PlaneSide rightPlaneRightDP = funnel.sideRightPlane(edge.rightVertex);

            // 右顶点在右平面里面
            if (rightPlaneRightDP != Plane.PlaneSide.Front)
            {
                // 右顶点在左平面里面
                if (leftPlaneRightDP != Plane.PlaneSide.Front)
                {
                    // Tighten the funnel. 缩小漏斗
                    funnel.setRightPlane(funnel.pivot, edge.rightVertex);
                    rightIndex = i;
                }
                else
                {
                    // Right over left, insert left to path and restart scan from portal left point.
                    // 右顶点在左平面外面，设置左顶点为漏斗顶点和路径点，从新已该漏斗开始扫描
                    if (calculateCrossPoint)
                    {
                        calculateEdgeCrossings(lastRestart, leftIndex, funnel.pivot, funnel.leftPortal);
                    }
                    else
                    {
                        vectors.Add(new Vector3(funnel.leftPortal));
                    }
                    funnel.pivot.set(funnel.leftPortal);
                    i = leftIndex;
                    rightIndex = i;
                    if (i < numEdges() - 1)
                    {
                        lastRestart = i;
                        funnel.setPlanes(funnel.pivot, getEdge(i + 1));
                        continue;
                    }
                    break;
                }
            }
            // 左顶点在左平面里面
            if (leftPlaneLeftDP != Plane.PlaneSide.Front)
            {
                // 左顶点在右平面里面
                if (rightPlaneLeftDP != Plane.PlaneSide.Front)
                {
                    // Tighten the funnel.
                    funnel.setLeftPlane(funnel.pivot, edge.leftVertex);
                    leftIndex = i;
                }
                else
                {
                    // Left over right, insert right to path and restart scan from portal right
                    // point.
                    if (calculateCrossPoint)
                    {
                        calculateEdgeCrossings(lastRestart, rightIndex, funnel.pivot, funnel.rightPortal);
                    }
                    else
                    {
                        vectors.Add(new Vector3(funnel.rightPortal));
                    }
                    funnel.pivot.set(funnel.rightPortal);
                    i = rightIndex;
                    leftIndex = i;
                    if (i < numEdges() - 1)
                    {
                        lastRestart = i;
                        funnel.setPlanes(funnel.pivot, getEdge(i + 1));
                        continue;
                    }
                    break;
                }
            }
        }
        if (calculateCrossPoint)
        {
            calculateEdgeCrossings(lastRestart, numEdges() - 1, funnel.pivot, end);
        }
        else
        {
            vectors.Add(new Vector3(end));
        }

        for (int i = 1; i < pathPoints.Count(); i++)
        {
            EdgePoint p = pathPoints.ElementAt(i);
            p.fromNode = pathPoints.ElementAt(i - 1).toNode;
        }
        return;
    }

    /**
	 * 计算存储和边的交叉点<br>
	 * Store all edge crossing points between the start and end indices. If the path
	 * crosses exactly the start or end points (which is quite likely), store the
	 * edges in order of crossing in the EdgePoint data structure.
	 * <p/>
	 * Edge crossings are calculated as intersections with the plane from the start,
	 * end and up vectors.
	 *
	 * @param startIndex
	 * @param endIndex
	 * @param startPoint 
	 * @param endPoint
	 */
    private void calculateEdgeCrossings(int startIndex, int endIndex, Vector3 startPoint, Vector3 endPoint)
    {

        if (startIndex >= numEdges() || endIndex >= numEdges())
        {
            return;
        }
        crossingPlane.set(startPoint, tmp1.set(startPoint).add(V3_UP), endPoint);

        EdgePoint previousLast = lastPointAdded;

        PolygonEdge edge = getEdge(endIndex);
        EdgePoint end = new EdgePoint(new Vector3(endPoint), edge.toNode);

        for (int i = startIndex; i < endIndex; i++)
        {
            edge = getEdge(i);

            if (edge.rightVertex.Equals(startPoint) || edge.leftVertex.Equals(startPoint))
            {
                previousLast.toNode = edge.toNode;
                if (!previousLast.connectingEdges.Contains(edge))
                {
                    previousLast.connectingEdges.Add(edge);
                }

            }
            else if (edge.leftVertex.Equals(endPoint) || edge.rightVertex.Equals(endPoint))
            {
                if (!end.connectingEdges.Contains(edge))
                {
                    end.connectingEdges.Add(edge);
                }

            }
            else if (Intersector.intersectSegmentPlane(edge.leftVertex, edge.rightVertex, crossingPlane, tmp1)
                    && !float.IsNaN(tmp1.x + tmp1.y + tmp1.z))
            {
                if (i != startIndex || i == 0)
                {
                    lastPointAdded.toNode = edge.fromNode;
                    EdgePoint crossing = new EdgePoint(new Vector3(tmp1), edge.toNode);
                    crossing.connectingEdges.Add(edge);
                    addPoint(crossing);
                }
            }
        }
        if (endIndex < numEdges() - 1)
        {
            end.connectingEdges.Add(getEdge(endIndex));
        }
        if (!lastPointAdded.Equals(end))
        {
            addPoint(end);
        }
    }

    public IEnumerator<Vector3> GetEnumerator()
    {
        return vectors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
