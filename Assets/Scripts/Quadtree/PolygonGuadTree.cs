using Pika.Ai.Nav.NavPolygon;
using Pika.Ai.Quadtree;
using Pika.Base.Mathj.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Pika.Ai.Quadtree
{

    /**
     * 多边形定制四叉树,用于快速判断一个坐标点位于哪个多边形中 <br>
     * 多边形和和象限相交,包含与被包含者插入
     * 
     * 
     * @author JiangZhiYong
     * @mail 359135103@qq.com
     */
    public class PolygonGuadTree : QuadTree<Vector3, Polygon>
    {



        private int depth = 5;
        private int items = 10;
        /** 真实个数，包含重复多边形 */
        private int realCount;

        public PolygonGuadTree(float minX, float minZ, float maxX, float maxZ) : base(minX, minZ, maxX, maxZ)
        {

        }

        /**
         * 
         * @param minX
         * @param minZ
         * @param maxX
         * @param maxZ
         * @param depth
         *            深度
         * @param items
         *            个数 建议小于10，数量太大，影响效率
         */
        public PolygonGuadTree(float minX, float minZ, float maxX, float maxZ, int depth, int items) : base(minX, minZ, maxX, maxZ)
        {

            this.depth = depth;
            this.items = items;
        }

        /**
         * 插入不按key分类，根据值进行插入到对应区域
         */

        public override void set(Vector3 k, Polygon v)
        {
            Node<Polygon> r = this.root;
            if (k.x < r.getX() || k.z < r.getZ() || k.x > r.getX() + r.getW() || k.z > r.getZ() + r.getH())
            {
                throw new QuadTreeException(String.Format("坐标越界:(%f,%f),范围(%f,%f)-->(%f,%f)", k.x, k.z,
                        getRootNode().getX(), getRootNode().getZ(), (getRootNode().getX() + getRootNode().getW()),
                        (getRootNode().getZ() + getRootNode().getH())));
            }

            if (this.insert(r, new PointData<Polygon>(k, v)))
            {
                this.count++;
            }
        }

        /**
         * 不重复个数唯一对象
         */

        public override int getCount()
        {
            return base.getCount();
        }

        /**
         * 获取当前坐标所在多边形
         */

        public override Polygon get(Vector3 position, Polygon defaultValue)
        {
            List<Polygon> polygons = getPolygons(position);
            if (polygons == null || polygons.Count() == 0)
            {
                return defaultValue;
            }
            if (polygons.Count() == 1)
            {
                return polygons.ElementAt(0);
            }
            //获取最接近坐标点的多边形
            float minDistance = byte.MaxValue;
            Polygon p = defaultValue;
            foreach (Polygon polygon in polygons)
            {
                float distance = Math.Abs(polygon.center.y - position.y);
                if (distance < minDistance)
                {
                    p = polygon;
                    minDistance = distance;
                }
            }
            return p;
        }

        /**
         * @param k
         *            此处为查询的坐标
         */

        public override Node<Polygon> find(Node<Polygon> node, Vector3 k)
        {
            Node<Polygon> resposne = null;
            switch (node.getNodeType())
            {
                case NodeType.EMPTY:
                    break;

                case NodeType.LEAF:
                    if (node.getPolygon().isInnerPoint(k))
                    {
                        resposne = node;
                    }

                    break;
                case NodeType.POINTER:
                    resposne = this.find(this.getQuadrantForPoint(node, k.x, k.z), k);
                    break;

                default:
                    throw new QuadTreeException("Invalid nodeType");
            }
            return resposne;
        }


        public override Polygon remove(Vector3 k)
        {
            throw new QuadTreeException(String.Format("多边形四叉树不能移除节点内容"));
        }


        public override List<T> getKeyValues<T>()
        {
            throw new QuadTreeException(String.Format("多边形四叉树不支持获取，请使用其他方式"));
        }


        public override List<Vector3> getKeys()
        {
            throw new QuadTreeException(String.Format("多边形四叉树不支持获取，请使用其他方式"));
        }


        public override List<Polygon> getValues()
        {
            throw new QuadTreeException(String.Format("多边形四叉树不支持获取，请使用其他方式"));
        }


        public override void clear()
        {
            base.clear();
            this.root.getDatas().Clear();
        }

        /**
         * Inserts a point into the tree, updating the tree's structure if necessary.
         * 
         * @param {.QuadTree.Node}
         *            parent The parent to insert the point into.
         * @param {QuadTree.Point}
         *            point The point to insert.
         * @return {bool} True if a new node was added to the tree; False if a node
         *         already existed with the correpsonding coordinates and had its value
         *         reset.
         * @private
         */
        private bool insert(Node<Polygon> parent, PointData<Polygon> point)
        {
            bool result = false;
            switch (parent.getNodeType())
            {
                case NodeType.EMPTY:
                    this.setPointForNode(parent, point);
                    result = true;
                    break;
                case NodeType.LEAF:
                    List<Data<Polygon>> datas = parent.getDatas();
                    // 未插满或者到达最大深度，直接加入
                    if (datas.Count() < items || parent.getDepth() >= depth)
                    {
                        if (datas.Contains(point))
                        {
                            return false;
                        }
                        this.setPointForNode(parent, point);
                        result = true;
                    }
                    else
                    {
                        this.split(parent);
                        result = this.insert(parent, point);
                    }

                    break;
                case NodeType.POINTER:
                    List<Node<Polygon>> nodes = this.getQuadrantForPolygon(parent, point.getValue());
                    foreach (Node<Polygon> n in nodes)
                    {
                        if (this.insert(n, point))
                        {
                            result = true;
                        }
                    }
                    break;

                default:
                    throw new QuadTreeException("Invalid nodeType in parent");
            }
            return result;
        }

        /**
         * Sets the point for a node, as long as the node is a leaf or empty.
         * 
         * @param {QuadTree.Node}
         *            node The node to set the point for.
         * @param {QuadTree.Point}
         *            point The point to set.
         * @private
         */
        private void setPointForNode(Node<Polygon> node, PointData<Polygon> point)
        {
            if (node.getNodeType() == NodeType.POINTER)
            {
                throw new QuadTreeException("Can not set point for node of type POINTER");
            }
            node.setNodeType(NodeType.LEAF);
            node.getDatas().Add(point);
            realCount++;
            //		PikaLogger.Debug("多边形{} 加入节点深度{}，个数{}", point.getValue().getIndex(), node.getDepth(), node.getDatas().Count());
        }

        /**
         * 拆分节点，当前节点变为指针节点，将多边形按是否和矩形相交进行分配到指定区域，一个多边形可在多个区域
         * 
         * @param node
         */
        private void split(Node<Polygon> node)
        {
            List<Data<Polygon>> datas = node.getDatas();
            node.setDatas(null);
            node.setNodeType(NodeType.POINTER);
            float x = node.getX();
            float z = node.getZ();
            float hw = node.getW() / 2;
            float hh = node.getH() / 2;
            int depth = node.getDepth() + 1;

            node.setNw(new Node<Polygon>(x, z, hw, hh, node, depth));
            node.setNe(new Node<Polygon>(x + hw, z, hw, hh, node, depth));
            node.setSw(new Node<Polygon>(x, z + hh, hw, hh, node, depth));
            node.setSe(new Node<Polygon>(x + hw, z + hh, hw, hh, node, depth));

            realCount -= datas.Count();
            foreach (Data<Polygon> point in datas)
            {
                this.insert(node, (PointData<Polygon>)point);
            }

        }

        /**
         * 获取多边形所在的象限
         * 
         * @param parent
         * @param polygon
         * @return
         */
        private List<Node<Polygon>> getQuadrantForPolygon(Node<Polygon> parent, Polygon polygon)
        {
            List<Node<Polygon>> nodes = new List<Node<Polygon>>();

            // 相交，象限包含多边形，多边形包含象限
            if (parent.getNw().getPolygon().contains(polygon) || polygon.contains(parent.getNw().getPolygon())
                    || polygon.intersectsEdge(parent.getNw().getPolygon()))
            {
                nodes.Add(parent.getNw());
            }

            // NE
            if (parent.getNe().getPolygon().contains(polygon) || polygon.contains(parent.getNe().getPolygon())
                    || polygon.intersectsEdge(parent.getNe().getPolygon()))
            {
                nodes.Add(parent.getNe());
            }

            // SE
            if (parent.getSe().getPolygon().contains(polygon) || polygon.contains(parent.getSe().getPolygon())
                    || polygon.intersectsEdge(parent.getSe().getPolygon()))
            {
                nodes.Add(parent.getSe());
            }

            // SW
            if (parent.getSw().getPolygon().contains(polygon) || polygon.contains(parent.getSw().getPolygon())
                    || polygon.intersectsEdge(parent.getSw().getPolygon()))
            {
                nodes.Add(parent.getSw());
            }

            return nodes;
        }

        /**
         * 真实个数，包含重复多边形
         * 
         * @return
         */
        public int getRealCount()
        {
            return realCount;
        }

        /**
         * 获取当前坐标所在多边形<br>
         * 一个坐标点可能在多个多边形中，存在上下重叠，具体在哪个多边形自行判断高度，或三维使用八叉树
         * 
         * @param position
         * @return
         */
        public List<Polygon> getPolygons(Vector3 position)
        {
            Node<Polygon> nodes = find(this.root, position);
            if (nodes == null || nodes.getDatas() == null)
            {
                return null;
            }
            var temp = nodes.getDatas().Select(n => n.getValue());

            var list = temp.Where(p => p.isInnerPoint(position))
                    .ToList();
            return list;
        }

        /**
         * 获取当前坐标所在象限所有的多边形
         * 
         * @param position
         * @return
         */
        public List<Polygon> getQuadrantPolygons(Vector3 position)
        {
            Node<Polygon> nodes = find(this.root, position);
            if (nodes == null || nodes.getDatas() == null)
            {
                return null;
            }
            List<Polygon> list = nodes.getDatas().Select(n => n.getValue()).ToList();
            return list;
        }

        /**
         * Returns the child quadrant within a node that contains the given (x, y)
         * coordinate.
         * 
         * @param {QuadTree.Node}
         *            parent The node.
         * @param {number}
         *            x The x-coordinate to look for.
         * @param {number}
         *            y The y-coordinate to look for.
         * @return {QuadTree.Node} The child quadrant that contains the point.
         * @private
         */
        private Node<Polygon> getQuadrantForPoint(Node<Polygon> parent, float x, float y)
        {
            float mx = parent.getX() + parent.getW() / 2;
            float mz = parent.getZ() + parent.getH() / 2;
            if (x < mx)
            {
                return y < mz ? parent.getNw() : parent.getSw();
            }
            else
            {
                return y < mz ? parent.getNe() : parent.getSe();
            }
        }

        /**
         * 获取以当前坐标为中心，半径为radius的正方形相交的多边形 <br>
         * 可以用于将进入阻挡区的对象移动到附近行走区
         * <br>
         * @note 非常耗时
         * @param radius
         *            坐标半径范围
         * @return
         */

        public List<Polygon> searchWithin(Vector3 point, float radius)
        {
            float xmin = point.x - radius;
            float zmin = point.z - radius;
            float xmax = point.x + radius;
            float zmax = point.z + radius;
            List<Polygon> arr = new List<Polygon>();

            Polygon p = new Polygon(0, new Vector3(xmin, zmin), new Vector3(xmin, zmax), new Vector3(xmax, zmax),
                   new Vector3(xmax, zmin));
            this.navigate(this.root, (quadTree, node) =>
            {
                foreach (Data<Polygon> data in node.getDatas())
                {
                    Polygon polygon = data.getValue();
                    if (polygon.contains(p) || p.contains(polygon) || p.intersectsEdge(polygon))
                    {
                        arr.Add(polygon);
                    }

                }
            }, xmin, zmin, xmax, zmax);
            return arr;
        }



        /**
         * 遍历节点
         * 
         * @param node
         * @param func
         * @param xmin
         * @param ymin
         * @param xmax
         * @param ymax
         */
        private void navigate(Node<Polygon> node, Action<PolygonGuadTree, Node<Polygon>> func, double xmin, double ymin, double xmax, double ymax)
        {
            switch (node.getNodeType())
            {
                case NodeType.LEAF:
                    func.Invoke(this, node);
                    break;

                case NodeType.POINTER:
                    if (intersects(xmin, ymax, xmax, ymin, node.getNe()))
                        this.navigate(node.getNe(), func, xmin, ymin, xmax, ymax);
                    if (intersects(xmin, ymax, xmax, ymin, node.getSe()))
                        this.navigate(node.getSe(), func, xmin, ymin, xmax, ymax);
                    if (intersects(xmin, ymax, xmax, ymin, node.getSw()))
                        this.navigate(node.getSw(), func, xmin, ymin, xmax, ymax);
                    if (intersects(xmin, ymax, xmax, ymin, node.getNw()))
                        this.navigate(node.getNw(), func, xmin, ymin, xmax, ymax);
                    break;
                default:
                    break;
            }
        }
    }

}
