using System;

[Serializable]
public class GiveSpecificCardCE : TriggeredCardEffect
{
    public CardData cardData;
    public PlayerCardPlace targetPlace = PlayerCardPlace.Hand;
    public int cardsToGive = 1;

    public override string Description => DescribeWithTrigger($"Добавь {cardsToGive} {(cardsToGive == 1 ? "карту" : cardsToGive <= 4 ? "карты" : "карт")} \"{(cardData != null ? cardData.cardName : "?")}\" в {PlaceToText(targetPlace)}");

    protected override void Apply(Player player)
    {
        if (player == null || cardData == null || cardsToGive <= 0) return;
        for (int i = 0; i < cardsToGive; i++) {
            player.GiveNewCardToPlayer(cardData, targetPlace);
        }
    }

    static string PlaceToText(PlayerCardPlace place) => place switch
    {
        PlayerCardPlace.Hand => "руку",
        PlayerCardPlace.Deck => "колоду",
        PlayerCardPlace.Discard => "сброс",
        PlayerCardPlace.InPlay => "розыгрыш",
        _ => "зону"
    };
}
