using System;
using UnityEngine;

[Serializable]
public class ChangeCurrentOxygenCE : CardEffect
{
    public float oxygenDelta;

    public override string Description => oxygenDelta >= 0
        ? $"Восстанавливает {oxygenDelta} ЕВ"
        : $"Тратит {Mathf.Abs(oxygenDelta)} ЕВ";

    public override void OnResolve(Player player)
    {
        if (player == null) return;
        player.currentOxygenTank += oxygenDelta;
    }
}
