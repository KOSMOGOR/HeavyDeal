using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int currentOxygenTank = 20;
    public int oxygenTanks = 2;
    public Transform handCenter;
    public float handDeltaDistance;
    public CardInstance cardInstancePrefab;

    List<CardInstance> deck = new(); // 0 - left card
    List<CardInstance> hand = new(); // 0 - top of the deck
    List<CardInstance> discard = new(); // 0 - bottom of discard

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
            cardInPlay.SetCardActive(true);
            cardInPlay = null;
        }
    }

    public void AddPlayerGameEffects(CardInstance card) {
        card.cardData.playerGameEffectsOnResolve.ForEach(ge => GameEffectInstance.CreateAndAdd(ge, playerGameEffects));
    }

    void RepositionCards() {
        if (hand.Count == 0) return;
        float startX = handCenter.position.x - (hand.Count - 1) * handDeltaDistance / 2;
        for (int i = 0; i < hand.Count; i++) hand[i].transform.DOMove(new(startX + handDeltaDistance * i, handCenter.position.y, handCenter.position.z), 1).SetEase(Ease.OutCubic);
    }

    void ShuffleDiscardToDeck() {
        deck.AddRange(discard);
        discard.Clear();
        deck.OrderBy(_ => Random.value);
    }

    public void DrawCard() {
        if (deck.Count == 0) ShuffleDiscardToDeck();
        if (deck.Count > 0) {
            CardInstance card = deck[0];
            deck.RemoveAt(0);
            hand.Add(card);
            card.SetCardActive(true);
            RepositionCards();
        }
    }
}
