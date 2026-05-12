using UnityEngine;

public class PauseOxygenCE : CardEffect
{
    public float oxygenChange = 0f;

    public override void OnPlay(Player player)
    {
        // player.oxygenConsumptionPauseCount += 1;
    }
    public override void OnDiscard(Player player)
    {
        // player.oxygenConsumptionPauseCount = Mathf.Max(0, player.oxygenConsumptionPauseCount - 1);
    }
}
