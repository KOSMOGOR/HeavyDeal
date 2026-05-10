using System;

[Serializable]
public class DealEffectAddCard : DealEffect
{
    public CardData cardToAdd;
    public PlayerCardPlace whereToAdd;

    public override string Description => $"Добавляет {cardToAdd.cardName} в {CardPlace(whereToAdd)}";

    public override void ApplyEffect(Player player) {
        player.GiveNewCardToPlayer(cardToAdd, whereToAdd);
    }

    string CardPlace(PlayerCardPlace cardPlace) {
        return cardPlace switch {
            PlayerCardPlace.Hand => "руку",
            PlayerCardPlace.Deck => "колоду",
            PlayerCardPlace.Discard => "сброс",
            _ => "-"
        };
    }
}
