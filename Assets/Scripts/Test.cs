using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Assets.Scripts
{
    public class Test : MonoBehaviour
    {
        public string filePath;
        public MeshFilter meshFilter;
        public MovePlayer player;
        private void Start()
        {
            var content = File.ReadAllText(filePath);
            PolygonNavMesh pv = new PolygonNavMesh(content);
            Mesh mesh = new Mesh();
            var graphData = pv.getGraph().getPolygonData();
            var verticles = graphData.getPathVertices();
            var triangles = graphData.getPathTriangles();
            mesh.SetVertices(verticles.Select(x => new Vector3(x.x, x.y, x.z)).ToArray());
            mesh.SetTriangles(triangles, 0);

            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;
            player.SetNavMesh(pv);
        }
    }
}
