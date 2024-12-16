
//import com.game.model.enums.ConstantConfig;


using Newtonsoft.Json;
using Pika.Ai.Nav;
using Pika.Ai.Nav.NavPolygon;
using Pika.Base.Mathj.Geometry;
using Pika.Base.Utils;
using System;
using System.Collections.Generic;
using System.Linq;


/**
 * 多边形寻路
 * 
 * <h3>思路</h3>
 * <p>
 * 1、整个寻路的网格是由多个互相连接的凸多边形组成<br>
 * 2、初始化网格的时候，计算出凸多边形互相相邻的边，和通过互相相邻的边到达另外一个多边形的距离。<br>
 * 3、开始寻路时，首先肯定是会得到一个开始点的坐标和结束点的坐标<br>
 * 4、判断开始点和结束点分别位于哪个多边形的内部（如果没有上下重叠的地形，可以只通过x和z坐标，高度先忽略掉），把多边形的编号记录下来<br>
 * 5、使用A*寻路，找到从开始的多边形到结束的多边形将会经过哪几个多边形，记录下来。<br>
 * 6、在得到途径的多边形后， 从开始点开始，根据拐点算法，计算出路径的各个拐点组成了路径点坐标。（忽略高度，只计算出x和z）。<br>
 * 7、到上一步，2D寻路部分结束。人物根据路径点做移动。<br>
 * 8、假如需要3D高度计算，那么在获得了刚才2D寻路的路径点之后，再分别和途径的多边形的边做交点计算，得出经过每一个边时的交点，那么当多边形与多边形之间有高低变化，路径点也就通过边的交点同样的产生高度的变化。<br>
 * <p>
 * 
 * @author JiangZhiYong
 * @date 2018年2月23日
 * @mail 359135103@qq.com
 */
public class PolygonNavMesh : NavMesh
{

    /** 高度验证精度 */
    private const int HIGH_PRECISION = 6;
    private PolygonGraph graph;
    private PolygonHeuristic heuristic;// 计算寻路消耗
    private IndexedAStarPathFinder<Polygon> pathFinder;

    public PolygonNavMesh(string navMeshStr) : this(navMeshStr, 1)
    {

    }

    /**
	 * @param navMeshStr 导航网格数据
	 * @param scale      放大倍数
	 */
    public PolygonNavMesh(string navMeshStr, int scale)
    {
        var polygonData = JsonConvert.DeserializeObject<PolygonData>(navMeshStr);
        graph = new PolygonGraph(polygonData, scale);
        pathFinder = new IndexedAStarPathFinder<Polygon>(graph);
        heuristic = new PolygonHeuristic();
    }

    /**
     * 查询路径
     * 
     * @param fromPoint
     * @param toPoint
     * @param path
     */
    public bool findPath(Vector3 fromPoint, Vector3 toPoint, PolygonGraphPath path)
    {
        path.clear();
        Polygon fromPolygon = getPolygon(fromPoint);
        Polygon toPolygon;
        // 起点终点在同一个多边形中
        if (fromPolygon != null && fromPolygon.isInnerPoint(toPoint))
        {
            toPolygon = fromPolygon;
        }
        else
        {
            toPolygon = getPolygon(toPoint);
            if (toPolygon == null)
            {
                PikaLogger.Info("点{}不在地图{}行走层", toPoint.ToString(), getMapId());
                return false;
            }
        }
        lock (pathFinder)
        {
            if (pathFinder.searchConnectionPath(fromPolygon, toPolygon, heuristic, path))
            {
                path.start = new Vector3(fromPoint);
                path.end = new Vector3(toPoint);
                path.startPolygon = fromPolygon;
                return true;
            }
        }
        return false;
    }

    /**
     * 查询路径
     * <p>
     * 丢失部分多边形坐标，有高度误差，运算速度较快
     * </p>
     * 
     * @param fromPoint
     * @param toPoint
     * @param pointPath
     * @return
     */
    public List<Vector3> findPath(Vector3 fromPoint, Vector3 toPoint, PolygonPointPath pointPath)
    {
        PolygonGraphPath polygonGraphPath = new PolygonGraphPath();
        bool find = findPath(fromPoint, toPoint, polygonGraphPath);
        if (!find)
        {
            return pointPath.getVectors();
        }
        // 计算坐标点
        pointPath.calculateForGraphPath(polygonGraphPath, false);

        return pointPath.getVectors();
    }

