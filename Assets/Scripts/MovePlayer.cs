using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class MovePlayer : MonoSteerableAdapter
{
    protected PolygonNavMesh map;
    public Vector3 start;
    public Vector3 end;
    public PolygonGraphPath path = new PolygonGraphPath();
    private SteeringBehavior<Pika.Base.Mathj.Geometry.Vector3> steeringBehavior;
    public void SetNavMesh(PolygonNavMesh mesh)
    {
        map = mesh;
        mesh.findPath(new Pika.Base.Mathj.Geometry.Vector2 { x = start.x, y = start.y, z = start.z }, new Pika.Base.Mathj.Geometry.Vector3 { x = end.x, y = end.y, z = end.z }, path);

    }
}

