using System;

[Serializable]
public class ChangeCurrentOxygenByTargetMassCE : CardEffect
{
    public float massMultiplier = 1f;
    public bool removeTargetCard;

    public override bool RequiresTarget => true;

    public override string Description => $"Изменяет ЕВ на вес выбранной карты x {massMultiplier}{(removeTargetCard ? " и удаляет её" : "")}";

    public override void OnResolve(Player player)
    {
        if (player == null || player.cardInPlay == null) return;
        CardInstance targetCard = player.cardInPlay.targetCard;
        if (targetCard == null || targetCard.player != player) return;

        player.currentOxygenTank += targetCard.EvaluateMass() * massMultiplier;
        if (removeTargetCard) player.RemoveCard(targetCard);
    }
}
