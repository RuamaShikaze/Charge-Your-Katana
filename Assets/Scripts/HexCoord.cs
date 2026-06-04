using UnityEngine;

// 正六边形 轴向坐标 (Q, R)
public struct HexCoord
{
    public int Q;
    public int R;
    public int S => -Q - R;

    public HexCoord(int q, int r)
    {
        Q = q;
        R = r;
    }

    // 六个邻格方向
    public static readonly HexCoord[] Directions =
    {
        new HexCoord(1, 0),
        new HexCoord(1, -1),
        new HexCoord(0, -1),
        new HexCoord(-1, 0),
        new HexCoord(-1, 1),
        new HexCoord(0, 1)
    };

    // 相加
    public static HexCoord operator +(HexCoord a, HexCoord b)
        => new HexCoord(a.Q + b.Q, a.R + b.R);

    // 相减
    public static HexCoord operator -(HexCoord a, HexCoord b)
        => new HexCoord(a.Q - b.Q, a.R - b.R);

    // 判断相等
    public static bool operator ==(HexCoord a, HexCoord b)
        => a.Q == b.Q && a.R == b.R;

    public static bool operator !=(HexCoord a, HexCoord b)
        => !(a == b);

    // 获取邻格
    public HexCoord Neighbor(int dir)
        => this + Directions[dir];

    // 网格距离
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

    // 求从 A 指向 B 的六边形最近方向
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

}