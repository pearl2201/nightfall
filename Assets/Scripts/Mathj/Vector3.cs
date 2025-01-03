﻿using Pika.Base.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pika.Base.Mathj.Geometry
{
    [Serializable]
    public class Vector3 : Vector<Vector3>
    {
        private static long serialVersionUID = 1L;
        //	private static  Logger LOGGER = LoggerFactory.getLogger(Vector3.class);

        public static float TWO_PI = (float)(Math.PI * 2);
        /** 度数转弧度换算单位 */
        public static float degreesToRadians = (float)(Math.PI / 180);
        /** 弧度转度数换算单位 */
        public static float radiansToDegrees = (float)(180f / Math.PI);
        /** 零向量 */
        public static Vector3 ZERO = new Vector3();
        public static Vector3 X = new Vector3(1, 0, 0);
        public static Vector3 Y = new Vector3(0, 1, 0);
        public static Vector3 Z = new Vector3(0, 0, 1);

        // 坐标
        public volatile float x;
        public volatile float y;
        public volatile float z;

        public Vector3()
        {

        }

        public Vector3(double degree)
        {
            degree = 90 - degree;
            x = (float)Math.Cos(degreesToRadians * degree);
            z = (float)Math.Sin(degreesToRadians * degree);
        }

        public Vector3(float x, float z)
        {
            this.x = x;
            this.z = z;
        }

        public Vector3(float x, float y, float z)
        {

            this.x = x;
            this.y = y;
            this.z = z;
        }

        /**
         * 复制一个坐标
         *
         * @param old
         */
        public Vector3(Vector3 old)
        {
            this.x = old.x;
            this.y = old.y;
            this.z = old.z;
        }

        /**
         * 复制点
         *
         * @return
         */
        public Vector3 copy()
        {
            return new Vector3(x, y, z);
        }

        public float getX()
        {
            return x;
        }

        public void setX(float x)
        {
            this.x = x;
        }

        public float getY()
        {
            return y;
        }

        public void setY(float y)
        {
            this.y = y;
        }

        public float getZ()
        {
            return z;
        }

        public void setZ(float z)
        {
            this.z = z;
        }

        /**
         * 平面距离平方
         *
         * @param p
         * @return
         */
        public float dst2(Vector3 p)
        {
            return dst2(this.x, this.z, p.x, p.z);
        }

        /**
         * 平面距离平方
         *
         * @param x1
         * @param z1
         * @param x2
         * @param z2
         * @return
         */
        public static float dst2(float x1, float z1, float x2, float z2)
        {
            x1 -= x2;
            z1 -= z2;
            return (x1 * x1 + z1 * z1);
        }

        /** @return the squared distance between the given points */
        public static float dst2(float x1, float y1, float z1, float x2, float y2,
                 float z2)
        {
            float a = x2 - x1;
            float b = y2 - y1;
            float c = z2 - z1;
            return a * a + b * b + c * c;
        }

        /**
         * Returns the squared distance between this point and the given point
         * 
         * @param x
         *            The x-component of the other point
         * @param y
         *            The y-component of the other point
         * @param z
         *            The z-component of the other point
         * @return The squared distance
         */
        public float dst2(float x, float y, float z)
        {
            float a = x - this.x;
            float b = y - this.y;
            float c = z - this.z;
            return a * a + b * b + c * c;
        }

        /**
         * 平面两坐标点距离
         *
         * @param p
         * @return
         */
        public float dst(Vector3 p)
        {
            return dst(this.x, this.z, p.x, p.z);
        }


        /**
         * 两坐标距离
         * @param vector1
         * @param vector2
         * @return
         */
        public static float dst(Vector3 vector1, Vector3 vector2)
        {
            return dst(vector1.x, vector1.z, vector2.x, vector2.z);
        }

        /**
         * * 平面两坐标点距离
         *
         * @param x1
         * @param z1
         * @param x2
         * @param z2
         * @return
         */
        public static float dst(float x1, float z1, float x2, float z2)
        {
            x1 -= x2;
            z1 -= z2;
            return (float)Math.Sqrt(x1 * x1 + z1 * z1);
        }

        /** @return the distance between this point and the given point */
        public float dst(float x, float y, float z)
        {
            float a = x - this.x;
            float b = y - this.y;
            float c = z - this.z;
            return (float)Math.Sqrt(a * a + b * b + c * c);
        }

        /**
         * 相对旋转方向，以p1为参考点，当前点以p2的旋转方向
         *
         * @param p1
         * @param p2
         * @return 1 逆时针 -1 顺时针 0 三点共线
         */
        public int relCCW(Vector3 p1, Vector3 p2)
        {
            return relCCW(p1.x, p1.z, p2.x, p2.z, x, z);
        }

        /**
         * 判断坐标相对选择方向 Returns a positive float if (px, py) is counter-clockwise to (x2,
         * z2) relative to (x1, z1). in the cartesian coordinate space (positive x-axis
         * : right, positive z-axis : up). Returns a negative double if (px,
         * py) is clockwise to (x2, z2) relative to (x1, z1). Returns a 0.0 if (px, py),
         * (x1, z1) and (x2, z2) are collinear. Note that this method gives different
         * results to java.awt.geom.Line2D.relativeCCW() since Java2D uses a different
         * coordinate system (positive x-axis : right, positive z-axis extends
         * down).
         *
         * @param x1
         * @param z1
         * @param x2
         * @param z2
         * @param px
         * @param py
         * @return 0 三点共线
         */
        public static int relCCW(float x1, float z1, float x2, float z2, float px, float py)
        {
            float ccw = relCCWDouble(x1, z1, x2, z2, px, py);
            return (ccw < 0.0) ? -1 : ((ccw > 0.0) ? 1 : 0);
        }

        /**
         * 两点相对某点的旋转方向
         *
         * @param x1
         * @param z1
         * @param x2
         * @param z2
         * @param px
         * @param py
         * @return
         */
        public static float relCCWDouble(float x1, float z1, float x2, float z2, float px, float py)
        {
            x2 -= x1;
            z2 -= z1;
            px -= x1;
            py -= z1;
            float ccw = py * x2 - px * z2;
            return ccw;
        }

        /**
         * 两点相对某点的旋转方向
         *
         * @param p1
         * @param p2
         * @return
         */
        public double relCCWDouble(Vector3 p1, Vector3 p2)
        {
            return relCCWDouble(p1.x, p1.z, p2.x, p2.z, x, z);
        }

        /**
         * 当前坐标和 两坐标直线的距离平方
         *
         * @param x1
         * @param z1
         * @param x2
         * @param z2
         * @return
         */
        public double ptSegDistSq(double x1, double z1, double x2, double z2)
        {
            return ptSegDistSq(x1, z1, x2, z2, x, z);
        }

        /**
         * @ 当前坐标和 两坐标直线的距离平方
         *
         * @param start
         * @param end
         * @return
         */
        public double ptSegDistSq(Vector3 start, Vector3 end)
        {
            return ptSegDistSq(start.x, start.z, end.x, end.z, x, z);
        }

        /**
         * 点 p到线段的最近平方
         *
         * @param x1
         * @param z1
         * @param x2
         * @param z2
         * @param px
         * @param py
         * @return
         */
        public static double ptSegDistSq(double x1, double z1, double x2, double z2, double px, double py)
        {
            // from: Line2D.Float.ptSegDistSq(x1, z1, x2, z2, px, py);
            // Adjust vectors relative to x1,z1
            // x2,z2 becomes relative vector from x1,z1 to end of segment
            x2 -= x1;
            z2 -= z1;
            // px,py becomes relative vector from x1,z1 to test point
            px -= x1;
            py -= z1;
            double dotprod = px * x2 + py * z2;
            double projlenSq;
            if (dotprod <= 0.0)
            {
                // px,py is on the side of x1,z1 away from x2,z2
                // distance to segment is length of px,py vector
                // "length of its (clipped) projection" is now 0.0
                projlenSq = 0.0;
            }
            else
            {
                // switch to backwards vectors relative to x2,z2
                // x2,z2 are already the negative of x1,z1=>x2,z2
                // to get px,py to be the negative of px,py=>x2,z2
                // the dot product of two negated vectors is the same
                // as the dot product of the two normal vectors
                px = x2 - px;
                py = z2 - py;
                dotprod = px * x2 + py * z2;
                if (dotprod <= 0.0)
                {
                    // px,py is on the side of x2,z2 away from x1,z1
                    // distance to segment is length of (backwards) px,py vector
                    // "length of its (clipped) projection" is now 0.0
                    projlenSq = 0.0;
                }
                else
                {
                    // px,py is between x1,z1 and x2,z2
                    // dotprod is the length of the px,py vector
                    // projected on the x2,z2=>x1,z1 vector times the
                    // length of the x2,z2=>x1,z1 vector
                    projlenSq = dotprod * dotprod / (x2 * x2 + z2 * z2);
                }
            }
            // Distance to line is now the length of the relative point
            // vector minus the length of its projection onto the line
            // (which is zero if the projection falls outside the range
            // of the line segment).
            double lenSq = px * px + py * py - projlenSq;
            if (lenSq < 0)
            {
                lenSq = 0;
            }
            return lenSq;
        }

        /**
         * 点到执行距离的平方
         *
         * @param start
         * @param end
         * @return
         */
        public double ptLineDistSq(Vector3 start, Vector3 end)
        {
            return ptLineDistSq(start.x, start.z, end.x, end.z, x, z);
        }

        /**
         * 点到执行距离的平方
         *
         * @param x1
         * @param z1
         * @param x2
         * @param z2
         * @param px
         * @param py
         * @return
         */
        public static double ptLineDistSq(double x1, double z1, double x2, double z2, double px, double py)
        {
            // from: Line2D.Float.ptLineDistSq(x1, z1, x2, z2, px, py);
            // Adjust vectors relative to x1,z1
            // x2,z2 becomes relative vector from x1,z1 to end of segment
            x2 -= x1;
            z2 -= z1;
            // px,py becomes relative vector from x1,z1 to test point
            px -= x1;
            py -= z1;
            double dotprod = px * x2 + py * z2;
            // dotprod is the length of the px,py vector
            // projected on the x1,z1=>x2,z2 vector times the
            // length of the x1,z1=>x2,z2 vector
            double projlenSq = dotprod * dotprod / (x2 * x2 + z2 * z2);
            // Distance to line is now the length of the relative point
            // vector minus the length of its projection onto the line
            double lenSq = px * px + py * py - projlenSq;
            if (lenSq < 0)
            {
                lenSq = 0;
            }
            return lenSq;
        }

        /**
         * Unity 平移
         *
         * @param sourceDirection
         *            方向向量
         * @param degrees
         *            方向度数
         * @param distance
         * @return
         */
        public Vector3 unityTranslate(Vector3 sourceDirection, float degrees, float distance)
        {
            return unityTranslate(sourceDirection.y + degrees, distance);
        }

        /**
         * Unity 平移
         *
         * @param degrees
         *            unity角度
         * @param distance
         * @return
         */
        public Vector3 unityTranslate(float degrees, float distance)
        {
            Vector3 p = this.clone();
            double angle = unityDegreesToAngle(degrees);
            p.x = (float)(x + (distance * Math.Cos(angle)));
            p.z = (float)(z + (distance * Math.Sin(angle)));
            return p;
        }

        /**
         * 度数转弧度
         *
         * @param degrees
         * @return
         */
        public static double unityDegreesToAngle(double degrees)
        {
            return ((90 - degrees) % 360) * Math.PI / 180;
        }

        /**
         * 移动并返回新对象
         *
         * @param degrees
         * @param radius
         * @return
         */
        public Vector3 translateCopy(double degrees, double radius)
        {
            Vector3 p = this.clone();
            double angle = degrees * Math.PI / 180;
            p.x = (float)(x + (radius * Math.Cos(angle)));
            p.z = (float)(z + (radius * Math.Sin(angle)));
            return p;
        }


        public Vector3 clone()
        {
            try
            {
                return (Vector3)this.clone();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException();
            }
        }


        public String toString()
        {
            return "{" + "x=" + x + ", y=" + y + ", z=" + z + '}';
        }

        /**
         * 2D 平面距离平方
         *
         * @param point
         * @return
         */
        public double distanceSqlt2D(Vector3 point)
        {
            double a = point.x - x;
            double c = point.z - z;
            return a * a + c * c;
        }

        public static Vector3 getClosestPointOnSegment(float x1, float z1, float x2, float z2, float px, float py)
        {
            Vector3 closestPoint = new Vector3();
            float x2LessX1 = x2 - x1;
            float y2LessY1 = z2 - z1;
            float lNum = x2LessX1 * x2LessX1 + y2LessY1 * y2LessY1;
            float rNum = ((px - x1) * x2LessX1 + (py - z1) * y2LessY1) / lNum;
            // double lNum = (x2 - x1)*(x2 - x1) + (z2 - z1)*(z2 - z1);
            // double rNum = ((px - x1)*(x2 - x1) + (py - z1)*(z2 - z1)) / lNum;
            if (rNum <= 0)
            {
                closestPoint.x = x1;
                closestPoint.z = z1;
            }
            else if (rNum >= 1)
            {
                closestPoint.x = x2;
                closestPoint.z = z2;
            }
            else
            {
                closestPoint.x = (x1 + rNum * x2LessX1);
                closestPoint.z = (z1 + rNum * y2LessY1);
            }
            return closestPoint;
        }

        /**
         * 两坐标的夹角
         *
         * @param dest
         * @return
         */
        public float findAngle(Vector3 dest)
        {
            return findAngle(this, dest);
        }

        /**
         * 两坐标的夹角
         *
         * @param start
         * @param dest
         * @return
         */
        public static float findAngle(Vector3 start, Vector3 dest)
        {
            return findAngle(start.x, start.z, dest.x, dest.z);
        }

        /**
         * 两坐标的夹角
         *
         * @param x1
         * @param z1
         * @param x2
         * @param z2
         * @return
         */
        public static float findAngle(float x1, float z1, float x2, float z2)
        {
            float angle = findSignedAngle(x1, z1, x2, z2);
            if (angle < 0)
            {
                angle += TWO_PI;
            }
            return angle;
        }

        /**
         * 两坐标的夹角
         *
         * @param x1
         * @param z1
         * @param x2
         * @param z2
         * @return
         */
        public static float findSignedAngle(float x1, float z1, float x2, float z2)
        {
            float x = x2 - x1;
            float z = z2 - z1;
            float angle = (float)(Math.Atan2(z, x));
            return angle;
        }

        /**
         * 根据度数和距离创建新坐标
         *
         * @param angle
         * @param distance
         * @return
         */
        public Vector3 createPointFromAngle(float angle, float distance)
        {
            return createPointFromAngle(this, angle, distance);
        }

        /**
         * 根据度数和距离创建新坐标
         *
         * @param source
         * @param angle
         * @param distance
         * @return
         */
        public static Vector3 createPointFromAngle(Vector3 source, float angle, float distance)
        {
            Vector3 p = new Vector3();
            float xDist = (float)(Math.Cos(angle) * distance);
            float yDist = (float)(Math.Sin(angle) * distance);
            p.x = (source.x + xDist);
            p.z = (source.z + yDist);
            p.y = source.y;
            return p;
        }

        /**
         * 是否在扇形中
         *
         * @param sourceDirection
         * @param target
         * @param radius
         * @param degrees
         * @return
         */
        public bool isInSector(Vector3 sourceDirection, Vector3 target, float radius, float degrees)
        {
            if (this.Equals(target))
            {
                return true;
            }
            double dis = this.dst2(target);
            Vector3 vt = target.sub(this);
            vt.y = 0;
            if (vt.isZero())
            {
                return true;
            }
            double targetAngle = angle(vt, new Vector3(sourceDirection.y));
            return (dis <= radius * radius && targetAngle <= degrees / 2 && targetAngle >= -(degrees / 2));
        }

        /**
         * 返回两个向量（从原点到两点）间的夹角
         *
         * @param v1
         * @param v2
         * @return Degrees
         */
        public static double angle(Vector3 v1, Vector3 v2)
        {
            if (v1.isZero())
            {
                throw new InvalidOperationException("v1为零向量");
            }
            if (v2.isZero())
            {
                throw new InvalidOperationException("v2为零向量");
            }
            return ((Math.Acos(v1.dot(v2) / (v1.len() * v2.len()))) * radiansToDegrees);
        }

        /**
         * 点乘
         * <p>
         * 判断两个向量是否垂直;计算一个向量在某个方向上的投影长度,角度计算
         * </p>
         *
         * @param vector
         * @return
         */
        public float dot(Vector3 vector)
        {
            return x * vector.x + y * vector.y + z * vector.z;
        }

        /**
         * 点乘
         * 
         * @return The dot product between the two vectors
         */
        public static float dot(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            return x1 * x2 + y1 * y2 + z1 * z2;
        }

        /**
         * Returns the dot product between this and the given vector.
         * 
         * @param x
         *            The x-component of the other vector
         * @param y
         *            The y-component of the other vector
         * @param z
         *            The z-component of the other vector
         * @return The dot product
         */
        public float dot(float x, float y, float z)
        {
            return this.x * x + this.y * y + this.z * z;
        }

        /**
         * 向量长度
         *
         * @return
         */
        public float len()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        /** @return The euclidean length */
        public static float len(float x, float y, float z)
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        /**
         * 2D平面长度
         * 
         * @return
         */
        public float len2D()
        {
            return (float)Math.Sqrt(x * x + z * z);
        }

        /**
         * 坐标相减
         *
         * @param a_vec
         * @return
         */
        public Vector3 sub(Vector3 a_vec)
        {
            return this.sub(a_vec.x, a_vec.y, a_vec.z);
        }

        /**
         * 坐标相减
         *
         * @param x
         * @param y
         * @param z
         * @return
         */
        public Vector3 sub(float x, float y, float z)
        {
            return this.set(this.x - x, this.y - y, this.z - z);
        }


        /**
         * 坐标相减
         * 
         */
        public static Vector3 sub(Vector3 left, Vector3 right)
        {
            return new Vector3(left.x - right.x, left.y - right.y, left.z - right.z);
        }

        /**
         * x轴加一个常量
         * 
         * @param x
         */
        public void addX(float x)
        {
            this.x += x;
        }

        public void addY(float y)
        {
            this.y += y;
        }

        public void addZ(float z)
        {
            this.z += z;
        }

        /**
         * 是否为0向量
         *
         * @return
         */
        public bool isZero()
        {
            return x == 0 && y == 0 && z == 0;
        }


        public Vector3 setZero()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            return this;
        }


        public Vector3 cpy()
        {
            return copy();
        }

        /**
         * 线段是否相交
         * @param p1
         * @param p2
         * @param p3
         * @param p4
         * @return
         */
        public static bool linesIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            return linesIntersect(p1.x, p1.z, p2.x, p2.z, p3.x, p3.z, p4.x, p4.z);
        }

        /**
         * 线段是否相交
         * @param x1
         * @param z1
         * @param x2
         * @param z2
         * @param x3
         * @param y3
         * @param x4
         * @param y4
         * @return
         */
        public static bool linesIntersect(double x1, double z1, double x2, double z2, double x3, double y3, double x4,
                double y4)
        {
            // Return false if either of the lines have zero length
            if (x1 == x2 && z1 == z2 || x3 == x4 && y3 == y4)
            {
                return false;
            }
            // Fastest method, based on Franklin Antonio's "Faster Line Segment
            // Intersection" topic "in Graphics Gems III" book
            // (http://www.graphicsgems.org/)
            double ax = x2 - x1;
            double ay = z2 - z1;
            double bx = x3 - x4;
            double by = y3 - y4;
            double cx = x1 - x3;
            double cy = z1 - y3;

            double alphaNumerator = by * cx - bx * cy;
            double commonDenominator = ay * bx - ax * by;
            if (commonDenominator > 0)
            {
                if (alphaNumerator < 0 || alphaNumerator > commonDenominator)
                {
                    return false;
                }
            }
            else if (commonDenominator < 0)
            {
                if (alphaNumerator > 0 || alphaNumerator < commonDenominator)
                {
                    return false;
                }
            }
            double betaNumerator = ax * cy - ay * cx;
            if (commonDenominator > 0)
            {
                if (betaNumerator < 0 || betaNumerator > commonDenominator)
                {
                    return false;
                }
            }
            else if (commonDenominator < 0)
            {
                if (betaNumerator > 0 || betaNumerator < commonDenominator)
                {
                    return false;
                }
            }
            // if commonDenominator == 0 then the lines are parallel.
            if (commonDenominator == 0)
            {
                // This code wasn't in Franklin Antonio's method. It was added by Keith
                // Woodward.
                // The lines are parallel.
                // Check if they're collinear.
                double collinearityTestForP3 = x1 * (z2 - y3) + x2 * (y3 - z1) + x3 * (z1 - z2); // see
                                                                                                 // http://mathworld.wolfram.com/Collinear.html
                                                                                                 // If p3 is collinear with p1 and p2 then p4 will also be collinear, since p1-p2
                                                                                                 // is parallel with p3-p4
                if (collinearityTestForP3 == 0)
                {
                    // The lines are collinear. Now check if they overlap.
                    if (x1 >= x3 && x1 <= x4 || x1 <= x3 && x1 >= x4 || x2 >= x3 && x2 <= x4 || x2 <= x3 && x2 >= x4
                            || x3 >= x1 && x3 <= x2 || x3 <= x1 && x3 >= x2)
                    {
                        if (z1 >= y3 && z1 <= y4 || z1 <= y3 && z1 >= y4 || z2 >= y3 && z2 <= y4 || z2 <= y3 && z2 >= y4
                                || y3 >= z1 && y3 <= z2 || y3 <= z1 && y3 >= z2)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            return true;

        }


        public Vector3 add(Vector3 vector)
        {
            return this.add(vector.x, vector.y, vector.z);
        }

        /**
         * Adds the given vector to this component
         * 
         * @param x
         *            The x-component of the other vector
         * @param y
         *            The y-component of the other vector
         * @param z
         *            The z-component of the other vector
         * @return This vector for chaining.
         */
        public Vector3 add(float x, float y, float z)
        {
            return this.set(this.x + x, this.y + y, this.z + z);
        }

        /**
         * Sets the vector to the given components
         *
         * @param x
         *            The x-component
         * @param y
         *            The y-component
         * @param z
         *            The z-component
         * @return this vector for chaining
         */
        public Vector3 set(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            return this;
        }


        public Vector3 set(Vector3 vector)
        {
            return this.set(vector.x, vector.y, vector.z);
        }


        public Vector3 scl(float scalar)
        {
            return this.set(this.x * scalar, this.y * scalar, this.z * scalar);
        }


        public Vector3 scl(Vector3 other)
        {
            return this.set(x * other.x, y * other.y, z * other.z);
        }

        /**
         * Sets this vector to the cross product between it and the other vector. <br>
         * 叉乘更多的是判断某个平面的方向。从这个平面上选两个不共线的向量，叉乘的结果就是这个平面的法向量
         * 
         * @param vector
         *            The other vector
         * @return This vector for chaining
         */
        public Vector3 crs(Vector3 vector)
        {
            return this.set(y * vector.z - z * vector.y, z * vector.x - x * vector.z, x * vector.y - y * vector.x);
        }

        /**
         * 叉乘<br>
         * Sets this vector to the cross product between it and the other vector.
         * 
         * @param x
         *            The x-component of the other vector
         * @param y
         *            The y-component of the other vector
         * @param z
         *            The z-component of the other vector
         * @return This vector for chaining
         */
        public Vector3 crs(float x, float y, float z)
        {
            return this.set(this.y * z - this.z * y, this.z * x - this.x * z, this.x * y - this.y * x);
        }


        public Vector3 nor()
        {
            float len2 = this.len2();
            if (len2 == 0f || len2 == 1f)
            {
                return this;
            }
            return this.scl(1f / (float)Math.Sqrt(len2));
        }

        /**
         * 2D平面单位向量
         * 
         * @return
         */
        public void nor2D()
        {
            float length = len2D();
            this.x /= length;
            this.z /= length;
        }


        public float len2()
        {
            return x * x + y * y + z * z;
        }

        /**
         * 叉积x z轴 <br>
         * http://www.yalewoo.com/in_triangle_test.html
         * 
         * @param fromPoint
         * @param toPoint
         * @param p
         * @return
         */
        public static float cross2D(Vector3 fromPoint, Vector3 toPoint, Vector3 p)
        {
            return (toPoint.x - fromPoint.x) * (p.z - fromPoint.z) - (toPoint.z - fromPoint.z) * (p.x - fromPoint.x);
        }

        /**
         * 点是否在线段的左侧 <br>
         * http://www.yalewoo.com/in_triangle_test.html
         * 
         * @param fromPoint
         * @param toPoint
         * @param p
         * @return
         */
        public static bool pointInLineLeft(Vector3 fromPoint, Vector3 toPoint, Vector3 p)
        {
            return cross2D(fromPoint, toPoint, p) > 0;
        }

        /**
         * Left-multiplies the vector by the given matrix, assuming the fourth (w)
         * component of the vector is 1.
         * 
         * @param matrix
         *            The matrix
         * @return This vector for chaining
         */
        public Vector3 mul(Matrix4 matrix)
        {
            float[] l_mat = matrix.val;
            return this.set(x * l_mat[Matrix4.M00] + y * l_mat[Matrix4.M01] + z * l_mat[Matrix4.M02] + l_mat[Matrix4.M03],
                    x * l_mat[Matrix4.M10] + y * l_mat[Matrix4.M11] + z * l_mat[Matrix4.M12] + l_mat[Matrix4.M13],
                    x * l_mat[Matrix4.M20] + y * l_mat[Matrix4.M21] + z * l_mat[Matrix4.M22] + l_mat[Matrix4.M23]);
        }

        /** Left-multiplies the vector by the given matrix.
         * @param matrix The matrix
         * @return This vector for chaining */
        public Vector3 mul(Matrix3 matrix)
        {
            float[] l_mat = matrix.val;
            return set(x * l_mat[Matrix3.M00] + y * l_mat[Matrix3.M01] + z * l_mat[Matrix3.M02], x * l_mat[Matrix3.M10] + y
                    * l_mat[Matrix3.M11] + z * l_mat[Matrix3.M12], x * l_mat[Matrix3.M20] + y * l_mat[Matrix3.M21] + z * l_mat[Matrix3.M22]);
        }


        public bool equal(Vector3 vector3, float precision)
        {
            if (Math.Abs(this.x - vector3.x) < precision && Math.Abs(this.y - vector3.y) < precision
                    && Math.Abs(this.z - vector3.z) < precision)
            {
                return true;
            }
            return false;
        }

        /**
         * siteIsEqual
         * @param vector3
         * @return
         */
        public bool equal(Vector3 vector3)
        {
            return this.equal(vector3, 0.0001f);
        }

        /**
         * 清空坐标，设为0
         */
        public void clear()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        /**
         * 根据两点组成的直线在xz平面的角度，顺时针
         *
         * @param start
         * @param end
         * @return
         */
        public static float toUnityDegrees(Vector3 start, Vector3 end)
        {
            return Vector3.toUnityDegrees(findAngle(start, end));
        }

        /**
         * 方向向量转
         * @param direction
         * @return
         */
        public static float toUnityDegree(Vector3 direction)
        {
            return (float)Math.Atan2(direction.x, direction.z);
        }

        /**
         * 度数转方向向量
         * @param degree
         * @return
         */
        public Vector3 toUnityDirection(float degree)
        {
            float rad = degree / MathUtil.degreesToRadians;
            return new Vector3((float)Math.Sin(rad), (float)Math.Cos(rad));
        }

        /**
         * java弧度转unity角度
         *
         * @param angle
         * @return
         */
        public static float toUnityDegrees(float angle)
        {
            return (float)(90 - MathUtil.radiansToDegrees * (angle));
        }


        public Vector3 mulAdd(Vector3 vec, float scalar)
        {
            this.x += vec.x * scalar;
            this.y += vec.y * scalar;
            this.z += vec.z * scalar;
            return this;
        }

        /**
         * 求两向量的方向向量
         * 
         * @param from
         * @param to
         * @return
         */
        public static Vector3 dirVector(Vector3 from, Vector3 to)
        {
            Vector3 vector3 = new Vector3(to.x - from.x, to.y - from.y, to.z - from.z);
            return vector3;
        }

        /**
         * 两向量是否靠近
         * 
         * @param vector3
         * @param maxDistance
         *            最大距离
         * @return true 接近 false 远离
         */
        public bool isColoseTo(Vector3 vector3, float maxDistance)
        {
            float distSq = this.x * vector3.x + this.y * vector3.y + this.z * vector3.z;
            return distSq <= maxDistance * maxDistance;
        }


        public Vector3 lerp(Vector3 target, float alpha)
        {
            x += alpha * (target.x - x);
            y += alpha * (target.y - y);
            z += alpha * (target.z - z);
            return this;
        }

        /**
         * Limits the length of this vector, based on the desired maximum length.
         * @param limit
         * @return
         */
        public Vector3 limit(float limit)
        {
            return limit2(limit * limit);
        }

        /**
         * imits the length of this vector, based on the desired maximum length squared.
         * This method is slightly faster than limit().
         * @param limit2
         * @return
         */
        public Vector3 limit2(float limit2)
        {
            float len2 = len2D();
            if (len2 > limit2)
            {
                scl((float)Math.Sqrt(limit2 / len2));
            }
            return this;
        }


        public bool isZero(float margin)
        {
            return len2() < margin;
        }


        public bool epsilonEquals(Vector3 other, float epsilon)
        {
            if (other == null) return false;
            if (Math.Abs(other.x - x) > epsilon) return false;
            if (Math.Abs(other.y - y) > epsilon) return false;
            if (Math.Abs(other.z - z) > epsilon) return false;
            return true;
        }

        public bool Equals(Vector3 other)
        {
            return this.x == other.x && this.y == other.y && this.z == other.z;
        }
    }
}
