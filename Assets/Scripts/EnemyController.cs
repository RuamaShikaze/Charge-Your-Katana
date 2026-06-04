using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private ElementType _element;
    private WeaponType _weapon;
    private Transform _target;

    public void AIStartTurn()
    {
        SwitchState(TurnManager.TurnState.SelectElement);
    }

    public void OnTurnStateUpdated(TurnManager.TurnState state)
    {
        // 只有敌方回合才执行！
        if (TurnManager.Instance.currentTurn != TurnManager.TurnOwner.Enemy)
            return;

        switch (state)
        {
            case TurnManager.TurnState.SelectElement: AIPickElement(); break;
            case TurnManager.TurnState.SelectWeapon: AIPickWeapon(); break;
            case TurnManager.TurnState.FindTarget: AIFindTarget(); break;
            case TurnManager.TurnState.LockTarget: AILockTarget(); break;
            case TurnManager.TurnState.Attack: AIAttack(); break;
        }
    }

    private void SwitchState(TurnManager.TurnState s)
    {
        TurnManager.Instance.SwitchState(s);
    }

    private void AIPickElement()
    {
        _element = Random.Range(0, 2) == 0 ? ElementType.PositiveElectricity : ElementType.NegativeElectricity;
        Debug.Log("敌方选择元素：" + _element);
        TurnManager.Instance.NextStep();
    }

    private void AIPickWeapon()
    {
        _weapon = WeaponType.Katana;
        Debug.Log("敌方选择武器：" + _weapon);
        TurnManager.Instance.NextStep();
    }

    private void AIFindTarget()
    {
        _target = TurnManager.Instance.player.transform;
        Debug.Log("敌方找到目标：玩家");
        TurnManager.Instance.NextStep();
    }

    private void AILockTarget()
    {
        Debug.Log("敌方锁定：玩家");
        TurnManager.Instance.NextStep();
    }

    private void AIAttack()
    {
        Debug.Log($"【敌方攻击】{_element} + {_weapon} → 玩家");
        TurnManager.Instance.NextStep();
    }
}