using UnityEngine;

// 武器类型枚举
public enum WeaponType
{
    Katana,  // 打刀（近战冲刺）
    Shuriken // 手里剑（远程）
}

// ============================
// 武器效果实现（可扩展）
// ============================
public static class WeaponEffect
{
    /// <summary>
    /// Katana 冲刺到目标位置，回调命中事件
    /// </summary>
    public static void KatanaAttack(Transform player, Transform target, System.Action onHit)
    {
        if (target == null) return;

        // 从玩家身上获取/添加移动组件
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement == null)
        {
            movement = player.gameObject.AddComponent<PlayerMovement>();
        }
       
        // 执行冲刺
        movement.DashToTarget(target, onHit);
    }
}

// ============================
// 移动逻辑：冲刺到目标（碰撞检测）
// ============================
public class PlayerMovement : MonoBehaviour
{
    private bool isDashing;
    private Transform dashTarget;
    private float dashSpeed = 50f;
    private System.Action onHitCallback;

    void Update()
    {
        if (!isDashing || dashTarget == null) return;

        // 向目标移动
        Vector3 dir = (dashTarget.position - transform.position).normalized;
        transform.position += dir * dashSpeed * Time.deltaTime;

        // 距离判断：防止穿透目标（替代仅靠Trigger）
        float distance = Vector3.Distance(transform.position, dashTarget.position);
        if (distance < 0.5f)
        {
            OnHitTarget();
        }
    }

    // 碰撞触发命中
    private void OnTriggerEnter(Collider other)
    {
        if (isDashing && dashTarget != null && other.transform == dashTarget)
        {
            OnHitTarget();
        }
    }

    // 命中目标处理
    private void OnHitTarget()
    {
        isDashing = false;
        onHitCallback?.Invoke();
        onHitCallback = null; // 清空回调防止重复执行
        dashTarget = null;
    }

    // 开始冲刺
    public void DashToTarget(Transform target, System.Action onHit)
    {
        dashTarget = target;
        isDashing = true;
        onHitCallback = onHit;
    }
}