using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public enum TurnState
    {
        SelectElement,
        SelectWeapon,
        FindTarget,
        LockTarget,
        Attack,
        TurnEnd
    }
    public enum TurnOwner
    {
        Player,
        Enemy
    }

    [Header("引用")]
    public PlayerController player;
    public EnemyController enemy;
    [HideInInspector] public TurnState currentState;
    [HideInInspector] public TurnOwner currentTurn;
    private bool _waitForAnimation;

    // ========= 新增：游戏结束标记 =========
    [HideInInspector]
    public bool isGameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartPlayerTurn();
        SoundManager.Instance.PlayBGM();
    }

    public void StartPlayerTurn()
    {
        // 游戏结束，不再开启新回合
        if (isGameOver) return;

        Debug.Log("=== 玩家回合开始 ===");
        currentTurn = TurnOwner.Player;
        // 新回合开始 → 强制清空UI
        ClearPlayerUI();
        SwitchState(TurnState.SelectElement);
    }

    public void StartEnemyTurn()
    {
        // 游戏结束，不再开启新回合
        if (isGameOver) return;

        Debug.Log("=== 敌方回合开始 ===");
        currentTurn = TurnOwner.Enemy;
        if (enemy != null)
            enemy.AIStartTurn();
    }

    public void SwitchState(TurnState newState)
    {
        // 游戏结束，禁止切换状态
        if (isGameOver) return;

        currentState = newState;
        Debug.Log($"【{currentTurn}】阶段：{currentState}");
        if (currentTurn == TurnOwner.Player)
            player.OnTurnStateUpdated(newState);
        else if (enemy != null)
            enemy.OnTurnStateUpdated(newState);
    }

    public void NextStep()
    {
        if (_waitForAnimation) return;
        // 游戏结束，禁止推进步骤
        if (isGameOver) return;

        if (currentState == TurnState.Attack)
        {
            EndTurn();
            return;
        }
        SwitchState((TurnState)(int)currentState + 1);
    }

    public void PrevStep()
    {
        if (_waitForAnimation) return;
        // 游戏结束，禁止回退步骤
        if (isGameOver) return;

        switch (currentState)
        {
            case TurnState.SelectWeapon:
                SwitchState(TurnState.SelectElement);
                break;
            case TurnState.FindTarget:
                SwitchState(TurnState.SelectWeapon);
                break;
            case TurnState.LockTarget:
                SwitchState(TurnState.FindTarget);
                break;
        }
    }

    public void LockStep()
    {
        _waitForAnimation = true;
    }

    public void UnlockStep()
    {
        _waitForAnimation = false;
    }

    // 回合结束 → 清空UI + 检测游戏结束
    private void EndTurn()
    {
        if (isGameOver) return;

        SwitchState(TurnState.TurnEnd);
        Debug.Log("=== 回合结束 ===");
        // 核心修复：回合结束清空UI
        ClearPlayerUI();
        Invoke(nameof(SwitchTurn), 0.8f);

        // 回合结束，清空所有敌人激发态
        EnemyCharge[] allEnemies = FindObjectsOfType<EnemyCharge>();
        foreach (var ec in allEnemies)
        {
            ec.SetExcited(false);
        }

        // ========= 新增：检测所有敌人是否已消灭 =========
        CheckAllEnemyDefeated();
    }

    /// <summary>检测全场敌人，无敌人则游戏结束</summary>
    public void CheckAllEnemyDefeated()
    {
        EnemyCharge[] allEnemies = FindObjectsOfType<EnemyCharge>();
        if (allEnemies.Length <= 0)
        {
            GameOver();
        }
    }

    /// <summary>游戏结束逻辑</summary>
    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("所有敌人已清除，游戏结束！");
        // 暂停游戏
        Time.timeScale = 1f;

        // 延迟2秒后 自动返回选关界面
        Invoke(nameof(BackToSelectLevel), 2f);

        // 可自行拓展：弹出胜利UI、播放胜利音效、跳转场景等
    }

    // 清空左手电性、右手武器UI
    public void ClearPlayerUI()
    {
        if (BattleUI.Instance != null)
        {
            BattleUI.Instance.HideAll();
        }
    }

    private void SwitchTurn()
    {
        // 游戏结束，不再切换回合
        if (isGameOver) return;

        if (currentTurn == TurnOwner.Player)
            StartEnemyTurn();
        else
            StartPlayerTurn();
    }

    private void BackToSelectLevel()
    {
        SceneManager.LoadScene("SelectLevel");
    }
}