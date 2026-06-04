using UnityEngine;

public class HexGridManager : MonoBehaviour
{
    public static HexGridManager Instance;

    [Header("六边形网格参数")]
    public float hexSize = 2f;
    public Transform gridRoot;

    [Header("网格绘制范围")]
    public int gridRange = 6; // 格子范围

    [Header("网格线设置")]
    public bool drawGridLines = true;
    public Color lineColor = Color.cyan;
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

    // ==========================
    // 正确绘制六边形网格（已修复）
    // ==========================
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

    private void DrawSingleHex(HexCoord coord)
    {
        Vector3 center = HexToWorld(coord);
        center.y = 0; // 强制贴地面，解决高度问题

        GameObject lineObj = new GameObject($"HexLine_{coord.Q}_{coord.R}");
        lineObj.transform.parent = gridRoot;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.useWorldSpace = true;
        lr.loop = true; // 闭合六边形（关键修复）
        lr.positionCount = 6;

        // 正确六边形6个顶点
        for (int i = 0; i < 6; i++)
        {
            float angle = Mathf.Deg2Rad * (60 * i - 30);
            float x = hexSize * Mathf.Cos(angle);
            float z = hexSize * Mathf.Sin(angle);
            lr.SetPosition(i, center + new Vector3(x, 0, z));
        }
    }

    // ==========================
    // 坐标转换（正确）
    // ==========================
    public Vector3 HexToWorld(HexCoord coord)
    {
        float x = hexSize * 1.5f * coord.Q;
        float z = hexSize * Mathf.Sqrt(3) * (coord.R + coord.Q * 0.5f);
        return new Vector3(x, 0, z) + gridRoot.position;
    }

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