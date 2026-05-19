using System;
using System.Linq;

[Serializable]
public class PlaySelfIfOtherCardTypeInHandOnDrawCE : CardEffect
{
    public CardType targetCardType;

    public override string Description => $"При получении: если в руке есть другая карта типа \"{targetCardType?.cardTypeName ?? "?"}\", разыграй эту карту";

    public override void OnDraw(Player player, CardInstance drawnCard)
    {
        if (player == null || drawnCard == null) return;
        if (targetCardType == null || drawnCard.player != player || !player.Hand.Contains(drawnCard)) return;

        bool hasOtherMatchingCard = player.Hand.Any(card => card != drawnCard && card.cardData.cardType == targetCardType);
        if (!hasOtherMatchingCard) return;

        player.PlayCardFromHand(drawnCard);
    }
}
