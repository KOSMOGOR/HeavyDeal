using System;
using System.Linq;

[Serializable]
public class RemoveAllMatchingCardsInPlaceCE : TriggeredCardEffect
{
    public PlayerCardPlace targetPlace = PlayerCardPlace.Hand;
    public CardType targetCardType;
    public CardData targetCardData;

    public override string Description
    {
        get
        {
            string filterDescription = targetCardData != null
                ? $"все карты \"{targetCardData.cardName}\""
                : targetCardType != null
                    ? $"все карты типа \"{targetCardType.cardTypeName}\""
                    : "все карты";
            return DescribeWithTrigger($"Удали {filterDescription} из {PlaceToText(targetPlace)}");
        }
    }

    protected override void Apply(Player player)
    {
        if (player == null) return;
        CardInstance[] cardsToRemove = player.GetCardListInPlace(targetPlace)
            .Where(card => card != null && Matches(card))
            .ToArray();
        player.RemoveCards(cardsToRemove);
    }

    bool Matches(CardInstance card)
    {
        if (targetCardData != null) return card.cardData == targetCardData;
        if (targetCardType != null) return card.cardData.cardType == targetCardType;
        return true;
    }

    static string PlaceToText(PlayerCardPlace place) => place switch
    {
        PlayerCardPlace.Hand => "руки",
        PlayerCardPlace.Deck => "колоды",
        PlayerCardPlace.Discard => "сброса",
        PlayerCardPlace.InPlay => "розыгрыша",
        _ => "зоны"
    };
}
