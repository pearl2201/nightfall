using Newtonsoft.Json;
using Pika.Base.Mathj.Geometry;
using System;
using System.Collections.Generic;

namespace Pika.Ai.Nav
{
    public class NavMeshData
    {
        /** 行走区顶点序号 */
        public int[] pathTriangles;
        /** 行走区坐标 */
        public Vector3[] pathVertices;

        /** 开始坐标 */
        public float startX;
        public float startZ;
        /** 结束坐标 */
        public float endX;
        public float endZ;
        /** navmesh地图id */
        public int mapID;

        public float width;    // 宽
        public float height;   // 高

        /**地图中心位置*/
        protected Vector3 centerPsoition;

        /**是否为3D地图*/
        protected bool threeDimensional;

        /**
         * 数据检测，客户端的顶点坐标和三角形数据有可能是重复的
         * <br>
         */
        public virtual void check(int scale)
        {
            amendmentSameVector(pathTriangles, pathVertices);
            scaleVector(pathVertices, scale);

            this.width = Math.Abs(this.getEndX() - this.getStartX());
            this.height = Math.Abs(this.getEndZ() - this.getStartZ());
            this.centerPsoition = new Vector3((endX - startX) / 2, (endZ - startZ) / 2);
        }

        /**
         * 缩放向量
         * 
         * @param scale
         */
        protected void scaleVector(Vector3[] vertices, int scale)
        {
            if (vertices == null || scale == 1)
            {
                return;
            }
            foreach (Vector3 vector3 in vertices)
            {
                vector3.addX(-this.startX); // 缩放移动
                vector3.addZ(-this.startZ);
                vector3.scl(scale);
            }
        }

        /**
         * 修正重复坐标，使坐标相同的下标修改为一致
         * <p>
         * unity的NavMeshData有一些共边的三角形，共边的三角形其实不是连通关系，共边的三角形只是他们共同构成一个凸多边形，并且这种共边的三角形，全部都是扇形排列。
         * </p>
         */
        public void amendmentSameVector(int[] indexs, Vector3[] vertices)
        {
            if (indexs == null || vertices == null)
            {
                return;
            }
            Dictionary<Vector3, int> map = new Dictionary<Vector3, int>();
            // 检测路径重复点
            for (int i = 0; i < vertices.Length; i++)
            {
                // 重复出现的坐标
                if (map.ContainsKey(vertices[i]))
                {
                    for (int j = 0; j < indexs.Length; j++)
                    {
                        if (indexs[j] == i)
                        { // 修正重复的坐标
                          // System.out.println(String.format("坐标重复为%s",
                          // indexs[j],i,vertices[i].ToString()));
                            indexs[j] = map[vertices[i]];
                        }
                    }
                    // vertices[i] = null;
                }
                else
                {
                    map.Add(vertices[i], i);
                }
            }
        }


        public int[] getPathTriangles()
        {
            return pathTriangles;
        }

        public void setPathTriangles(int[] pathTriangles)
        {
            this.pathTriangles = pathTriangles;
        }

        public Vector3[] getPathVertices()
        {
            return pathVertices;
        }

        public void setPathVertices(Vector3[] pathVertices)
        {
            this.pathVertices = pathVertices;
        }

        public float getStartX()
        {
            return startX;
        }

        public void setStartX(float startX)
        {
            this.startX = startX;
        }

        public float getStartZ()
        {
            return startZ;
        }

        public void setStartZ(float startZ)
        {
            this.startZ = startZ;
        }

        public float getEndX()
        {
            return endX;
        }

        public void setEndX(float endX)
        {
            this.endX = endX;
        }

        public float getEndZ()
        {
            return endZ;
        }

        public void setEndZ(float endZ)
        {
            this.endZ = endZ;
        }

        public int getMapID()
        {
            return mapID;
        }

        public void setMapID(int mapID)
        {
            this.mapID = mapID;
        }


        public float getWidth()
        {
            return width;
        }

        public void setWidth(float width)
        {
            this.width = width;
        }

        public float getHeight()
        {
            return height;
        }

        public void setHeight(float height)
        {
            this.height = height;
        }

        public Vector3 getCenterPsoition()
        {
            return centerPsoition;
        }

        public bool isThreeDimensional()
        {
            return threeDimensional;
        }

        public void setThreeDimensional(bool threeDimensional)
        {
            this.threeDimensional = threeDimensional;
        }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);   
        }
    }
}
