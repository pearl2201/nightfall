


using Pika.Ai.Nav.NavPolygon;
using Pika.Ai.Quadtree;
using Pika.Base.Mathj.Geometry;
using Pika.Base.Utils;
using System;
using System.Collections.Generic;
using System.Linq;


/**
 * 多边形图对象
 * 
 * @author JiangZhiYong
 * @mail 359135103@qq.com
 */
public class PolygonGraph : IndexedGraph<Polygon>
{


    private List<Polygon> polygons = new List<Polygon>();

    private IDictionary<Polygon, List<PolygonEdge>> sharedEdges;// 多边形共享边关联关系
    private ISet<IndexConnection> indexConnections = new HashSet<IndexConnection>();// 多边形共享边
    /** 坐标缩放倍数 */
    private int scale;
    private PolygonData polygonData;
    /** 缓存的随机点 x z */
    private Dictionary<int, IDictionary<int, List<Vector3>>> allRandomPointsInPath = new Dictionary<int, IDictionary<int, List<Vector3>>>();
    private List<Vector3> allPoints = new List<Vector3>();
    /** 缓存多边形 */
    private QuadTree<Vector3, Polygon> quadTree;

    public PolygonGraph(PolygonData polygonData, int scale)
    {
        this.scale = scale;
        this.polygonData = polygonData;
        this.polygonData.check(scale);
        initCalculate(polygonData, scale);
    }

    /**
	 * 初始化计算
	 * 
	 * @param polygonData
	 * @param scale
	 */
    private void initCalculate(PolygonData polygonData, int scale)
    {
        quadTree = new PolygonGuadTree(polygonData.getStartX() * scale, polygonData.getStartZ() * scale,
                polygonData.getEndX() * scale, polygonData.getEndZ() * scale, (int)(polygonData.getWidth() / 50), 10);
        createPolygons(polygonData, scale);// 创建多变形
        createPathRandomPoint();// 生成多变形内的随机点
        calculateIndexConnections(polygonData.getPathPolygonIndexs());// 计算共享边
        sharedEdges = createSharedEdgesMap(indexConnections, polygons);// 创建共享边对应关系
        initPathRandomPoint();
        PikaLogger.Debug("地图：{} 多边形个数：{} 共享边：{}", polygonData.getMapID(), polygons.Count(), indexConnections.Count());
    }


    public List<Connection<Polygon>> getConnections(Polygon fromNode)
    {
        var temp = sharedEdges[fromNode];
        return temp.Cast<Connection<Polygon>>().ToList();
    }


    public int getIndex(Polygon node)
    {
        return node.getIndex();
    }


    public int getNodeCount()
    {
        return sharedEdges.Count();
    }

    /**
	 * 计算共享边
	 * 
	 * @note 两个多边形只存一个共享边
	 */
    private void calculateIndexConnections(IDictionary<int, ISet<int>> polygonVectorIndexs)
    {
        int i = 0, j = 0, m = 0;
        Vector3[] edge = { null, null };
        int[][] indices = new int[polygonVectorIndexs.Count()][];

        foreach (var entry in polygonVectorIndexs)
        {
            var value = entry.Value;
            indices[m] = new int[value.Count()];
            int n = 0;
            foreach (int index in value)
            {
                indices[m][n++] = index;
            }
            m++;
        }

        while (i < indices.Length)
        {
            int[] polygonAIndex = indices[i];
            j = 0;
            while (j < indices.Length)
            {
                if (i == j)
                {
                    j++;
                    continue;
                }
                int[] polygonBIndex = indices[j];
                if (hasSharedEdgeIndices(polygonAIndex, polygonBIndex, edge))
                {
                    IndexConnection indexConnection1 = new IndexConnection(edge[0], edge[1], i, j);
                    IndexConnection indexConnection2 = new IndexConnection(edge[1], edge[0], j, i);
                    indexConnections.Add(indexConnection1);
                    indexConnections.Add(indexConnection2);
                    // PikaLogger.Debug("共享边1：{}", indexConnection1.ToString());
                    // PikaLogger.Debug("共享边2：{}", indexConnection2.ToString());
                    edge[0] = null;
                    edge[1] = null;
                }
                j++;
            }
            i++;
        }
    }

