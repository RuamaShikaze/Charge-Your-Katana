using UnityEngine;

public class HexGridManager : MonoBehaviour
{
    public static HexGridManager Instance;
    [Header("六边形网格参数(全局唯一基准)")]
    public float hexSize = 1.8f;   // 外接圆半径，和地板尺寸严格一致
    public Transform gridRoot;
    [Header("网格绘制范围")]
    public int gridRange = 10;
    [Header("网格线设置")]
    public bool drawGridLines = true;
    public Color lineColor = Color.blue;
    public float lineWidth = 0.08f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (drawGridLines)
            DrawHexGrid();
    }

    private void DrawHexGrid()
    {
        GameObject lineRoot = new GameObject("HexGridLines");
        lineRoot.transform.parent = gridRoot;

        for (int q = -gridRange; q <= gridRange; q++)
        {
            for (int r = -gridRange; r <= gridRange; r++)
            {
                HexCoord coord = new HexCoord(q, r);
                DrawSingleHex(coord);
            }
        }
    }

    // 绘制【标准平边朝上正六边形】，角度统一，无变形
    private void DrawSingleHex(HexCoord coord)
    {
        Vector3 center = HexToWorld(coord);
        center.y = 0;

        GameObject lineObj = new GameObject($"HexLine_{coord.Q}_{coord.R}");
        lineObj.transform.parent = gridRoot;
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.useWorldSpace = true;
        lr.loop = true;
        lr.positionCount = 6;

        // 标准 Flat-Top 正六边形顶点角度（0°朝右，60°间隔）
        for (int i = 0; i < 6; i++)
        {
            float angle = Mathf.Deg2Rad * (60f * i);
            float x = hexSize * Mathf.Cos(angle);
            float z = hexSize * Mathf.Sin(angle);
            lr.SetPosition(i, center + new Vector3(x, 0, z));
        }
    }

    // ========== 全局统一：轴向坐标 → 世界坐标 ==========
    public Vector3 HexToWorld(HexCoord coord)
    {
        float x = hexSize * 1.5f * coord.Q;
        float z = hexSize * Mathf.Sqrt(3) * (coord.R + coord.Q * 0.5f);
        return new Vector3(x, 0, z) + gridRoot.position;
    }

    // ========== 全局统一：世界坐标 → 轴向坐标 ==========
    public HexCoord WorldToHex(Vector3 worldPos)
    {
        Vector3 local = worldPos - gridRoot.position;
        float q = (2f / 3f * local.x) / hexSize;
        float r = (-1f / 3f * local.x + Mathf.Sqrt(3) / 3f * local.z) / hexSize;
        return RoundHex(q, r);
    }

    private HexCoord RoundHex(float q, float r)
    {
        float s = -q - r;
        int rQ = Mathf.RoundToInt(q);
        int rR = Mathf.RoundToInt(r);
        int rS = Mathf.RoundToInt(s);

        float dQ = Mathf.Abs(rQ - q);
        float dR = Mathf.Abs(rR - r);
        float dS = Mathf.Abs(rS - s);

        if (dQ > dR && dQ > dS) rQ = -rR - rS;
        else if (dR > dS) rR = -rQ - rS;

        return new HexCoord(rQ, rR);
    }
}