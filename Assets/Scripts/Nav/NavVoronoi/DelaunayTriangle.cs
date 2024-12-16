

using Pika.Base.Mathj.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;


/**
 * Delaunay 三角形
 * 
 * @author JiangZhiYong
 * @mail 359135103@qq.com
 */
public class DelaunayTriangle : Triangle
{

    /** 外接圆半径 */
    public float radius;
    /** 邻接三角形 */
    public List<DelaunayTriangle> neighborTriangles;

    public DelaunayTriangle(Vector3 a, Vector3 b, Vector3 c) : base(a, b, c, 0)
    {

        calculateCircleCenter();
    }

    /**
	 * 计算外接圆圆心和半径
	 */
    private void calculateCircleCenter()
    {
        float x1, x2, x3, z1, z2, z3;
        float x = 0;
        float z = 0;

        x1 = a.x;
        x2 = b.x;
        x3 = c.x;
        z1 = a.z;
        z2 = b.z;
        z3 = c.z;
        x = ((z2 - z1) * (z3 * z3 - z1 * z1 + x3 * x3 - x1 * x1) - (z3 - z1) * (z2 * z2 - z1 * z1 + x2 * x2 - x1 * x1))
                / (2 * (x3 - x1) * (z2 - z1) - 2 * ((x2 - x1) * (z3 - z1)));
        z = ((x2 - x1) * (x3 * x3 - x1 * x1 + z3 * z3 - z1 * z1) - (x3 - x1) * (x2 * x2 - x1 * x1 + z2 * z2 - z1 * z1))
                / (2 * (z3 - z1) * (x2 - x1) - 2 * ((z2 - z1) * (x3 - x1)));

        center.x = x;
        center.z = z;
        center.y = 0;
        radius = (float)Math.Sqrt(Math.Abs(a.x - x) * Math.Abs(a.x - x) + Math.Abs(a.z - z) * Math.Abs(a.z - z));
    }

    /**
	 * 构建 DelaunayTriangle 三角形
	 * 
	 * @param vector3s
	 * @return
	 */
    public static List<DelaunayTriangle> buildDelaunayTriangle(List<DelaunayTriangle> triangles,
            List<Vector3> vector3s)
    {
        for (int i = 0; i < vector3s.Count(); i++)
        {
            Vector3 vector3 = vector3s.ElementAt(i);
            List<DelaunayTriangle> tempTriangles = new List<DelaunayTriangle>();
            tempTriangles.AddRange(triangles);

            // 受影响的三角形链表
            List<DelaunayTriangle> influenedTriangles = new List<DelaunayTriangle>();
            // 新形成的三角形链表
            List<DelaunayTriangle> newTriangles = new List<DelaunayTriangle>();
            // 受影响三角形的公共边
            List<TriangleEdge> commonEdges = new List<TriangleEdge>();

            for (int j = 0; j < tempTriangles.Count(); j++)
            {
                DelaunayTriangle tempTriangle = tempTriangles.ElementAt(j);
                double lengthToCenter = Vector3.dst(tempTriangle.center, vector3);// 该点到圆心距离
                if (lengthToCenter < tempTriangle.radius)
                { // 点在三角形内部
                    influenedTriangles.Add(tempTriangle); // 添加到受影响的三角形链表
                    triangles.Remove(tempTriangle); // 移除当前三角形
                }
            }

            // 将点与受影响的三角形三点连接，形成新的三个三角形添加到三角形链中
            for (int k = 0; k < influenedTriangles.Count(); k++)
            {
                DelaunayTriangle delaunayTriangle = influenedTriangles.ElementAt(k);
                newTriangles.Add(new DelaunayTriangle(delaunayTriangle.a, delaunayTriangle.b, vector3));
                newTriangles.Add(new DelaunayTriangle(delaunayTriangle.a, delaunayTriangle.c, vector3));
                newTriangles.Add(new DelaunayTriangle(delaunayTriangle.b, delaunayTriangle.c, vector3));
            }

            // 查找受影响三角形的公共边
            if (influenedTriangles.Count() > 1)
            {
                commonEdges = findCommonEdges(influenedTriangles);
            }

            // 将受影响三角形中的公共边所在的新形成的三角形排除
            if (commonEdges.Count() > 0)
            {
                remmoveTrianglesByEdges(newTriangles, commonEdges);
            }

            // 对新形成的三角形进行局部优化
            LOP(newTriangles);

            // 将优化后的新形成的三角形添加到三角形链表中
            triangles.AddRange(newTriangles);
        }
        return triangles;
    }

