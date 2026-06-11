using UnityEngine;

public class Wall : MonoBehaviour
{
    // 当前所在六边形坐标
    public HexCoord wallCoord;
    // 是否被激发
    [Header("墙体状态")]
    public bool isExcited = false;

    private HexGridUnit _gridUnit;

    private void Awake()
    {
        _gridUnit = GetComponent<HexGridUnit>();
        if (_gridUnit != null)
        {
            wallCoord = _gridUnit.GetCurrentHex();
        }
    }

    // 【规则1】禁止主动附着电荷
    public bool CanReceiveCharge()
    {
        return false;
    }

    // 【规则2】墙体被激发/解除激发
    public void SetExcited(bool active)
    {
        isExcited = active;
        // 可在这里加：变色、特效、音效
        if (active)
        {
            Debug.Log($"墙体 {wallCoord.Q},{wallCoord.R} 已被激发");
        }
    }

    // 外部快速获取当前格子
    public HexCoord GetWallCoord() => wallCoord;
}