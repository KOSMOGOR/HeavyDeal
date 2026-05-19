using System;
using UnityEngine;

[Serializable]
public class FearOfDepthCE : CardEffect
{
    public float oxygenConsumptionDelta = 1f;
    [Range(0f, 1f)] public float riseProgressToRemove = 0.5f;

    public override string Description => $"Пока карта в кабине: потребление ЕВ {(oxygenConsumptionDelta >= 0f ? "+" : string.Empty)}{oxygenConsumptionDelta}. Удали эту карту, когда прогресс подъёма достигнет {(int)(riseProgressToRemove * 100f)}%";

    public override float OnEvaluateOxygenConsumption(Player player, CardInstance card, float oxygen)
    {
        if (card == null || player == null || card.player != player) return oxygen;
        return oxygen + oxygenConsumptionDelta;
    }

    public override void OnMinutePassInCabin(Player player, CardInstance card)
    {
        if (card == null || player == null || card.player != player) return;
        if (GetRiseProgress(player) < Mathf.Clamp01(riseProgressToRemove)) return;

        player.RemoveCard(card);
    }

    float GetRiseProgress(Player player)
    {
        if (player.baseDistanceToSurface <= 0f) return 1f;

        float progress = 1f - player.distanceToSurface / player.baseDistanceToSurface;
        return Mathf.Clamp01(progress);
    }
}
