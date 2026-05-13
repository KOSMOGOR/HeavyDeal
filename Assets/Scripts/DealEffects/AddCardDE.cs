using System;

[Serializable]
public class AddCardDE : DealEffect
{
    public CardData cardToAdd;
    public int amountCardsToAdd = 1;
    public PlayerCardPlace whereToAdd;

    public override string Description => $"Добавляет {amountCardsToAdd} {cardToAdd.cardName} в {CardPlace(whereToAdd)}";

    public override void ApplyEffect(Player player) {
        for (int i = 0; i < amountCardsToAdd; i++) player.GiveNewCardToPlayer(cardToAdd, whereToAdd);
    }

    public static string CardPlace(PlayerCardPlace cardPlace) {
        return cardPlace switch {
            PlayerCardPlace.Hand => "руку",
            PlayerCardPlace.Deck => "колоду",
            PlayerCardPlace.Discard => "сброс",
            _ => "-"
        };
    }
}
