using UnityEngine;
using System.Collections;
using TMPro;

public class HexGridUnit : MonoBehaviour
{
    [Header("网格绑定")]
    public HexCoord currentHex;
    public bool autoSnapOnStart = true;

    [Header("移动动画")]
    public float moveSpeed = 8f;
    public bool isMoving = false;

    [Header("电荷UI")]
    public TextMeshPro chargeText;

    public Coroutine moveCoroutine;
    private Vector3 targetWorldPos;
    private int remainingSteps = 0;
    private int currentMoveDir = -1;

    private void Start()
    {
        if (autoSnapOnStart)
            SnapToGrid();

        UpdateChargeUI();
    }

    public void StartMove(int dir, int totalSteps)
    {
        if (isMoving) return;

        remainingSteps = totalSteps;
        currentMoveDir = dir;
        PrepareNextStep();
    }

    // 移动前检查：预判碰撞（你要的核心逻辑）
    private void PrepareNextStep()
    {
        if (remainingSteps <= 0)
        {
            isMoving = false;
            return;
        }

        HexCoord nextHex = currentHex.Neighbor(currentMoveDir);
        HexGridUnit occupier = FindUnitOnHex(nextHex);
        bool isWall = CheckWall(nextHex);

        // 撞墙反弹
        if (isWall)
        {
            currentMoveDir = (currentMoveDir + 3) % 6;
            PrepareNextStep();
            return;
        }

        // A 试图进入 B 的格子 → 直接碰撞
        if (occupier != null && occupier != this)
        {
            HexCollisionManager.HandleCollision(this, occupier);
            isMoving = false;
            return;
        }

        // 无阻碍，移动
        MoveToNextHex(nextHex);
    }

    private void MoveToNextHex(HexCoord nextHex)
    {
        currentHex = nextHex;
        targetWorldPos = HexGridManager.Instance.HexToWorld(currentHex);
        targetWorldPos.y = transform.position.y;
        moveCoroutine = StartCoroutine(SmoothMove());
    }

    private IEnumerator SmoothMove()
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, targetWorldPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetWorldPos;
        remainingSteps--;
        isMoving = false;
        PrepareNextStep();
    }

    public void StopMove()
    {
        remainingSteps = 0;
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        isMoving = false;
    }

    public HexGridUnit FindUnitOnHex(HexCoord hex)
    {
        foreach (var u in FindObjectsOfType<HexGridUnit>())
            if (u.currentHex == hex && u != this)
                return u;
        return null;
    }

    public void SnapToGrid()
    {
        currentHex = HexGridManager.Instance.WorldToHex(transform.position);
        targetWorldPos = HexGridManager.Instance.HexToWorld(currentHex);
        transform.position = new Vector3(targetWorldPos.x, transform.position.y, targetWorldPos.z);
    }

    public void TeleportToHex(HexCoord coord)
    {
        currentHex = coord;
        targetWorldPos = HexGridManager.Instance.HexToWorld(currentHex);
        transform.position = new Vector3(targetWorldPos.x, transform.position.y, targetWorldPos.z);
    }

    // 检查指定格子是否为墙体（不可通行）
    public bool CheckWall(HexCoord hex)
    {
        // 遍历场景内所有墙体
        Wall[] allWalls = Object.FindObjectsOfType<Wall>();
        foreach (var wall in allWalls)
        {
            if (wall.GetWallCoord() == hex)
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateChargeUI()
    {
        if (chargeText == null) return;
        var ec = GetComponent<EnemyCharge>();
        chargeText.text = ec.currentChargeCount.ToString();
    }

    public HexCoord GetCurrentHex()
    {
        return currentHex;
    }

    public int RemainingSteps => remainingSteps;
    public int CurrentMoveDir => currentMoveDir;
    public void SetRemainingSteps(int s) => remainingSteps = s;
    public void MoveToDir(int dir, int step = 1) => StartMove(dir, step);
}