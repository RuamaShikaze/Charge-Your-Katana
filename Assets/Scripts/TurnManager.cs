using UnityEngine;

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
        Debug.Log("=== 玩家回合开始 ===");
        currentTurn = TurnOwner.Player;

        // 新回合开始 → 强制清空UI
        ClearPlayerUI();

        SwitchState(TurnState.SelectElement);
    }

    public void StartEnemyTurn()
    {
        Debug.Log("=== 敌方回合开始 ===");
        currentTurn = TurnOwner.Enemy;
        if (enemy != null)
            enemy.AIStartTurn();
    }

    public void SwitchState(TurnState newState)
    {
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

    // 回合结束 → 清空UI
    private void EndTurn()
    {
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
        if (currentTurn == TurnOwner.Player)
            StartEnemyTurn();
        else
            StartPlayerTurn();
    }
}