    /**
     * 查询路径
     * 
     * @param fromPoint
     * @param toPoint
     * @param fromPolygon
     * @param toPolygon
     * @return
     */
    public List<Vector3> findPath(Vector3 fromPoint, Vector3 toPoint, Polygon fromPolygon, Polygon toPolygon)
    {
        // 起点和目标点在同一个多边形中，直接返回
        if (fromPolygon == toPolygon)
        {
            List<Vector3> list = new List<Vector3>();
            list.Add(fromPoint);
            list.Add(toPoint);
            return list;
        }
        PolygonPointPath pointPath = new PolygonPointPath();
        PolygonGraphPath polygonGraphPath = new PolygonGraphPath();
        lock (pathFinder)
        {
            if (pathFinder.searchConnectionPath(fromPolygon, toPolygon, heuristic, polygonGraphPath))
            {
                polygonGraphPath.start = fromPoint;
                polygonGraphPath.end = toPoint;
                polygonGraphPath.startPolygon = fromPolygon;
            }
            else
            {
                return pointPath.getVectors();
            }
        }
        // 计算坐标点
        pointPath.calculateForGraphPath(polygonGraphPath, false);
        return pointPath.getVectors();

    }

    /**
     * 查询有高度路径
     * 
     * @param fromPoint
     * @param toPoint
     * @param pointPath
     * @return
     */
    public List<Vector3> find3DPath(Vector3 fromPoint, Vector3 toPoint, PolygonPointPath pointPath)
    {
        PolygonGraphPath polygonGraphPath = new PolygonGraphPath();
        bool find = findPath(fromPoint, toPoint, polygonGraphPath);
        if (!find)
        {
            return pointPath.getVectors();
        }
        // 计算坐标点
        pointPath.calculateForGraphPath(polygonGraphPath, true);

        return pointPath.getVectors();
    }

    /**
     * 坐标点所在的多边形
     * 
     * @param point
     * @return
     */
    public Polygon getPolygon(Vector3 point)
    {
        // // 3D地图，有navmesh地图重叠，验证坐标高度
        // // NOTE 3D地图获取数据需要全部遍历，有计算误差，可能查找到错误多边形
        // if (graph.getPolygonData().isThreeDimensional()) {
        // Polygon p = null;
        // float minDistance = Byte.MAX_VALUE;
        // int count = 0;
        // for (Polygon polygon : graph.getPolygons()) {
        // if (polygon.isInnerPoint(point)) {
        // float distance = Math.Abs(polygon.center.y - point.y);
        // // 高度验证，高度不能太高，会进入其他区域，不精准
        // if (distance > ConstantConfig.getInstance().getDefaultAttackHight()) {
        // continue;
        // }
        //
        // //
        // PikaLogger.Debug("距离{}，坐标点:{},多边形：{}",distance,point.ToString(),polygon.ToString());
        // if (distance < minDistance) {
        // p = polygon;
        // minDistance = distance;
        // count++;
        // }
        // }
        // }
        // if (LOGGER.isDebugEnabled() && count > 1) {
        // PikaLogger.Debug("坐标点：{} 所在多边形--{}", point.ToString(), p.ToString());
        // }
        // return p;
        // } else {
        // Optional<Polygon> findFirst = graph.getPolygons().stream().filter(p ->
        // p.isInnerPoint(point)).findFirst();
        // if (findFirst.isPresent()) {
        // return findFirst.ElementAt();
        // }
        // }
        //
        // return null;

        Polygon polygon = graph.getQuadTree().get(point, null);

        return polygon;
    }


    public override Vector3 getPointInPath(float x, float z)
    {
        Vector3 vector3 = new Vector3(x, z);
        Polygon polygon = getPolygon(vector3);
        if (polygon == null)
        {
            PikaLogger.Info("地图{}坐标({},{})不在路径中", getMapId(), x, z);
            return null;
        }
        vector3.y = polygon.y;
        return vector3;
    }

    public PolygonGraph getGraph()
    {
        return graph;
    }

    /**
     * 获取矩形
     * 
     * @param position        当前位置，一般为玩家坐标
     * @param distance        矩形最近边中点到当前位置的距离
     * @param sourceDirection 方向向量
     * @param width           宽度
     * @param height          高度
     * @return
     */
    public Polygon getRectangle(Vector3 position, float distance, Vector3 sourceDirection, float width, float height)
    {
        Vector3 source = position.unityTranslate(sourceDirection, 0f, distance); // 中心坐标
        Vector3 corner_1 = source.unityTranslate(sourceDirection, -90, width / 2);
        Vector3 corner_2 = source.unityTranslate(sourceDirection, 90, width / 2);
        Vector3 corner_3 = corner_2.unityTranslate(sourceDirection, 0, height);
        Vector3 corner_4 = corner_1.unityTranslate(sourceDirection, 0, height);
        List<Vector3> list = new List<Vector3>(4);
        list.Add(corner_1);
        list.Add(corner_4);
        list.Add(corner_3);
        list.Add(corner_2);
        return new Polygon(0, list, null, false);
    }

