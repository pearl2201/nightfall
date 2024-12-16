using Pika.Base.Mathj.Geometry;
using Pika.Base.Utils;
using System;
using System.Collections.Concurrent;

namespace Pika.Base.Mathj
{
    public class MemoryPool<T> where T : IMemoryObject
    {

        private BlockingCollection<T> cache;


        public MemoryPool(int max)
        {
            cache = new BlockingCollection<T>(max);
        }

        public void put(T value)
        {
            value.release();
            this.cache.Add(value);
        }

        public T get<U>() where U : T
        {
            try
            {
                T t = this.cache.Take();
                if (t == null)
                {
                    t = Activator.CreateInstance<U>();
                }
                return t;
            }
            catch (Exception e)
            {
                PikaLogger.Error("Could not create data: " + e.Message);
            }
            return default(U);
        }

    }
}
