using System;

[Serializable]
public class SetTargetCardDataCE : TriggeredCardEffect
{
    public CardData newCardData;

    public override bool RequiresTarget => true;
    public override int MinimumOtherHandCardsToPlay => 1;

    public override string Description => DescribeWithTrigger(
        $"Превращает выбранную карту в \"{(newCardData != null ? newCardData.cardName : "?")}\"");

    protected override void Apply(Player player)
    {
        if (player == null || player.cardInPlay == null || newCardData == null || player.Hand.Count <= 0) return;

        CardInstance targetCard = player.cardInPlay.targetCard;
        if (targetCard == null || targetCard.player != player || !player.Hand.Contains(targetCard)) return;

        targetCard.SetCardData(newCardData);
        player.CheckDeadCardsLoseCondition();
    }
}
