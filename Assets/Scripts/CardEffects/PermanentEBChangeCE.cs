using UnityEngine;

public class PermanentEBChangeCE : CardEffect
{
    public float oxygenChange = 0.1f;
    public override void OnRemove(Player player)
    {
        if (oxygenChange > 0)
        {
            // player.oxygenProductionMinute += oxygenChange;
        }
        else
        {
            // player.oxygenComsumptionMinute += oxygenChange;
        }
    }
}
