
using Pika.Ai.Nav;
using Pika.Ai.Utils;
using Pika.Base.Mathj.Geometry;
using System;
using System.Collections.Generic;

public class PolygonData : NavMeshData
{


    /** 多边形顶点序号， */
    private IDictionary<int, ISet<int>> pathPolygonIndexs;

    /**
     * <p>
     * Unity的NavMeshData有一些共边的三角形，共边的三角形其实不是连通关系，
     * 共边的三角形只是他们共同构成一个凸多边形，并且这种共边的三角形，全部都是扇形排列。
     * </p>
     * <p>
     * 首先先以此划分，生成多边形列表。这个多边形列表，当然没有共边。
     * </p>
     * <p>
     * Unity的NavMeshData 那些不共边的多边形只是index索引不共边，从坐标上还是有共边的，所以我们合并掉重合顶点，重新排列多边形的index索引，就可以恢复到有共边的多边形列表和顶点列表
     * </p>
     */

    public override void check(int scale)
    {
        scaleVector(pathVertices, scale);//地图坐标缩放计算
        pathPolygonIndexs = buildUnityPolygonIndex(this.pathTriangles);
        this.width = Math.Abs(this.getEndX() - this.getStartX());
        this.height = Math.Abs(this.getEndZ() - this.getStartZ());
        this.centerPsoition = new Vector3((endX - startX) / 2, (endZ - startZ) / 2);
    }


    /**
     * 构建多边形索引
     * <p>
     * Unity的NavMeshData有一些共边的三角形，共边的三角形其实不是连通关系，
     * 共边的三角形只是他们共同构成一个凸多边形，并且这种共边的三角形，全部都是扇形排列。
     * </p>
     * @param indexs
     * @return
     */
    private IDictionary<int, ISet<int>> buildUnityPolygonIndex(int[] indexs)
    {
        SortedDictionary<int, ISet<int>> map = new SortedDictionary<int, ISet<int>>();
        int index = 0;
        for (int i = 0; i < indexs.Length;)
        {
            ISet<int> set = new SortedSet<int>();
            set.Add(indexs[i]);
            set.Add(indexs[i + 1]);
            set.Add(indexs[i + 2]);
            int jIndex = i + 3;
            for (int j = jIndex; j < indexs.Length; j += 3)
            {
                if (set.Contains(indexs[j]) || set.Contains(indexs[j + 1]) || set.Contains(indexs[j + 2]))
                {
                    set.Add(indexs[j]);
                    set.Add(indexs[j + 1]);
                    set.Add(indexs[j + 2]);
                    i += 3;
                }
                else
                {
                    i += 3;
                    break;
                }
            }
            map.Add(index++, set);
            if (jIndex == indexs.Length)
            {
                break;
            }
        }

        return map;
    }


    public IDictionary<int, ISet<int>> getPathPolygonIndexs()
    {
        return pathPolygonIndexs;
    }


    public void setPathPolygonIndexs(IDictionary<int, ISet<int>> pathPolygonIndexs)
    {
        this.pathPolygonIndexs = pathPolygonIndexs;
    }

}
