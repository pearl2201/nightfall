using Pika.Ai.Nav.NavPolygon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pika.Ai.Quadtree
{
    /**
     * 功能函数
     * 
     * @author JiangZhiYong
     * @mail 359135103@qq.com
     * @param <V>
     */
    public interface Func<V>
    {
        //	public default void call(PointQuadTree<V> quadTree, Node<V> node) {
        //		
        //	}

        void call(PolygonGuadTree quadTree, Node<Polygon> node);
    }
}
