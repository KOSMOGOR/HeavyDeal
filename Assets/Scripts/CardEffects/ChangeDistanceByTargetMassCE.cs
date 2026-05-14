using System;

[Serializable]
public class ChangeDistanceByTargetMassCE : TriggeredCardEffect
{
    public float massMultiplier = 1f;
    public bool removeTargetCard;

    public override bool RequiresTarget => true;

    public override string Description => DescribeWithTrigger($"Изменяет глубину на вес выбранной карты x {massMultiplier}{(removeTargetCard ? " и удаляет её" : "")}");

    protected override void Apply(Player player)
    {
        if (player == null || player.cardInPlay == null) return;
        CardInstance targetCard = player.cardInPlay.targetCard;
        if (targetCard == null || targetCard.player != player) return;

        player.distanceToSurface += targetCard.EvaluateMass() * massMultiplier;
        if (removeTargetCard) player.RemoveCard(targetCard);
    }
}
