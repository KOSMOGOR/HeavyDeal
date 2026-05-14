using System;
using UnityEngine;

[Serializable]
public class ChangeCurrentOxygenCE : TriggeredCardEffect
{
    public float oxygenDelta;

    public override string Description => DescribeWithTrigger(oxygenDelta >= 0
        ? $"Восстанавливает {oxygenDelta} ЕВ"
        : $"Тратит {Mathf.Abs(oxygenDelta)} ЕВ");

    protected override void Apply(Player player)
    {
        if (player == null) return;
        player.currentOxygenTank += oxygenDelta;
    }
}
