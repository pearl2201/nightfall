using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class TargetPlayer: MonoSteerableAdapterV3
    {
        private void Awake()
        {
            this.position = new Pika.Base.Mathj.Geometry.Vector3(this.transform.position.x, 0, this.transform.position.z);
            this.orientation = this.vectorToAngle(new Pika.Base.Mathj.Geometry.Vector3(this.transform.eulerAngles.x, 0, this.transform.eulerAngles.z));
        }

        private void Update()
        {
            this.position.x = this.transform.position.x;
            this.position.z = this.transform.position.z;
            
        }
    }
}