    /**
	 * 对新形成的三角形进行局部优化
	 * 
	 * @param newTriList
	 * @return
	 */
    public static List<DelaunayTriangle> LOP(List<DelaunayTriangle> newTriList)
    {
        List<DelaunayTriangle> resultTriList = new List<DelaunayTriangle>();
        // 拷贝新形成的三角
        resultTriList.AddRange(newTriList);

        for (int i = 0; i < newTriList.Count(); i++)
        {
            for (int j = i + 1; j < newTriList.Count(); j++)
            {
                TriangleEdge commonEdge;// 需要调整对角线的的三角形的公共边
                Vector3 anotherPoint = new Vector3();// 新对角线的另一点
                if (isInCircle(newTriList.ElementAt(j), newTriList.ElementAt(i).a))// 三角形点在外接圆内
                {
                    // 找出两个三角形的公共边
                    commonEdge = findCommonEdge(newTriList.ElementAt(i), newTriList.ElementAt(j));
                    if (commonEdge != null)
                    {
                        // 移除需要调整的三角形
                        resultTriList.Remove(newTriList.ElementAt(i));
                        resultTriList.Remove(newTriList.ElementAt(j));
                        // 找出对角线的另一点
                        if (newTriList.ElementAt(j).a.equal(commonEdge.leftVertex) == false
                                && newTriList.ElementAt(j).a.equal(commonEdge.rightVertex) == false)
                            anotherPoint = newTriList.ElementAt(j).a;
                        if (newTriList.ElementAt(j).b.equal(commonEdge.leftVertex) == false
                                && newTriList.ElementAt(j).b.equal(commonEdge.rightVertex) == false)
                            anotherPoint = newTriList.ElementAt(j).b;
                        if (newTriList.ElementAt(j).c.equal(commonEdge.leftVertex) == false
                                && newTriList.ElementAt(j).c.equal(commonEdge.rightVertex) == false)
                            anotherPoint = newTriList.ElementAt(j).c;
                        // 形成两个新的三角形
                        resultTriList
                                .Add(new DelaunayTriangle(newTriList.ElementAt(i).a, anotherPoint, commonEdge.leftVertex));
                        resultTriList
                                .Add(new DelaunayTriangle(newTriList.ElementAt(i).a, anotherPoint, commonEdge.rightVertex));
                    }
                }
                if (isInCircle(newTriList.ElementAt(j), newTriList.ElementAt(i).b))// 三角形点在外接圆内
                {
                    // 找出两个三角形的公共边
                    commonEdge = findCommonEdge(newTriList.ElementAt(i), newTriList.ElementAt(j));
                    if (commonEdge != null)
                    {
                        // 移除需要调整的三角形
                        resultTriList.Remove(newTriList.ElementAt(i));
                        resultTriList.Remove(newTriList.ElementAt(j));
                        // 找出对角线的另一点
                        if (newTriList.ElementAt(j).a.equal(commonEdge.leftVertex) == false
                                && newTriList.ElementAt(j).a.equal(commonEdge.rightVertex) == false)
                            anotherPoint = newTriList.ElementAt(j).a;
                        if (newTriList.ElementAt(j).b.equal(commonEdge.leftVertex) == false
                                && newTriList.ElementAt(j).b.equal(commonEdge.rightVertex) == false)
                            anotherPoint = newTriList.ElementAt(j).b;
                        if (newTriList.ElementAt(j).c.equal(commonEdge.leftVertex) == false
                                && newTriList.ElementAt(j).c.equal(commonEdge.rightVertex) == false)
                            anotherPoint = newTriList.ElementAt(j).c;
                        // 形成两个新的三角形
                        resultTriList
                                .Add(new DelaunayTriangle(newTriList.ElementAt(i).b, anotherPoint, commonEdge.leftVertex));
                        resultTriList
                                .Add(new DelaunayTriangle(newTriList.ElementAt(i).b, anotherPoint, commonEdge.rightVertex));
                    }
                }

                if (isInCircle(newTriList.ElementAt(j), newTriList.ElementAt(i).c))// 三角形点在外接圆内
                {
                    // 找出两个三角形的公共边
                    commonEdge = findCommonEdge(newTriList.ElementAt(i), newTriList.ElementAt(j));
                    if (commonEdge != null)
                    {
                        // 移除需要调整的三角形
                        resultTriList.Remove(newTriList.ElementAt(i));
                        resultTriList.Remove(newTriList.ElementAt(j));
                        // 找出对角线的另一点
                        if (newTriList.ElementAt(j).a.equal(commonEdge.leftVertex) == false
                                && newTriList.ElementAt(j).a.equal(commonEdge.rightVertex) == false)
                            anotherPoint = newTriList.ElementAt(j).a;
                        if (newTriList.ElementAt(j).b.equal(commonEdge.leftVertex) == false
                                && newTriList.ElementAt(j).b.equal(commonEdge.rightVertex) == false)
                            anotherPoint = newTriList.ElementAt(j).b;
                        if (newTriList.ElementAt(j).c.equal(commonEdge.leftVertex) == false
                                && newTriList.ElementAt(j).c.equal(commonEdge.rightVertex) == false)
                            anotherPoint = newTriList.ElementAt(j).c;
                        // 形成两个新的三角形
                        resultTriList
                                .Add(new DelaunayTriangle(newTriList.ElementAt(i).c, anotherPoint, commonEdge.leftVertex));
                        resultTriList
                                .Add(new DelaunayTriangle(newTriList.ElementAt(i).c, anotherPoint, commonEdge.rightVertex));
                    }
                }
            }
        }
        newTriList = resultTriList;
        return resultTriList;// 返回优化后的新形成的三角形
    }

