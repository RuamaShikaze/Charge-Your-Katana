using UnityEngine;

public enum ElementType
{
    None,
    PositiveElectricity,
    NegativeElectricity
}

public static class ElementReaction
{
    public static GameObject shockWavePrefab;

    //==============================
    // 手里剑元素反应（核心逻辑）
    //==============================
    public static void ShurikenReaction(GameObject enemy, ElementType bulletCharge)
    {
        if (enemy == null || bulletCharge == ElementType.None) return;

        EnemyCharge ec = enemy.GetComponent<EnemyCharge>();
        if (ec == null) return; // 提前判空，避免空引用
        ec.SetExcited(true);

        // 实时刷新颜色
        RefreshEnemyColor(ec);

        // 1. 首次附着：赋予对应电荷
        if (ec.currentCharge == ElementType.None)
        {
            ec.SetCharge(bulletCharge);
            Debug.Log($"<color=yellow>首次附着！{enemy.name} 赋予 {ec.currentCharge}</color>");
            RefreshEnemyColor(ec);
            return;
        }

        // 2. 同电荷：叠加层数
        if (ec.currentCharge == bulletCharge)
        {
            ec.capacity++;
            Debug.Log($"<color=orange>叠加层数！{enemy.name} 当前层数{ec.capacity}</color>");

            // 达到2层触发超载
            if (ec.capacity >= ec.maxCapacity)
            {
                Debug.Log($"<color=cyan>超载触发！{ec.currentCharge} 冲击波生成</color>");
                PlayElementFlash(ec.gameObject, ec.currentCharge, isExcited: true);
                // 修复：调用嵌套类的静态方法（调整访问权限后）
                ElementTypes.CreateShockWave(ec.transform, ec.currentCharge);
                ec.capacity = 1;
            }
            RefreshEnemyColor(ec);
            return;
        }

        // 3. 反电荷：湮灭消失
        else
        {
            Debug.Log($"<color=purple>湮灭反应！{enemy.name} 电荷消失</color>");
            PlayElementFlash(enemy, ElementType.None, isExcited: false, isAnnihilation: true);
            SoundManager.Instance.PlayAnnihilation(enemy.transform.position);
            Object.Destroy(enemy, 0.1f);
        }
    }

    //==============================
    // 武士刀攻击判定（反电荷才能攻击）
    //==============================
    public static bool KatanaCanAttack(GameObject enemy, ElementType playerCharge)
    {
        if (enemy == null) return false;
        EnemyCharge ec = enemy.GetComponent<EnemyCharge>();
        if (ec == null) return false;

        return ec.currentCharge != ElementType.None && ec.currentCharge != playerCharge;
    }

    //==============================
    // 嵌套类：元素特效与冲击波
    //==============================
    public static class ElementTypes
    {
        public static GameObject shockWavePrefab;
        public static float waveRadius = 16f; // 影响范围 = 视觉范围

        public static void CreateShockWave(Transform center, ElementType type)
        {
            // 生成视觉冲击波（范围 = 8，和影响范围完全一样）
            if (shockWavePrefab != null)
            {
                GameObject wave = Object.Instantiate(
                    shockWavePrefab,
                    center.position,
                    Quaternion.identity
                );
                SoundManager.Instance.PlayShockWaveSound(center.position);
                ShockWaveVFX vfx = wave.GetComponent<ShockWaveVFX>();
                if (vfx != null)
                {
                    if (type == ElementType.PositiveElectricity)
                        vfx.SetColor(Color.red);
                    else if (type == ElementType.NegativeElectricity)
                        vfx.SetColor(Color.blue);
                }
            }

            // 范围完全同步：8米
            Collider[] hits = Physics.OverlapSphere(center.position, waveRadius);

            foreach (var hit in hits)
            {
                EnemyCharge ec = hit.GetComponent<EnemyCharge>();
                if (ec == null || ec.IsExcited) continue;

                HexGridUnit unit = hit.GetComponent<HexGridUnit>();
                if (unit == null) continue;

                // 给无电小球附着电荷 → 并强制刷新颜色
                if (ec.currentCharge == ElementType.None)
                {
                    ec.SetCharge(type);
                    ElementReaction.RefreshEnemyColor(ec); // 🔥 修复颜色不显示
                    continue;
                }

                // 同性推开 / 异性拉回
                HexCoord centerHex = HexGridManager.Instance.WorldToHex(center.position);
                HexCoord targetHex = unit.GetCurrentHex();
                int dirToCenter = HexCoord.GetHexDirection(targetHex, centerHex);

                if (ec.currentCharge == type)
                {
                    int dirAway = (dirToCenter + 3) % 6;
                    if (!unit.isMoving)
                        unit.MoveToDir(dirAway, 1);
                }
                else
                {
                    if (!unit.isMoving)
                        unit.MoveToDir(dirToCenter, 1);
                }
            }
        }
    }


    //==============================
    // 刷新敌人电荷颜色
    //==============================
    public static void RefreshEnemyColor(EnemyCharge ec)
    {
        if (ec == null) return;
        Renderer r = ec.GetComponent<Renderer>();
        if (r == null) return;

        switch (ec.currentCharge)
        {
            case ElementType.None:
                r.material.color = Color.gray;
                break;
            case ElementType.PositiveElectricity:
                r.material.color = Color.red;
                break;
            case ElementType.NegativeElectricity:
                r.material.color = Color.blue;
                break;
        }
    }

    //==============================
    // 播放元素闪烁效果（超载/湮灭）
    //==============================
    public static void PlayElementFlash(GameObject go, ElementType type, bool isExcited = false, bool isAnnihilation = false)
    {
        if (go == null) return;
        Renderer r = go.GetComponent<Renderer>();
        if (r == null) return;

        Color flashColor = Color.clear;

        if (isAnnihilation)
        {
            flashColor = Color.magenta; // 品红 = 湮灭
        }
        else if (isExcited)
        {
            flashColor = type == ElementType.PositiveElectricity ? Color.red : Color.blue;
        }

        if (flashColor == Color.clear) return;

        // 闪烁逻辑
        Color original = r.material.color;
        r.material.color = flashColor;
        InvokeColorReset(r, original, 0.2f);
    }

    // 延迟重置颜色
    private static void InvokeColorReset(Renderer r, Color original, float delay)
    {
        if (r == null) return;
        System.Action reset = () => {
            if (r != null) r.material.color = original;
        };
        r.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(DelayInvoke(delay, reset));
    }

    private static System.Collections.IEnumerator DelayInvoke(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}

// 补充ShockWaveVFX的基础定义（如果没有的话）
