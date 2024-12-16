using System;

namespace Pika.Ai.Quadtree
{
    /**
     * 数据
     * @author JiangZhiYong
     * @mail 359135103@qq.com
     */
    public abstract class Data<T> : IComparable<Data<T>>
    {

        public T value { get; set; }

        public Data(T value)
        {
            this.value = value;
        }

        public T getValue()
        {
            return value;
        }

        public void setValue(T value)
        {
            this.value = value;
        }
        public abstract int CompareTo(Data<T> other);
    }

}
