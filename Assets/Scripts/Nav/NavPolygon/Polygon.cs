using Newtonsoft.Json;
using Pika.Base.Mathj;
using Pika.Base.Mathj.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Pika.Ai.Nav.NavPolygon
{
    /**
     * 多边形,用于navmesh寻路
     * 
     * @author JiangZhiYong
     * @date 2018年2月20日
     * @mail 359135103@qq.com
     */
    public class Polygon
    {
        //	private const Logger LOGGER = LoggerFactory.getLogger(Polygon.class);

        /** 多边形序号 */
        public int index;
        /** 顶点坐标 */
        public List<Vector3> points;
        /** y轴，所有顶点平均高度 */
        public float y;
        /** 中心坐标 */
        public Vector3 center;
        /** 面积 */
        public float area;
        /** 逆时针方向 */
        public bool counterClockWise;
        /** 凸多边形 */
        public bool convex;
        /** 半径 */
        public float radius;
        /** 半径平方 */
        public float radiusSq;
        /** 顶点坐标索引 */
        public int[] vectorIndexs;
        /** 和其他多边形的共享变 */
        public List<Connection<Polygon>> connections;
        /** 预先生成的随机点 */
        public List<Vector3> randomPoints = new List<Vector3>();

        public Polygon(int index, List<Vector3> points, int[] vectorIndexs)
        {
            this.index = index;
            this.vectorIndexs = vectorIndexs;
            this.points = points;
            this.connections = new List<Connection<Polygon>>();
            initCalculate();
        }

        /**
         * 
         * @param index
         *            寻路索引编号
         * @param points
         *            坐标点
         * @param vectorIndexs
         *            坐标顶点序号
         * @param initCalculate 初始化计算       
         */
        public Polygon(int index, List<Vector3> points, int[] vectorIndexs, bool initCalculate)
        {
            this.index = index;
            this.vectorIndexs = vectorIndexs;
            this.points = points;
            if (initCalculate)
            {
                this.initCalculate();
            }
        }

        public Polygon(int index, params Vector3[] point) : this(index, point.ToList(), null)
        {

        }

        public Polygon(List<Vector3> points) : this(0, points, null)
        {

        }

        /**
         * 初始化计算
         */
        private void initCalculate()
        {
            calculateArea();
            calculateCenter();
            calculateRadius();
            calculateIsConvex();
            // y坐标
            foreach (Vector3 point in points)
            {
                y += point.y;
            }
            y = y / points.Count();
        }

        /**
         * 计算中心坐标
         */
        public void calculateCenter()
        {
            if (center == null)
            {
                center = new Vector3();
            }
            if (getArea() == 0)
            {
                center.x = points.ElementAt(0).x;
                center.z = points.ElementAt(0).z;
                center.y = points.ElementAt(0).y;
                return;
            }
            float cx = 0.0f;
            float cz = 0.0f;
            float cy = 0.0f; // 取坐标平均值
            Vector3 pointIBefore = (!(points.Count == 0) ? points.ElementAt(points.Count() - 1) : null);
            for (int i = 0; i < points.Count(); i++)
            {
                Vector3 pointI = points.ElementAt(i);
                double multiplier = (pointIBefore.z * pointI.x - pointIBefore.x * pointI.z);
                cx += (float)((pointIBefore.x + pointI.x) * multiplier);
                cz += (float)((pointIBefore.z + pointI.z) * multiplier);
                pointIBefore = pointI;
                cy += pointI.y;
            }
            cx /= (6 * getArea());
            cz /= (6 * getArea());
            if (counterClockWise == true)
            {
                cx *= -1;
                cz *= -1;
            }
            center.x = cx;
            center.z = cz;
            center.y = cy / points.Count();
        }

        /**
         * 计算半径
         */
        public void calculateRadius()
        {
            if (center == null)
            {
                calculateCenter();
            }
            double maxRadiusSq = -1;
            int furthestPointIndex = 0;
            for (int i = 0; i < points.Count(); i++)
            {
                double currentRadiusSq = (center.dst2(points.ElementAt(i)));
                if (currentRadiusSq > maxRadiusSq)
                {
                    maxRadiusSq = currentRadiusSq;
                    furthestPointIndex = i;
                }
            }
            radius = (center.dst(points.ElementAt(furthestPointIndex)));
            radiusSq = radius * radius;
        }

        /**
         * 计算面积
         */
        public void calculateArea()
        {
            float signedArea = getAndCalcSignedArea();
            if (signedArea < 0)
            {
                counterClockWise = false;
            }
            else
            {
                counterClockWise = true;
            }
            area = (float)Math.Abs(signedArea);
        }

        /**
         * 计算面积
         * 
         * @return 小于0坐标点顺时针排序，大于0逆时针
         */
        public float getAndCalcSignedArea()
        {
            float totalArea = 0;
            for (int i = 0; i < points.Count() - 1; i++)
            {
                totalArea += ((points.ElementAt(i).x - points.ElementAt(i + 1).x)
                        * (points.ElementAt(i + 1).z + (points.ElementAt(i).z - points.ElementAt(i + 1).z) / 2));
            }
            // need to do points[point.Length-1] and points[0].
            totalArea += ((points.ElementAt(points.Count() - 1).x - points.ElementAt(0).x)
                    * (points.ElementAt(0).z + (points.ElementAt(points.Count() - 1).z - points.ElementAt(0).z) / 2));
            return totalArea;
        }

        /**
         * 计算是否为凸多边形
         * 
         * @return
         */
        public bool calculateIsConvex()
        {
            int size = points.Count();
            for (int i = 0; i < size; i++)
            {
                Vector3 point1 = getPoint(i % size); // 前一顶点
                Vector3 point2 = getPoint((i + 1) % size); // 中间顶点
                Vector3 point3 = getPoint((i + 2) % size); // 后一顶点
                int relCCW = point2.relCCW(point1, point3);
                // 凹点
                if ((counterClockWise && relCCW > 0) || (!counterClockWise && relCCW < 0))
                {
                    convex = false;
                    return convex;
                }
            }
            convex = true;
            return convex;
        }

        public float getArea()
        {
            return area;
        }

        public int getIndex()
        {
            return index;
        }

        /**
         * 获取坐标点
         * 
         * @param i
         *            坐标序号
         * @return
         */
        public Vector3 getPoint(int i)
        {
            return points.ElementAt(i);
        }

        /**
         * 坐标点是否在多边形内部
         * 
         * @param point
         * @return
         */
        public bool isInnerPoint(Vector3 point)
        {
            return Intersector.isPointInPolygon(points, point);
        }

        /**
         * 是否包含另外一个多边形
         * 
         * @param polygon
         * @return
         */
        public bool contains(Polygon polygon)
        {
            if (intersectsEdge(polygon))
            {
                return false;
            }

            if (contains(polygon.getPoint(0)) == false)
            {
                return false;
            }
            return true;
        }

        /**
         * 坐标点是否在多边形内部 <br>
         * 比{@code isInnerPoint}速度慢
         * 
         * @param p
         * @return
         */
        public bool contains(Vector3 p)
        {
            return Contains(p.x, p.z);
        }

        /**
         * 两多边形的边是否相交
         * 
         * @param polygon
         * @return
         */
        public bool intersectsEdge(Polygon polygon)
        {
            Vector3 pointIBefore = (points.Count() != 0 ? points.ElementAt(points.Count() - 1) : null);
            Vector3 pointJBefore = (polygon.points.Count() != 0 ? polygon.points.ElementAt(polygon.points.Count() - 1) : null);
            for (int i = 0; i < points.Count(); i++)
            {
                Vector3 pointI = points.ElementAt(i);
                for (int j = 0; j < polygon.points.Count(); j++)
                {
                    Vector3 pointJ = polygon.points.ElementAt(j);
                    // The below linesIntersect could be sped up slightly since many things are
                    // recalc'ed over and over again.
                    if (Vector3.linesIntersect(pointI, pointIBefore, pointJ, pointJBefore))
                    {
                        return true;
                    }
                    pointJBefore = pointJ;
                }
                pointIBefore = pointI;
            }
            return false;
        }


        public override string ToString()
        {
            return "Polygon [index=" + index + ", points=" + points + ", y=" + y + ", center=" + center + ", area=" + area
                    + ", counterClockWise=" + counterClockWise + ", convex=" + convex + ", radius=" + radius + ", radiusSq="
                    + radiusSq + ", vectorIndexs=" + string.Join(",", vectorIndexs) + "]";
        }


        public Rectangle getBounds()
        {
            throw new NotSupportedException();
        }


        public Rectangle2D getBounds2D()
        {
            throw new NotSupportedException();
        }


        public bool Contains(float x, float z)
        {
            Vector3 pointIBefore = (points.Count() != 0 ? points.ElementAt(points.Count() - 1) : null);
            int crossings = 0;
            for (int i = 0; i < points.Count(); i++)
            {
                Vector3 pointI = points.ElementAt(i);
                if (((pointIBefore.z <= z && z < pointI.z) || (pointI.z <= z && z < pointIBefore.z))
                        && x < ((pointI.x - pointIBefore.x) / (pointI.z - pointIBefore.z) * (z - pointIBefore.z)
                                + pointIBefore.x))
                {
                    crossings++;
                }
                pointIBefore = pointI;
            }
            return (crossings % 2 != 0);
        }


        public bool contains(Point2D p)
        {
            return this.Contains(p.X, p.Y);
        }


        public bool intersects(double x, double y, double w, double h)
        {
            throw new NotSupportedException();
        }


        public bool intersects(Rectangle2D r)
        {
            throw new NotSupportedException();
        }


        public bool contains(double x, double y, double w, double h)
        {
            throw new NotSupportedException();
        }


        public bool contains(Rectangle2D r)
        {
            throw new NotSupportedException();
        }


        public PathIterator getPathIterator(AffineTransform at)
        {
            return new PolygonIterator(this, at);
        }


        public PathIterator getPathIterator(AffineTransform at, double flatness)
        {
            return new PolygonIterator(this, at);
        }

        public String print()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\n");
            sb.Append("序号:" + index);
            sb.Append("\r\n");
            sb.Append("坐标:" + points.ToString());
            sb.Append("\r\n");
            sb.Append("顶点序号:" + JsonConvert.SerializeObject(vectorIndexs));
            return sb.ToString();
        }

        public class PolygonIterator : PathIterator
        {


            int type = PathIterator.SEG_MOVETO;
            int index = 0;
            Polygon polygon;
            Vector3 currentPoint;
            AffineTransform affine;

            double[]
            singlePointSetDouble = new double[2];

            public PolygonIterator(Polygon polygon) : this(polygon, null)
            {

            }

            public PolygonIterator(Polygon kPolygon, AffineTransform at)
            {
                this.polygon = kPolygon;
                this.affine = at;
                currentPoint = polygon.getPoint(0);
            }

            public override int getWindingRule()
            {
                return PathIterator.WIND_EVEN_ODD;
            }


            public override bool isDone()
            {
                if (index == polygon.points.Count() + 1)
                {
                    return true;
                }
                return false;
            }


            public override void next()
            {
                index++;
            }

            public void assignPointAndType()
            {
                if (index == 0)
                {
                    currentPoint = polygon.getPoint(0);
                    type = PathIterator.SEG_MOVETO;
                }
                else if (index == polygon.points.Count())
                {
                    type = PathIterator.SEG_CLOSE;
                }
                else
                {
                    currentPoint = polygon.getPoint(index);
                    type = PathIterator.SEG_LINETO;
                }
            }


            public override int currentSegment(float[] coords)
            {
                assignPointAndType();
                if (type != PathIterator.SEG_CLOSE)
                {
                    if (affine != null)
                    {
                        float[] singlePointSetFloat = new float[2];
                        singlePointSetFloat[0] = (float)currentPoint.x;
                        singlePointSetFloat[1] = (float)currentPoint.z;
                        affine.transform(singlePointSetFloat, 0, coords, 0, 1);
                    }
                    else
                    {
                        coords[0] = (float)currentPoint.x;
                        coords[1] = (float)currentPoint.z;
                    }
                }
                return type;
            }


            public override int currentSegment(double[] coords)
            {
                assignPointAndType();
                if (type != PathIterator.SEG_CLOSE)
                {
                    if (affine != null)
                    {
                        singlePointSetDouble[0] = currentPoint.x;
                        singlePointSetDouble[1] = currentPoint.z;
                        affine.transform(singlePointSetDouble.Cast<float>().ToArray(), 0, coords.Cast<float>().ToArray(), 0, 1);
                    }
                    else
                    {
                        coords[0] = currentPoint.x;
                        coords[1] = currentPoint.z;
                    }
                }
                return type;
            }
        }

    }
}
