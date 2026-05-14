using System;

[Serializable]
public class DontDiscardCardTypeOnWaitGE : GameEffect
{
    public bool allCards;
    public CardType cardType;

    public override string DescriptionEffect => "Во время ожидания ты не сбрасываешь карты" + (allCards ? "" : $" типа {cardType.cardTypeName}");

    public override bool OnEvaluateWaitingDiscard(bool discard, CardInstance card) => !allCards && card.cardData.cardType != cardType;
}
