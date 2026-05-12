using UnityEngine;

public class ManageOxygenCE : CardEffect
{
    public float oxygenConsumption = 0f;

    public override void OnPlay(Player player)
    {
        player.oxygenComsumptionMinute += oxygenConsumption;
    }
    public override void OnDiscard(Player player)
    {
        player.oxygenComsumptionMinute -= oxygenConsumption;
    }
}
