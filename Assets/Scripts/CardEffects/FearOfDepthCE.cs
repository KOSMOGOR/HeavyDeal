using System;
using UnityEngine;

[Serializable]
public class FearOfDepthCE : CardEffect
{
    public float oxygenConsumptionDelta = 1f;
    [Range(0f, 1f)] public float riseProgressToRemove = 0.5f;

    public override string Description => $"Пока карта в кабине: потребление ЕВ +{oxygenConsumptionDelta}. TODO: удалить карту при достижении {(int)(riseProgressToRemove * 100f)}% подъёма";

    public override float OnEvaluateOxygenConsumption(Player player, CardInstance card, float oxygen)
    {
        // TODO: apply passive oxygen consumption while the card is in cabin
        return oxygen;
    }

    public override void OnMinutePassInCabin(Player player, CardInstance card)
    {
        // TODO: remove the card when rise progress reaches the configured threshold
    }
}