    /**
	 * 多边形是否有共享边
	 * 
	 * @param polygonAIndex
	 * @param polygonBIndex
	 * @param edge
	 *            共享边顶点坐标
	 * @return
	 */
    private bool hasSharedEdgeIndices(int[] polygonAIndex, int[] polygonBIndex, Vector3[] edge)
    {
        int aLength = polygonAIndex.Length;
        int bLength = polygonBIndex.Length;
        float precision = 0.001f;
        for (int i = 0; i < polygonAIndex.Length; i++)
        {
            Vector3 av1 = polygonData.getPathVertices()[polygonAIndex[i]];
            Vector3 av2 = polygonData.getPathVertices()[polygonAIndex[(i + 1) % aLength]];

            for (int j = 0; j < polygonBIndex.Length; j++)
            {
                Vector3 bv0 = null;
                if (j > 0)
                {
                    bv0 = polygonData.getPathVertices()[polygonBIndex[j - 1]];
                }
                else
                {
                    bv0 = polygonData.getPathVertices()[polygonBIndex[polygonBIndex.Length - 1]];
                }

                Vector3 bv1 = polygonData.getPathVertices()[polygonBIndex[j]];
                Vector3 bv2 = polygonData.getPathVertices()[polygonBIndex[(j + 1) % bLength]];

                // 顺序相等
                if (av1.equal(bv1, precision) && av2.equal(bv2, precision))
                {
                    edge[0] = av1;
                    edge[1] = av2;
                    return true;

                    // 逆序相等
                }
                else if (/* bv0 != null && */ av1.equal(bv1, precision) && av2.equal(bv0, precision))
                {
                    edge[0] = av1;
                    edge[1] = av2;
                    return true;

                }
                /**
				 * unity自带navmesh导出工具存在共享边包含关系(共享边存在三个顶点)，强制加成共享边 TODO
				 * 能解决部分问，但是unity自带工具导出地图还存在找不到共边的其他问题
				 **/
                else if (av1.equal(bv1, precision) && !av2.equal(bv0, precision)
                        && Vector3.relCCW(av1.x, av1.z, av2.x, av2.z, bv0.x, bv0.z) == 0)
                {
                    Vector3 dirVector1 = Vector3.dirVector(av1, av2);
                    Vector3 dirVector2 = Vector3.dirVector(av1, bv0);
                    edge[0] = av1;
                    if (dirVector1.len2() > dirVector2.len2())
                    {
                        edge[1] = bv0;
                    }
                    edge[1] = av2;
                    if (dirVector1.nor().equal(dirVector2.nor(), 0.001f))
                    {
                        return true;
                    }
                }
                else if (!av1.equal(bv1, precision) && av2.equal(bv0, precision)
                        && Vector3.relCCW(av1.x, av1.z, av2.x, av2.z, bv1.x, bv1.z) == 0)
                {
                    Vector3 dirVector1 = Vector3.dirVector(av2, av1);
                    Vector3 dirVector2 = Vector3.dirVector(av2, bv1);
                    edge[0] = av1;
                    if (dirVector1.len2() > dirVector2.len2())
                    {
                        edge[1] = bv1;
                    }
                    edge[1] = av2;
                    if (dirVector1.nor().equal(dirVector2.nor(), 0.001f))
                    {
                        return true;
                    }
                    // 逆序第一个顶点共顶点
                }
                else if (av1.equal(bv2) && !av2.equal(bv1)
                        && Vector3.relCCW(av1.x, av1.z, av2.x, av2.z, bv1.x, bv1.z) == 0)
                {
                    Vector3 dirVector1 = Vector3.dirVector(av1, av2);
                    Vector3 dirVector2 = Vector3.dirVector(av1, bv1);
                    edge[0] = av1;
                    edge[1] = av2;
                    if (dirVector1.len2() > dirVector2.len2())
                    {
                        edge[1] = bv1;
                    }
                    if (dirVector1.nor().equal(dirVector2.nor(), precision))
                    {
                        return true;
                    }
                    // 逆序第二个顶点共顶点
                }
                else if (!av1.equal(bv2) && av2.equal(bv1)
                        && Vector3.relCCW(av1.x, av1.z, av2.x, av2.z, bv2.x, bv2.z) == 0)
                {
                    Vector3 dirVector1 = Vector3.dirVector(av2, av1);
                    Vector3 dirVector2 = Vector3.dirVector(av2, bv2);
                    edge[0] = av2;
                    edge[1] = av1;
                    if (dirVector1.len2() > dirVector2.len2())
                    {
                        edge[1] = bv2;
                    }
                    if (dirVector1.nor().equal(dirVector2.nor(), precision))
                    {
                        return true;
                    }
                    // 顺序
                }
                else if (!av1.equal(bv1) && av2.equal(bv2)
                        && Vector3.relCCW(av2.x, av2.z, av1.x, av1.z, bv1.x, bv1.z) == 0)
                {
                    Vector3 dirVector1 = Vector3.dirVector(av2, av1);
                    Vector3 dirVector2 = Vector3.dirVector(av2, bv1);
                    edge[0] = av2;
                    edge[1] = av1;
                    if (dirVector1.len2() > dirVector2.len2())
                    {
                        edge[1] = bv1;
                    }
                    if (dirVector1.nor().equal(dirVector2.nor(), precision))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /***
	 * 创建多边形
	 * 
	 * @param scale
	 * @return
	 */
    private List<Polygon> createPolygons(PolygonData polygonData, int scale)
    {
        var entrySet = (SortedDictionary<int, ISet<int>>)polygonData.getPathPolygonIndexs();
        foreach (var entry in entrySet)
        {
            int key = entry.Key;
            var value = entry.Value;
            List<Vector3> points = new List<Vector3>();
            int[] vectorIndexs = new int[value.Count()];
            int i = 0;
            foreach (int index in value)
            {
                points.Add(polygonData.getPathVertices()[index]);
                vectorIndexs[i++] = index;
            }
            Polygon polygon = new Polygon(key, points, vectorIndexs);
            if (!polygon.convex)
            {
                PikaLogger.Debug("多边形{}不是凸多边形", polygon.ToString());
                continue;
            }
            //if (LOGGER.isDebugEnabled())
            {
                // polygon.print();
                // polygons.forEach(p->{
                // if(p.contains(polygon)) {
                // LOGGER.warn("多边形{}是多边形{}的内嵌多边形",polygon.getIndex(),p.getIndex());
                // }else if(polygon.contains(p)) {
                // LOGGER.warn("多边形{}是多边形{}的内嵌多边形",p.getIndex(),polygon.getIndex());
                // }
                // if(p.intersectsEdge(polygon)) {
                // LOGGER.warn("多边形{}与多边形{}的边相交",p.getIndex(),polygon.getIndex());
                // }
                // });
            }

            polygons.Add(polygon);
            try
            {
                quadTree.set(polygon.center, polygon);
            }
            catch (Exception e)
            {
                PikaLogger.Debug(String.Format("地图%d 添加节点错误", polygonData.getMapID()), e);
            }

        }

        return polygons;
    }

    /**
	 * 创建多边形内的随机点 <br>
	 * 未找到合适方法，先生成三角形，三角形生成随机点，判断点是在哪个多边形内
	 */
    public void createPathRandomPoint()
    {
        int[] indexs = polygonData.getPathTriangles();
        Vector3[] vertices = polygonData.getPathVertices();
        for (int i = 0; i < indexs.Length;)
        {
            Triangle triangle = new Triangle(vertices[indexs[i++]], vertices[indexs[i++]], vertices[indexs[i++]], i);
            int count = (int)(triangle.area() / (this.scale * 5)) + 1;
            // TODO 分层问题？
            Polygon findAny = polygons.Where(p => p.isInnerPoint(triangle.center)).FirstOrDefault();
            if (findAny != null)
            {
                continue;
            }

            for (int j = 0; j < count; j++)
            {
                findAny.randomPoints.Add(triangle.getRandomPoint(new Vector3()));
            }
        }
    }

    /**
	 * 初始化所有随机点 <br>
	 * 以空间换时间
	 * 
	 */
    public void initPathRandomPoint()
    {
        int count = 0;
        int x, z;
        foreach (Polygon polygon in getPolygons())
        {
            foreach (Vector3 point in polygon.randomPoints)
            {
                x = (int)point.x;
                z = (int)point.z;
                IDictionary<int, List<Vector3>> map = allRandomPointsInPath[x];
                if (map == null)
                {
                    map = new Dictionary<int, List<Vector3>>();
                    allRandomPointsInPath.Add(x, map);
                }
                List<Vector3> list = map[z];
                if (list == null)
                {
                    list = new List<Vector3>();
                    map.Add(z, list);
                }
                list.Add(point);
                allPoints.Add(point);
                count++;
            }
        }
        PikaLogger.Debug("{0}-{1}", getPolygonData().getMapID(), count);
    }

    public PolygonData getPolygonData()
    {
        return polygonData;
    }

    public List<Polygon> getPolygons()
    {
        return polygons;
    }

    private static IDictionary<Polygon, List<PolygonEdge>> createSharedEdgesMap(ISet<IndexConnection> indexConnections,
            List<Polygon> polygons)
    {

        IDictionary<Polygon, List<PolygonEdge>> connectionMap = new SortedDictionary<Polygon, List<PolygonEdge>>(new PolygonComperator());
        // connectionMap.ordered = true;

        foreach (Polygon polygon in polygons)
        {
            connectionMap.Add(polygon, new List<PolygonEdge>());
        }

        foreach (IndexConnection indexConnection in indexConnections)
        {
            Polygon fromNode = polygons.ElementAt(indexConnection.fromPolygonIndex);
            Polygon toNode = polygons.ElementAt(indexConnection.toPolygonIndex);
            Vector3 edgeVertexA = indexConnection.edgeVector1;
            Vector3 edgeVertexB = indexConnection.edgeVector2;

            PolygonEdge edge = new PolygonEdge(fromNode, toNode, edgeVertexA, edgeVertexB);
            connectionMap[fromNode].Add(edge);
            fromNode.connections.Add(edge);
        }
        return connectionMap;
    }

    public Dictionary<int, IDictionary<int, List<Vector3>>> getAllRandomPointsInPath()
    {
        return allRandomPointsInPath;
    }

    public QuadTree<Vector3, Polygon> getQuadTree()
    {
        return quadTree;
    }

    /**
	 * 存储相互连接多边形的关系 Class for storing the edge connection data between two adjacent
	 * triangles.
	 */
    private class IndexConnection
    {
        // // The vertex indices which makes up the edge shared between two triangles.
        // int edgeVertexIndex1;
        // int edgeVertexIndex2;
        public Vector3 edgeVector1;
        public Vector3 edgeVector2;
        // The indices of the two polygon sharing this edge.
        public int fromPolygonIndex;
        public int toPolygonIndex;

        public IndexConnection(Vector3 edgeVector1, Vector3 edgeVector2, int fromPolygonIndex, int toPolygonIndex)
        {
            this.edgeVector1 = edgeVector1;
            this.edgeVector2 = edgeVector2;
            this.fromPolygonIndex = fromPolygonIndex;
            this.toPolygonIndex = toPolygonIndex;
        }


        public override string ToString()
        {
            return "IndexConnection [edgeVector1=" + edgeVector1 + ", edgeVector2=" + edgeVector2
                    + ", fromPolygonIndex=" + fromPolygonIndex + ", toPolygonIndex=" + toPolygonIndex + "]";
        }

    }

    public List<Vector3> getAllPoints()
    {
        return allPoints;
    }

    public int getScale()
    {
        return scale;
    }
}

public class PolygonComperator : IComparer<Polygon>
{
    public int Compare(Polygon x, Polygon y)
    {
        return x.getIndex() - y.getIndex();
    }
}