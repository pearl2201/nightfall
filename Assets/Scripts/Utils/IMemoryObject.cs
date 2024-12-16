using System;
using System.Collections.Generic;
using System.Text;

namespace Pika.Base.Mathj.Geometry
{
    /**
     * 对象池对象
     */
    public interface IMemoryObject
    {

        /**
         * 对象释放
         */
        void release();
    }

}
