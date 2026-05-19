using System;
using Random = UnityEngine.Random;

[Serializable]
public class AddCardOfRandomTypeOrAnotherСE : TriggeredCardEffect
{
    public CardType cardTypeToAdd;
    public int amountCardsToAdd = 1;
    public CardType anotherCardTypeToAdd;
    public float chanceToAddAnotherType = 0.1f;
    public PlayerCardPlace whereToAdd;

    public override string Description => $"Добавляет {(amountCardsToAdd == 1 ? "случайную карту" : $"{amountCardsToAdd} случайных карт")} типа {cardTypeToAdd.cardTypeName} в {AddCardСE.CardPlace(whereToAdd)}" +
        $", с шансом {(int)(chanceToAddAnotherType * 100)}% тип карты будет {anotherCardTypeToAdd.cardTypeName}";

    protected override void Apply(Player player) {
        for (int i = 0; i < amountCardsToAdd; i++) {
            CardType cardType = Random.value <= chanceToAddAnotherType ? anotherCardTypeToAdd : cardTypeToAdd;
            CardData cardToAdd = GameManager.I.GetRandomCardOfType(cardType);
            if (cardToAdd == null) return;
            player.GiveNewCardToPlayer(cardToAdd, whereToAdd);
        }
    }
}
