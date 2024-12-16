using System.IO;
using System.Text;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class PolygonNavMeshWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/PolygonNavMeshWindow")]
    public static void ShowExample()
    {
        PolygonNavMeshWindow wnd = GetWindow<PolygonNavMeshWindow>();
        wnd.titleContent = new GUIContent("PolygonNavMeshWindow");
    }

    private int mapID;  //地图id
    private GameObject pathMesh;    //寻路网格对象
    [SerializeField]
    //地图坐标范围
    public IntegerField startX;
    public IntegerField startZ;
    public IntegerField endX;
    public IntegerField endZ;
    [CreateProperty]
    public string width = "Width: 100";
    [CreateProperty]
    public string height = "Height: 100";
    public new void Show()
    {
        base.Show();
        Init(null);
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        root.dataSource = this;
        Debug.Log(root.dataSource);
        root.Add(new Label("1. The pathfinding layer is marked as terrain or named model_xzm_map id"));
        root.Add(new Label("2. When exporting data, please just circle the pathfinding layer"));
        startX = new IntegerField("StartX");
        root.Add(startX);
        startZ = new IntegerField("startZ");
        root.Add(startZ);
        endX = new IntegerField("EndX");
        root.Add(endX);
        endZ = new IntegerField("EndZ");
        root.Add(endZ);
        startX.RegisterCallback<InputEvent>(e => OnReloadSize());
        startZ.RegisterCallback<InputEvent>(e => OnReloadSize());
        endX.RegisterCallback<InputEvent>(e => OnReloadSize());
        endZ.RegisterCallback<InputEvent>(e => OnReloadSize());
        var widthField = new Label("Width");
        widthField.SetBinding("text", new DataBinding
        {
            dataSourcePath = new PropertyPath(nameof(width))
        });
        root.Add(widthField);
        var heightField = new Label("Height");
        heightField.SetBinding("text", new DataBinding
        {
            dataSourcePath = new PropertyPath(nameof(height))
        });
        root.Add(heightField);
        {
            Button btnTestSize = new Button();
            btnTestSize.text = "Test size";
            btnTestSize.RegisterCallback<ClickEvent>((e) =>
            {
                CreateMapTestMesh(e);
            });
            root.Add(btnTestSize);
        }
        {
            Button btnTestSize = new Button();
            btnTestSize.text = "Generate Server data";
            btnTestSize.RegisterCallback<ClickEvent>((e) =>
            {
                CreatePolyNavMesh(e);
            });
            root.Add(btnTestSize);
        }
        {
            Button btnTestSize = new Button();
            btnTestSize.text = "Reload configuration";
            btnTestSize.RegisterCallback<ClickEvent>((e) =>
            {
                Init(e);
            });
            root.Add(btnTestSize);
        }
        //btn.RegisterCallback<ClickEvent>(CreateMapTestMesh);
        //btn.RegisterCallback<ClickEvent>(CreatePolyNavMesh);
        //btn.RegisterCallback<ClickEvent>(Init);
        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
    }

    public void OnReloadSize()
    {
        Debug.Log("OnReloadChagne");
        width = $"Width: {endX.value - startX.value}";
        height = $"Height:  {endZ.value - startZ.value}";
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(ClickEvent @event)
    {
        try
        {
            ClearMapTest();
            string sceneName = SceneManager.GetActiveScene().name.Trim();
            mapID = int.Parse(sceneName.Split('_')[0]);

            //寻路层先根据标记查找，如果失败根据名称
            pathMesh = GameObject.FindGameObjectWithTag("terrain");
            if (pathMesh == null)
            {
                pathMesh = GameObject.Find("model_xzm_" + mapID);
            }
            if (pathMesh == null)
            {
                Debug.LogError("寻路层找不到，请确定tag设为terrain或名称设为model_xzm_地图id");
                Close();
            }
        }
        catch (System.Exception ex)
        {
            Close();
        }
    }

    private void OnDestroy()
    {
        mapID = 0;
        pathMesh = null;
    }

    /// 创建网格数据
    /// </summary>
    private void CreatePolyNavMesh(ClickEvent @event)
    {
        UnityEngine.AI.NavMeshTriangulation triangulatedWalkNavMesh = Path();
        string path = System.Environment.CurrentDirectory.Replace("\\", "/") + "/Config/NavMeshBuild/";
        Debug.Log(path);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        StringBuilder sb = new StringBuilder("{");
        sb.Append("\"mapID\":").Append(mapID);
        sb.Append(",\"startX\":").Append(startX.value).Append(",\"startZ\":").Append(startZ.value);
        sb.Append(",\"endX\":").Append(endX.value).Append(",\"endZ\":").Append(endZ.value);
        string filename = path + mapID + ".navmesh";

        string data = "";
        data = PathMeshToString(triangulatedWalkNavMesh);
        sb.Append(",").Append(data);
        sb.Append("}");
        MeshToFile(filename, sb.ToString());
    }

    /// <summary>
    /// 清除临时对象
    /// </summary>
    public void ClearMapTest()
    {
        GameObject mapTest = GameObject.Find("MapTest");
        if (mapTest != null)
        {
            Object.DestroyImmediate(mapTest);
        }
    }

    /// <summary>
    /// 创建测试网格
    /// </summary>
    private void CreateMapTestMesh(ClickEvent @event)
    {
        GameObject ob = GameObject.Find("MapTest");
        if (ob == null)
        {
            ob = new GameObject("MapTest");
            ob.AddComponent<MeshFilter>();//网格
            ob.AddComponent<MeshRenderer>();//网格渲染器  

        }
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[]
        {
                new Vector3(startX.value, 0, startZ.value),
                new Vector3(startX.value, 0, endZ.value+startZ.value),
                new Vector3(endX.value+startX.value, 0, endZ.value+startZ.value),
                new Vector3(endX.value+startX.value, 0, startZ.value)
        };
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        ob.GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    /// <summary>
    /// 计算行走层三角网格
    /// </summary>
    /// <returns></returns>
    private UnityEngine.AI.NavMeshTriangulation Path()
    {
        ClearMapTest();
        pathMesh.GetComponent<Renderer>().enabled = true;
        UnityEditor.AI.NavMeshBuilder.ClearAllNavMeshes();
        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
        UnityEngine.AI.NavMeshTriangulation triangulatedNavMesh = UnityEngine.AI.NavMesh.CalculateTriangulation();
        return triangulatedNavMesh;
    }

    /// <summary>
    /// 寻路数据转换为字符串
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    private string PathMeshToString(UnityEngine.AI.NavMeshTriangulation mesh)
    {
        if (mesh.indices.Length < 1)
        {
            return "";
        }
        StringBuilder sb = new StringBuilder();
        sb.Append("\"pathTriangles\":[");
        foreach (var t in mesh.indices)
        {
            sb.Append(t).Append(",");
        }
        sb.Length--;
        sb.Append("],");

        sb.Append("\"pathVertices\":[");
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 v = mesh.vertices[i];
            sb.Append("{\"x\":").Append(v.x).Append(",\"y\":").Append(v.y).Append(",\"z\":").Append(v.z).Append("},");
        }
        sb.Length--;
        sb.Append("]");
        return sb.ToString();
    }

    static void MeshToFile(string filename, string meshData)
    {
        using (StreamWriter sw = new StreamWriter(filename))
        {
            sw.Write(meshData);
        }
    }
}