    /**
     * 获取扇形 <br>
     * 由多边形组成
     * 
     * @param position        当前位置，一般为玩家坐标
     * @param sourceDirection 方向向量
     * @param distance        扇形起点到当前位置的距离
     * @param radius          扇形半径
     * @param degrees         扇形角度
     * @return
     */
    public Polygon getSector(Vector3 position, Vector3 sourceDirection, float distance, float radius,
            float degrees)
    {
        Vector3 source = position.unityTranslate(sourceDirection, 0, distance); // 中心坐标
        Vector3 forward_l = source.unityTranslate(sourceDirection, -degrees / 2, radius);
        Vector3 forward_r = source.unityTranslate(sourceDirection, degrees / 2, radius);
        List<Vector3> sectors = new List<Vector3>(6);
        sectors.Add(source);
        sectors.Add(forward_l);
        int size = (int)(degrees / 10) / 2 - 1;
        for (int i = -size; i <= size; i++)
        {
            Vector3 forward = source.unityTranslate(sourceDirection, i * 10, radius);
            sectors.Add(forward);
        }
        sectors.Add(forward_r);
        return new Polygon(0, sectors, null, false);
    }

    /**
     * 获取N正多边形 <br>
     * N大于15基本上接近圆
     * 
     * @param center      中心点
     * @param radius      半径
     * @param vertexCount 顶点个数
     * @return
     */
    public Polygon getNPolygon(Vector3 center, float radius, int vertexCount)
    {
        if (vertexCount < 3)
        {
            vertexCount = 3;
        }
        List<Vector3> sectors = new List<Vector3>(vertexCount);
        float degrees = 360f / vertexCount;
        // float randomDegrees =MathUtil.random() * 360; //随机转向
        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 source = center.translateCopy(i * degrees /* + randomDegrees */, radius);
            sectors.Add(source);
        }
        return new Polygon(0, sectors, null, false);
    }

    /**
     * 获取随机点 <br>
     * 长距离获取效率低
     * @note 请勿修改返回对象数据
     */

    public override List<Vector3> getRandomPointsInPath(Vector3 center, float radius, float minDisToCenter)
    {
        int x = (int)center.x;
        int z = (int)center.z;
        int offset = (int)Math.Ceiling(radius);
        List<Vector3> targets = new List<Vector3>();
        var entrySet = this.graph.getAllRandomPointsInPath();

        foreach (var entry in entrySet)
        {
            if (Math.Abs(entry.Key - x) <= offset)
            {
                var entrySet2 = entry.Value;
                foreach (var entry2 in entrySet2)
                {
                    if (Math.Abs(entry2.Key - z) <= offset)
                    {
                        // 高度验证
                        if (graph.getPolygonData().isThreeDimensional())
                        {
                            List<Vector3> positions = entry2.Value;
                            foreach (Vector3 point in positions)
                            {
                                if (Math.Abs(center.y - point.y) < HIGH_PRECISION)
                                {
                                    targets.Add(point);
                                }
                            }
                        }
                        else
                        {
                            targets.AddRange(entry2.Value);
                        }
                    }
                }
            }
        }
        MathUtil.Shuffle(targets);
        return targets;
    }

    /**
	 * 复制有个数限制的随机点
	 * @param center
	 * @param radius
	 * @param count
	 * @return
	 */
    public List<Vector3> copyRandomPointsInPath(Vector3 center, float radius, int count)
    {
        List<Vector3> randomPointInPath = getRandomPointsInPath(center, radius, 0f);
        List<Vector3> points = new List<Vector3>();
        int n = randomPointInPath.Count() > count ? count : randomPointInPath.Count();
        for (int i = 0; i < n; i++)
        {
            points.Add(randomPointInPath.ElementAt(i).copy());
        }
        return points;
    }

    /**
     * 在整个地图中获取随机点
     * 
     * @return
     */
    public Vector3 getRandomPointInMap()
    {
        return graph.getAllPoints().ElementAt(MathUtil.Random(graph.getAllPoints().Count() - 1)).copy();
    }

    /**
     * <p>
     * 远距离效率不高
     * <p>
     */

    public override Vector3 getRandomPointInPath(Vector3 center, float radius, float minDisToCenter)
    {
        List<Vector3> list = getRandomPointsInPath(center, radius, minDisToCenter);
        Vector3 vector3 = MathUtil.Random(list);
        if (vector3 != null)
        {
            return vector3.copy();
        }
        return null;
    }


    public override float getWidth()
    {
        return graph.getPolygonData().getWidth();
    }


    public override float getHeight()
    {
        return graph.getPolygonData().getHeight();
    }


    public override int getMapId()
    {
        return graph.getPolygonData().getMapID();
    }


    public override bool isPointInPath(Vector3 point)
    {
        Polygon polygon = getPolygon(point);
        if (polygon == null)
        {
            return false;
        }
        return true;
    }


}
