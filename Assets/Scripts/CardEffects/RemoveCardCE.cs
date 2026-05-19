using System;

[Serializable]
public class RemoveCardCE : TriggeredCardEffect
{
    public int cardsToDiscard = 1;

    public override bool RequiresTarget => true;

    public override string Description => DescribeWithTrigger("Удали карту в руке");

    protected override void Apply(Player player) {
        if (player == null || player.cardInPlay == null || player.Hand.Count <= 0) return;

        CardInstance targetCard = player.cardInPlay.targetCard;
        if (targetCard == null || targetCard.player != player) return;

        player.RemoveCard(targetCard);
    }
}
