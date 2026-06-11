using UnityEngine;

// 六边形坐标 (Q, R)
public struct HexCoord
{
    public int Q;
    public int R;
    public int S => -Q - R;

    // 六边形单元格的尺寸（可根据项目需求调整）
    public static float HexSize = 1f;

    public HexCoord(int q, int r)
    {
        Q = q;
        R = r;
    }

    // 方向数组
    public static readonly HexCoord[] Directions =
    {
        new HexCoord(1, 0),
        new HexCoord(1, -1),
        new HexCoord(0, -1),
        new HexCoord(-1, 0),
        new HexCoord(-1, 1),
        new HexCoord(0, 1)
    };

    // 加法重载
    public static HexCoord operator +(HexCoord a, HexCoord b)
        => new HexCoord(a.Q + b.Q, a.R + b.R);

    // 减法重载
    public static HexCoord operator -(HexCoord a, HexCoord b)
        => new HexCoord(a.Q - b.Q, a.R - b.R);

    // 相等判断
    public static bool operator ==(HexCoord a, HexCoord b)
        => a.Q == b.Q && a.R == b.R;

    public static bool operator !=(HexCoord a, HexCoord b)
        => !(a == b);

    // 获取相邻格子
    public HexCoord Neighbor(int dir)
        => this + Directions[dir];

    // 计算距离
    public int Distance(HexCoord other)
    {
        int dq = Mathf.Abs(Q - other.Q);
        int dr = Mathf.Abs(R - other.R);
        int ds = Mathf.Abs(S - other.S);
        return (dq + dr + ds) / 2;
    }

    public override bool Equals(object obj)
        => obj is HexCoord coord && this == coord;

    public override int GetHashCode()
        => Q ^ R;

    // 获取从 from 到 to 的方向
    public static int GetHexDirection(HexCoord from, HexCoord to)
    {
        HexCoord diff = to - from;
        int bestDir = 0;
        int minDist = int.MaxValue;

        for (int i = 0; i < 6; i++)
        {
            HexCoord neighbor = from + Directions[i];
            int d = neighbor.Distance(to);
            if (d < minDist)
            {
                minDist = d;
                bestDir = i;
            }
        }
        return bestDir;
    }

    // 新增：将六边形坐标转换为世界坐标（核心补充方法）
    public Vector3 HexToWorld()
    {
        // 六边形轴向坐标转世界坐标公式（水平布局，偶行偏移）
        float x = HexSize * (3f / 2f * Q);
        float z = HexSize * (Mathf.Sqrt(3) / 2f * Q + Mathf.Sqrt(3) * R);
        // Y轴默认0，可根据需求调整（比如高度）
        return new Vector3(x, 0, z);
    }

    // 可选：静态版本，支持直接通过 HexCoord.HexToWorld(coord) 调用
    public static Vector3 HexToWorld(HexCoord coord)
    {
        return coord.HexToWorld();
    }

    // ----------------------------------------------------------------
    // 【只添加这一个方法】解决你的报错！完全不改动原有代码！
    // ----------------------------------------------------------------
    public static Vector3 HexToWorld(int q, int r, float hexSize)
    {
        float x = hexSize * (3f / 2f * q);
        float z = hexSize * (Mathf.Sqrt(3) / 2f * q + Mathf.Sqrt(3) * r);
        return new Vector3(x, 0, z);
    }
}