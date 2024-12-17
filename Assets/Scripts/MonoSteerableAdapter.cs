using Pika.Base.Mathj.Geometry;
using UnityEngine;
using Vector3 = Pika.Base.Mathj.Geometry.Vector3;


public class MonoSteerableAdapterV3 : MonoBehaviour, Steerable<Vector3>
{
    public Vector3 position;
    public float orientation;
    public bool independentFacing;
    [Header("Linear movement")]
    public Vector3 linearVelocity;
    public float maxSpeed;
    public float linearAcceleration;
    public float maxLinearAcceleration;
    [Header("Angular movement")]
    public float angularVelocity;
    public float maxAngularVelocity;
    public float angularAcceleration;
    public float maxAngularAcceleration;
    protected SteeringAcceleration<Vector3> steeringOutput =
        new SteeringAcceleration<Vector3>(new Vector3());

    void Start()
    {
        //if 
        //this.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        //this.linearVelocity = 
    }

    public virtual float getZeroLinearSpeedThreshold()
    {
        return 0.001f;
    }


    public virtual void setZeroLinearSpeedThreshold(float value)
    {
    }


    public virtual float getMaxLinearSpeed()
    {
        return maxSpeed;
    }


    public virtual void setMaxLinearSpeed(float maxLinearSpeed)
    {
        this.maxSpeed = maxLinearSpeed;
    }


    public virtual float getMaxLinearAcceleration()
    {
        return maxLinearAcceleration;
    }


    public virtual void setMaxLinearAcceleration(float maxLinearAcceleration)
    {
        this.maxLinearAcceleration = maxLinearAcceleration;
    }


    public virtual float getMaxAngularSpeed()
    {
        return maxAngularVelocity;
    }


    public virtual void setMaxAngularSpeed(float maxAngularSpeed)
    {
        this.maxAngularVelocity = maxAngularSpeed;
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
        return position.copy();
    }


    public virtual float getOrientation()
    {
        return orientation;
    }


    public virtual void setOrientation(float orientation)
    {
        this.orientation = orientation;
    }


    public virtual Vector3 getLinearVelocity()
    {
        return linearVelocity.copy();
    }


    public virtual float getAngularVelocity()
    {
        return angularVelocity;
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
        return (float)Mathf.Atan2(-vector.x, vector.z);
    }


    public virtual Vector3 angleToVector(Vector3 outVector, float angle)
    {
        outVector.x = -(float)Mathf.Sin(angle);
        outVector.z = (float)Mathf.Cos(angle);
        return outVector;
    }

}



