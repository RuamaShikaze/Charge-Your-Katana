using UnityEngine;

public class EnemyCharge : MonoBehaviour
{
    public ElementType currentCharge = ElementType.None;
    public int capacity = 1;
    public int maxCapacity = 2;
    public int currentChargeCount = 1;
    public bool IsExcited { get; private set; }
    public void SetExcited(bool value) => IsExcited = value;
    void Start()
    {
        // 开局刷新颜色
        ElementReaction.RefreshEnemyColor(this);
    }

    public void SetChargeCount(int value)
    {
        currentChargeCount = Mathf.Max(0, value);
        GetComponent<HexGridUnit>().UpdateChargeUI();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.CheckAllEnemyDefeated();
        }
    }
    public void SetCharge(ElementType type)
    {
        currentCharge = type;
        capacity = 1;
    }

    public void ClearCharge()
    {
        currentCharge = ElementType.None;
        capacity = 1;
    }

    
}
