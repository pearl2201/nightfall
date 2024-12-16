using Pika.Base.Mathj.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vector3 = Pika.Base.Mathj.Geometry.Vector3;

namespace Assets.Scripts
{
    public class MonoSteerableAdapterV3 : MonoBehaviour, Steerable<Vector3>
    {


        public virtual float getZeroLinearSpeedThreshold()
        {
            return 0.001f;
        }


        public virtual void setZeroLinearSpeedThreshold(float value)
        {
        }


        public virtual float getMaxLinearSpeed()
        {
            return 0;
        }


        public virtual void setMaxLinearSpeed(float maxLinearSpeed)
        {
        }


        public virtual float getMaxLinearAcceleration()
        {
            return 0;
        }


        public virtual void setMaxLinearAcceleration(float maxLinearAcceleration)
        {
        }


        public virtual float getMaxAngularSpeed()
        {
            return 0;
        }


        public virtual void setMaxAngularSpeed(float maxAngularSpeed)
        {
        }


        public virtual float getMaxAngularAcceleration()
        {
            return 0;
        }


        public virtual void setMaxAngularAcceleration(float maxAngularAcceleration)
        {
        }

        public virtual Vector3 getPosition()
        {
            return new Vector3(transform.position.x, transform.position.y, transform.position.z)
        }


        public virtual float getOrientation()
        {
            return 0;
        }


        public virtual void setOrientation(float orientation)
        {
        }


        public virtual Vector3 getLinearVelocity()
        {
            return default(T);
        }


        public virtual float getAngularVelocity()
        {
            return 0;
        }


        public virtual float getBoundingRadius()
        {
            return 0;
        }


        public virtual bool isTagged()
        {
            return false;
        }


        public virtual void setTagged(bool tagged)
        {
        }


        public virtual Location<Vector3> newLocation()
        {
            return null;
        }


        public virtual float vectorToAngle(Vector3 vector)
        {
            return 0;
        }


        public virtual Vector3 angleToVector(Vector3 outVector, float angle)
        {
            return default(Vector3);
        }

    }


}
