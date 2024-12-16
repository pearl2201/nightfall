using Pika.Base.Mathj.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Pika.Ai.Quadtree
{

    /**
     * 坐标点数据结构
     * 
     * @author JiangZhiYong
     * @mail 359135103@qq.com
     */
    public class PointData<T> : Data<T>
    {

        private Vector3 point;

        public PointData(Vector3 point, T value) : base(value)
        {
            this.point = point;
        }

        public Vector3 getPoint()
        {
            return point;
        }

        public void setPoint(Vector3 point)
        {
            this.point = point;
        }


        public override int CompareTo(Data<T> data)
        {
            PointData<T> o = (PointData<T>)data;
            if (this.point.x < o.point.x)
            {
                return -1;
            }
            else if (this.point.x > o.point.x)
            {
                return 1;
            }
            else
            {
                if (this.point.z < o.point.z)
                {
                    return -1;
                }
                else if (this.point.z > o.point.z)
                {
                    return 1;
                }
                return 0;
            }
        }


        public override string ToString()
        {
            return "PointData [point=" + point + ", getValue()=" + value + "]";
        }

    }

}
