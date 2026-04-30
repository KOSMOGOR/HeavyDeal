using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int currentOxygenTank = 20;
    public int oxygenTanks = 2;

    List<CardInstance> deck = new();
    List<CardInstance> hand = new();
    List<CardInstance> discard = new();

    public CardInstance selectedCard;
    public CardInstance cardInPlay;
    public List<GameEffectInstance> playerGameEffects = new();

    void Start() {
        GameManager.I.RegisterPlayer(this);
    }

    public void SelectCard(CardInstance card) {
        selectedCard = card;
    }

    public void PlaySelectedCard() {
        if (selectedCard != null && hand.Contains(selectedCard) && cardInPlay == null) {
            hand.Remove(selectedCard);
            cardInPlay = selectedCard;
            selectedCard = null;
            GameManager.I.PlayCard(this, cardInPlay);
        }
    }

    public void DiscardCardInPlay() {
        if (cardInPlay != null) {
            discard.Add(cardInPlay);
            cardInPlay = null;
        }
    }

    public void AddPlayerGameEffects(CardInstance card) {
        card.cardData.playerGameEffectsOnResolve.ForEach(ge => GameEffectInstance.CreateAndAdd(ge, playerGameEffects));
    }
}
