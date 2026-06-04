using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("视角")]
    public Camera playerCamera;
    public float mouseSensitivity = 2f;

    [Header("目标检测")]
    public float detectionRange = 100f;
    public LayerMask targetLayer;

    public ElementType selectedElement;
    public WeaponType selectedWeapon;
    private Transform lockedTarget;
    private float xRotation;

    // ==============================================================================
    // 阶段更新
    // ==============================================================================
    public void OnTurnStateUpdated(TurnManager.TurnState state)
    {
        if (TurnManager.Instance.currentTurn != TurnManager.TurnOwner.Player)
            return;

        switch (state)
        {
            case TurnManager.TurnState.SelectElement:
                Debug.Log("选元素：1正 2负 | 按3返回");
                BattleUI.Instance.ClearTargetCircle();
                SafeHideCrosshair();
                break;

            case TurnManager.TurnState.SelectWeapon:
                Debug.Log("选武器：Q刀 E手里剑 | 按R返回");
                SafeHideCrosshair();
                break;

            case TurnManager.TurnState.FindTarget:
                Debug.Log("瞄准目标：左键锁定 | 右键返回");
                SafeShowCrosshair();
                break;

            case TurnManager.TurnState.LockTarget:
                TryLockTarget();
                break;

            case TurnManager.TurnState.Attack:
                ExecuteAttack();
                break;
        }
    }

    // ==============================================================================
    // 全阶段可转视角
    // ==============================================================================
    private void Update()
    {
        if (TurnManager.Instance.currentTurn != TurnManager.TurnOwner.Player)
            return;

        HandleCameraRotation();

        if (TurnManager.Instance.currentState != TurnManager.TurnState.Attack)
            HandleInput();
    }

    // ==============================================================================
    // 输入
    // ==============================================================================
    private void HandleInput()
    {
        var state = TurnManager.Instance.currentState;

        if (state == TurnManager.TurnState.SelectElement)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectElement(ElementType.PositiveElectricity);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectElement(ElementType.NegativeElectricity);
            if (Input.GetKeyDown(KeyCode.Alpha3)) GoBack();
        }
        else if (state == TurnManager.TurnState.SelectWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Q)) SelectWeapon(WeaponType.Katana);
            if (Input.GetKeyDown(KeyCode.E)) SelectWeapon(WeaponType.Shuriken);
            if (Input.GetKeyDown(KeyCode.R)) GoBack();
        }
        else if (state == TurnManager.TurnState.FindTarget)
        {
            if (Input.GetMouseButtonDown(0)) TurnManager.Instance.NextStep();
            if (Input.GetMouseButtonDown(1)) GoBack();
        }
    }

    private void GoBack()
    {
        var state = TurnManager.Instance.currentState;

        // 从武器页返回 → 清空武器图标
        if (state == TurnManager.TurnState.SelectWeapon)
        {
            BattleUI.Instance.HideWeapon();
        }
        // 从瞄准页返回 → 清空武器 + 取消瞄准UI
        else if (state == TurnManager.TurnState.FindTarget)
        {
            BattleUI.Instance.HideWeapon();
            BattleUI.Instance.ClearTargetCircle();
        }
        BattleUI.Instance.ClearTargetCircle();
        TurnManager.Instance.PrevStep();
    }

    // ==============================================================================
    // 选元素 → 左手显示
    // ==============================================================================
    private void SelectElement(ElementType type)
    {
        SoundManager.Instance.PlayElectricChoosen(transform.position);
        BattleUI.Instance.HideElement();
        selectedElement = type;
        Debug.Log("已选元素：" + type);
        BattleUI.Instance.ShowElement(type);
        TurnManager.Instance.NextStep();
    }

    // ==============================================================================
    // 选武器 → 右手显示
    // ==============================================================================
    private void SelectWeapon(WeaponType type)
    {
        if(type == WeaponType.Katana)
        {
            SoundManager.Instance.PlayKatanaChoosen(transform.position);
        }
        else if(type == WeaponType.Shuriken)
        {
            SoundManager.Instance.PlayShurikenChoosen(transform.position);
        }
        BattleUI.Instance.HideWeapon();
        selectedWeapon = type;
        Debug.Log("已选武器：" + type);
        BattleUI.Instance.ShowWeapon(type);
        TurnManager.Instance.NextStep();
    }

    // ==============================================================================
    // 锁定目标 → 圈真实圈在敌人身上
    // ==============================================================================
    private void TryLockTarget()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        lockedTarget = null;

        if (Physics.Raycast(ray, out RaycastHit hit, detectionRange, targetLayer))
        {
            lockedTarget = hit.transform;
            Debug.Log("已锁定：" + lockedTarget.name);
            BattleUI.Instance.SetTarget(lockedTarget); // ✅ 圈在敌人身上
        }

        TurnManager.Instance.NextStep();
    }

    // ==============================================================================
    // 攻击
    // ==============================================================================
    private void ExecuteAttack()
    {
        if (lockedTarget == null)
        {
            BattleUI.Instance.ClearTargetCircle();
            TurnManager.Instance.NextStep();
            return;
        }

        Debug.Log($"攻击：{selectedElement} + {selectedWeapon} → {lockedTarget.name}");
        BattleUI.Instance.PlayAttackAnim();
        TurnManager.Instance.LockStep();

        switch (selectedWeapon)
        {
            case WeaponType.Katana:
                if (!ElementReaction.KatanaCanAttack(lockedTarget.gameObject, selectedElement))
                {
                    Debug.Log("<color=red>无法攻击：仅可攻击异种电荷</color>");
                    BattleUI.Instance.ClearTargetCircle();
                    TurnManager.Instance.UnlockStep();
                    TurnManager.Instance.NextStep();
                    return;
                }

                WeaponEffect.KatanaAttack(transform, lockedTarget, () =>
                {
                    SoundManager.Instance.PlayKatanaSound(transform.position);
                    Destroy(lockedTarget.gameObject);
                    BattleUI.Instance.ClearTargetCircle();
                    TurnManager.Instance.UnlockStep();
                    TurnManager.Instance.NextStep();
                });
                break;

            case WeaponType.Shuriken:
                ElementReaction.ShurikenReaction(lockedTarget.gameObject, selectedElement);
                SoundManager.Instance.PlayShurikenSound(transform.position);
                BattleUI.Instance.ClearTargetCircle();
                TurnManager.Instance.UnlockStep();
                TurnManager.Instance.NextStep();
                break;

            default:
                BattleUI.Instance.ClearTargetCircle();
                TurnManager.Instance.UnlockStep();
                TurnManager.Instance.NextStep();
                break;
        }
    }

    // ==============================================================================
    // 视角
    // ==============================================================================
    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void SafeShowCrosshair()
    {
        if (CrosshairController.Instance != null)
            CrosshairController.Instance.Show();
    }

    private void SafeHideCrosshair()
    {
        if (CrosshairController.Instance != null)
            CrosshairController.Instance.Hide();
    }

    private void OnDrawGizmos()
    {
        if (playerCamera == null) return;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * detectionRange);
    }
}