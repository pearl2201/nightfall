

using Pika.Ai.Nav;
using Pika.Base.Mathj.Geometry;
using Pika.Base.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


/**
 * 三角形
 * 
 * @author JiangZhiYong
 * @QQ 359135103 2017年11月7日 下午4:41:27
 */
public class Triangle
{
    /** 三角形序号 */
    public int index;
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;
    public float y; //三角形高度，三个顶点的平均高度
    /** 中点 */
    public Vector3 center;
    /** 三角形和其他三角形的共享边 */
    public List<Connection<Triangle>> connections;
    /**三角形顶点序号*/
    public int[] vectorIndex;

    public Triangle(Vector3 a, Vector3 b, Vector3 c, int index, params int[] vectorIndex)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.y = (a.y + b.y + c.y) / 3;
        this.index = index;
        this.center = new Vector3(a).add(b).add(c).scl(1f / 3f);
        this.connections = new List<Connection<Triangle>>();
        this.vectorIndex = vectorIndex;
    }


    public override string ToString()
    {
        return "Triangle [index=" + index + ", a=" + a + ", b=" + b + ", c=" + c + ", center=" + center + "]";
    }

    public int getIndex()
    {
        return this.index;
    }

    public List<Connection<Triangle>> getConnections()
    {
        return connections;
    }

    /**
	 * Calculates the angle in radians between a reference vector and the (plane)
	 * normal of the triangle.
	 *
	 * @param reference
	 * @return
	 */
    public float getAngle(Vector3 reference)
    {
        float x = reference.x;
        float y = reference.y;
        float z = reference.z;
        Vector3 normal = reference;
        normal.set(a).sub(b).crs(b.x - c.x, b.y - c.y, b.z - c.z).nor();
        float angle = (float)Math.Acos(normal.dot(x, y, z) / (normal.len() * Math.Sqrt(x * x + y * y + z * z)));
        reference.set(x, y, z);
        return angle;
    }

    /**
	 * Calculates a random point in this triangle.
	 *
	 * @param out
	 *            Output vector
	 * @return Output for chaining
	 */
    public Vector3 getRandomPoint(Vector3 outV)
    {
        float sr1 = (float)Math.Sqrt(MathUtil.Random());
        float r2 = MathUtil.Random();
        float k1 = 1 - sr1;
        float k2 = sr1 * (1 - r2);
        float k3 = sr1 * r2;
        outV.x = k1 * a.x + k2 * b.x + k3 * c.x;
        outV.y = k1 * a.y + k2 * b.y + k3 * c.y;
        outV.z = k1 * a.z + k2 * b.z + k3 * c.z;
        return outV;
    }

    /**
	 * Calculates the area of the triangle.
	 *
	 * @return
	 */
    public float area()
    {
        float abx = b.x - a.x;
        float aby = b.y - a.y;
        float abz = b.z - a.z;
        float acx = c.x - a.x;
        float acy = c.y - a.y;
        float acz = c.z - a.z;
        float r = aby * acz - abz * acy;
        float s = abz * acx - abx * acz;
        float t = abx * acy - aby * acx;
        return 0.5f * (float)Math.Sqrt(r * r + s * s + t * t);
    }

    /**
	 * 三角形2D平面面积
	 * @return
	 */
    public float area2D()
    {
        float abx = b.x - a.x;
        float abz = b.z - a.z;
        float acx = c.x - a.x;
        float acz = c.z - a.z;
        return acx * abz - abx * acz;
    }


    /**
	 * 判断一个点是否在三角形内,二维判断
	 * <br> http://www.yalewoo.com/in_triangle_test.html
	 */
    public bool isInnerPoint(Vector3 point)
    {
        bool res = Vector3.pointInLineLeft(a, b, point);
        if (res != Vector3.pointInLineLeft(b, c, point))
        {
            return false;
        }
        if (res != Vector3.pointInLineLeft(c, a, point))
        {
            return false;
        }
        if (Vector3.cross2D(a, b, c) == 0)
        {   //三点共线
            return false;
        }

        return true;
    }


    public override int GetHashCode()
    {
        int prime = 31;
        int result = 1;
        result = prime * result + index;
        return result;
    }


    public override bool Equals(Object obj)
    {
        if (this == obj)
            return true;
        if (obj == null)
            return false;
        if (this.GetType() != obj.GetType())
            return false;
        Triangle other = (Triangle)obj;
        if (index != other.index)
            return false;
        return true;
    }


    public Pika.Ai.Nav.Rectangle getBounds()
    {
        throw new NotSupportedException();
    }


    public Rectangle2D getBounds2D()
    {
        throw new NotSupportedException();
    }


    public bool contains(double x, double y)
    {
        throw new NotSupportedException();
    }


    public bool contains(Point2D p)
    {
        throw new NotSupportedException();
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
        return new TrianglePathIterator(this, at);
    }


    public PathIterator getPathIterator(AffineTransform at, double flatness)
    {
        return new TrianglePathIterator(this, at);
    }

    public class TrianglePathIterator : PathIterator
    {

        int type = PathIterator.SEG_MOVETO;
        int index = 0;
        Triangle triangle;
        Vector3 currentPoint;
        AffineTransform affine;

        double[] singlePointSetDouble = new double[2];

        public TrianglePathIterator(Triangle triangle) : this(triangle, null)
        {

        }

        public TrianglePathIterator(Triangle triangle, AffineTransform at)
        {
            this.triangle = triangle;
            this.affine = at;
            currentPoint = triangle.a;
        }

        public override int getWindingRule()
        {
            return PathIterator.WIND_EVEN_ODD;
        }


        public override bool isDone()
        {
            if (index == 4)
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
                currentPoint = triangle.a;
                type = PathIterator.SEG_MOVETO;
            }
            else if (index == 3)
            {
                type = PathIterator.SEG_CLOSE;
            }
            else
            {
                if (index == 0)
                {
                    currentPoint = triangle.a;
                }
                else if (index == 1)
                {
                    currentPoint = triangle.b;
                }
                else
                {
                    currentPoint = triangle.c;
                }
                //                currentPoint = polygon.getPoint(index);
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
