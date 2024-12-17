using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Pika.Base.Utils;
using UnityEngine.ProBuilder.Shapes;
using UnityEditor;
using Newtonsoft.Json.Linq;

public class MovePlayer : MonoSteerableAdapterV3
{
    protected PolygonNavMesh map;
    public Vector3 start;
    public Vector3 end;
    public PolygonGraphPath path = new PolygonGraphPath();
    private SteeringBehavior<Pika.Base.Mathj.Geometry.Vector3> steeringBehavior;
    [SerializeField] MonoSteerableAdapterV3 target;
    public LinePath<Pika.Base.Mathj.Geometry.Vector3> linePath;
    List<(Vector3, Vector3)> draw;
    public void SetNavMesh(PolygonNavMesh mesh)
    {
        map = mesh;
        this.start = this.transform.position;
        this.end = target.transform.position;
        mesh.findPath(new Pika.Base.Mathj.Geometry.Vector3 { x = start.x, y = start.y, z = start.z }, new Pika.Base.Mathj.Geometry.Vector3 { x = end.x, y = end.y, z = end.z }, path);
        List<Pika.Base.Mathj.Geometry.Vector3> points = new List<Pika.Base.Mathj.Geometry.Vector3>();
        points.Add(path.start);
        draw = new List<(Vector3, Vector3)>();
        Vector3 prev = new Vector3(path.start.x, path.start.y, path.start.z);
        foreach (var conn in path)
        {
            var p = conn.getToNode().center;
            points.Add(new Pika.Base.Mathj.Geometry.Vector3(p.x,0,p.z));
            var temp =  new Vector3(conn.getToNode().center.x, conn.getToNode().center.y, conn.getToNode().center.z);
            draw.Add((prev, temp));
            prev = temp;
        }
        points.Add(path.end);
        draw.Add((prev, new Vector3(path.end.x, path.end.y, path.end.z)));
        //Handles.DrawLines(points.Select(x => new Vector3(x.x, x.y, x.z)).ToArray());
        
        linePath = new LinePath<Pika.Base.Mathj.Geometry.Vector3>(points, true);
        var behaviour = new FollowPath<Pika.Base.Mathj.Geometry.Vector3, LinePathParam>(this, linePath, 0, 0f);
        behaviour.setDecelerationRadius(1f);
        behaviour.setArrivalTolerance(0.5f);
        steeringBehavior = behaviour;
        this.position = new Pika.Base.Mathj.Geometry.Vector3(this.transform.position.x, 0, this.transform.position.z);
        this.orientation = this.vectorToAngle(new Pika.Base.Mathj.Geometry.Vector3(this.transform.eulerAngles.x, 0, this.transform.eulerAngles.z));

    }


    private void OnDrawGizmos()
    {
        if (draw == null)
        {
            return;
        }
        foreach(var d in draw)
        {
            Gizmos.DrawLine(d.Item1, d.Item2);
        }
    }
    private void Update()
    {
        if (steeringBehavior != null)
        {
            steeringBehavior.calculateSteering(steeringOutput);
            /*
			 * Here you might want to add a motor control layer filtering steering accelerations.
			 * 
			 * For instance, a car in a driving game has physical constraints on its movement:
			 * - it cannot turn while stationary
			 * - the faster it moves, the slower it can turn (without going into a skid)
			 * - it can brake much more quickly than it can accelerate
			 * - it only moves in the direction it is facing (ignoring power slides)
			 */

            // Apply steering acceleration to move this agent
            applySteering(steeringOutput, Time.deltaTime);
        }
    }

    private void applySteering(SteeringAcceleration<Pika.Base.Mathj.Geometry.Vector3> steering, float time)
    {
     
        this.linearVelocity.mulAdd(steering.linear, time);
        this.linearVelocity.limit(this.getMaxLinearSpeed());
        this.position.mulAdd(linearVelocity, time);
        this.transform.position = new Vector3(this.position.x, this.position.y, this.position.z);

        this.orientation += angularVelocity * time;
        this.angularVelocity += steering.angular * time;
        Pika.Base.Mathj.Geometry.Vector3 f = new Pika.Base.Mathj.Geometry.Vector3();
        var temp = this.angleToVector(f, this.orientation);
        this.transform.eulerAngles = new Vector3(f.x, f.y, f.z);
    }
}

