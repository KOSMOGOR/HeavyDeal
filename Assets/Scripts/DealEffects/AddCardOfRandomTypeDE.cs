using System;

[Serializable]
public class AddCardOfRandomTypeDE : DealEffect
{
    public CardType cardTypeToAdd;
    public int amountCardsToAdd = 1;
    public PlayerCardPlace whereToAdd;

    public override string Description => $"Добавляет {(amountCardsToAdd == 1 ? "случайную карту" : $"{amountCardsToAdd} случайных карт")} типа {cardTypeToAdd.cardTypeName} в {AddCardDE.CardPlace(whereToAdd)}";

    public override void ApplyEffect(Player player) {
        for (int i = 0; i < amountCardsToAdd; i++) {
            CardData cardToAdd = GameManager.I.GetRandomCardOfType(cardTypeToAdd);
            if (cardToAdd == null) return;
            player.GiveNewCardToPlayer(cardToAdd, whereToAdd);
        }
    }
}