    /**
	 * 移除所有边边所在的三角形
	 */
    private static void remmoveTrianglesByEdges(List<DelaunayTriangle> triangles, List<TriangleEdge> edges)
    {
        // 拷贝所有三角形
        List<DelaunayTriangle> tmpTriList = new List<DelaunayTriangle>(triangles);

        for (int i = 0; i < tmpTriList.Count(); i++)
        {
            for (int j = 0; j < edges.Count(); j++)
            {
                if (isEdgeOnTriangle(tmpTriList.ElementAt(i), edges.ElementAt(j)))
                {
                    triangles.Remove(tmpTriList.ElementAt(i));
                }
            }
        }
    }

    /**
	 * 判断边是否属于三角形
	 * 
	 * @param edge
	 * @return
	 */
    public static bool isEdgeOnTriangle(DelaunayTriangle triangle, TriangleEdge edge)
    {
        int samePointNum = 0;
        if (edge.leftVertex.equal(triangle.a) || edge.leftVertex.equal(triangle.b)
                || edge.leftVertex.equal(triangle.c))
        {
            samePointNum++;
        }
        if (edge.rightVertex.equal(triangle.a) || edge.rightVertex.equal(triangle.b)
                || edge.rightVertex.equal(triangle.c))
        {
            samePointNum++;
        }
        if (samePointNum == 2)
        {
            return true;
        }
        return false;
    }

    /**
	 * 找出受影响的三角形的公共边
	 * 
	 * @param influenedTriangles
	 * @return
	 */
    public static List<TriangleEdge> findCommonEdges(List<DelaunayTriangle> influenedTriangles)
    {
        List<TriangleEdge> commonEdges = new List<TriangleEdge>();
        TriangleEdge tmpEdge;
        for (int i = 0; i < influenedTriangles.Count(); i++)
        {
            for (int j = i + 1; j < influenedTriangles.Count(); j++)
            {
                tmpEdge = findCommonEdge(influenedTriangles.ElementAt(i), influenedTriangles.ElementAt(j));
                if (tmpEdge != null)
                {
                    commonEdges.Add(tmpEdge);
                }
            }
        }
        return commonEdges;
    }

    /**
	 * 找出两个三角形的公共边
	 */
    public static TriangleEdge findCommonEdge(DelaunayTriangle triangle1, DelaunayTriangle triangle2)
    {
        TriangleEdge edge;
        List<Vector3> commonVectors = new List<Vector3>();
        if (triangle1.a.equal(triangle2.a) || triangle1.a.equal(triangle2.b) || triangle1.a.equal(triangle2.c))
        {
            commonVectors.Add(triangle1.a);
        }
        if (triangle1.b.equal(triangle2.a) || triangle1.b.equal(triangle2.b) || triangle1.b.equal(triangle2.c))
        {
            commonVectors.Add(triangle1.b);
        }
        if (triangle1.c.equal(triangle2.a) || triangle1.c.equal(triangle2.b) || triangle1.c.equal(triangle2.c))
        {
            commonVectors.Add(triangle1.c);
        }
        if (commonVectors.Count() == 2)
        {
            edge = new TriangleEdge(commonVectors.ElementAt(1), commonVectors.ElementAt(0));
            return edge;
        }
        return null;
    }

    /**
	 * 判断点是否在三角形外接圆的内部
	 * 
	 * @param triangle
	 * @param vector3
	 * @return
	 */
    public static bool isInCircle(DelaunayTriangle triangle, Vector3 vector3)
    {
        double lengthToCenter = triangle.center.dst(vector3);// 该点到圆心距离
        if (lengthToCenter < triangle.radius)
        {
            return true;
        }
        return false;
    }

    /**
	 * 获取三角形所有的边对象
	 * 
	 * @param triangles
	 * @return
	 */
    public HashSet<TriangleEdge> getEdgesOfTriangles(List<DelaunayTriangle> triangles)
    {
        HashSet<TriangleEdge> commonEdges = new HashSet<TriangleEdge>();
        for (int i = 0; i < triangles.Count(); i++)
        {
            TriangleEdge edge1 = new TriangleEdge(triangles.ElementAt(i).a, triangles.ElementAt(i).b);
            TriangleEdge edge2 = new TriangleEdge(triangles.ElementAt(i).a, triangles.ElementAt(i).c);
            TriangleEdge edge3 = new TriangleEdge(triangles.ElementAt(i).b, triangles.ElementAt(i).c);
            commonEdges.Add(edge1);
            commonEdges.Add(edge2);
            commonEdges.Add(edge3);
        }
        return commonEdges;
    }

}
