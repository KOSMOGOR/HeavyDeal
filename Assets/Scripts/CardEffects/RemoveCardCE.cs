using System;

[Serializable]
public class RemoveCardCE : CardEffect
{
    public int cardsToDiscard = 1;

    public override bool RequiresTarget => true;

    public override void OnResolve(Player player) {
        if (player == null || player.cardInPlay == null || player.Hand.Count <= 0) return;

        CardInstance targetCard = player.cardInPlay.targetCard;
        if (targetCard == null || targetCard.player != player) return;

        player.RemoveCard(targetCard);
    }
}
