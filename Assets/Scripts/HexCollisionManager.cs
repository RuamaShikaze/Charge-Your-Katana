using UnityEngine;

public static class HexCollisionManager
{
    public static void HandleCollision(HexGridUnit attacker, HexGridUnit target)
    {
        if (attacker.GetComponent<Wall>() != null || target.GetComponent<Wall>() != null)
        {
            return;
        }
        EnemyCharge ecA = attacker.GetComponent<EnemyCharge>();
        EnemyCharge ecT = target.GetComponent<EnemyCharge>();

        if (ecA == null || ecT == null) return;
        if (ecA.IsExcited || ecT.IsExcited) return;

        // 同电性
        if (ecA.currentCharge == ecT.currentCharge)
        {
            int x = ecA.currentChargeCount;
            int y = ecT.currentChargeCount;
            int sum = x + y;

            int targetGet = Mathf.CeilToInt(sum / 2f);
            int attackerGet = sum - targetGet;

            ecT.SetChargeCount(targetGet);
            ecA.SetChargeCount(attackerGet);

            attacker.StopMove();
            target.StartMove(target.CurrentMoveDir, attacker.RemainingSteps);
        }
        // 异电性
        else
        {
            int x = ecA.currentChargeCount;
            int y = ecT.currentChargeCount;

            if (x == y)
            {
                ecA.DestroySelf();
                ecT.DestroySelf();
                return;
            }

            EnemyCharge winner = x > y ? ecA : ecT;
            EnemyCharge loser = x > y ? ecT : ecA;

            winner.SetChargeCount(Mathf.Abs(winner.currentChargeCount - loser.currentChargeCount));
            loser.DestroySelf();

            attacker.StopMove();
            target.StopMove();
        }

        attacker.UpdateChargeUI();
        target.UpdateChargeUI();
    }
}