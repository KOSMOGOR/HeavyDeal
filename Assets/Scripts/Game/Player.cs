using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Data")]
    public int currentOxygenTank = 20;
    public int oxygenTanks = 2;
    public int oxygenPerTank = 20;
    // public float distanceToSurface = 500f;
    public CardInstance selectedCard;
    public CardInstance cardInPlay;
    public List<GameEffectInstance> playerGameEffects = new();

    [Header("Card Places")]
    public CardInstance cardInstancePrefab;
    public Transform handCenter;
    public float handDeltaDistance;
    public Transform cardPlayZone;
    public Transform cardDeckZone;
    public Transform cardDiscardZone;

    List<CardInstance> deck = new(); // 0 - left card
    List<CardInstance> hand = new(); // 0 - top of the deck
    List<CardInstance> discard = new(); // 0 - bottom of discard

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

    public void AddCardToHand(CardInstance card) {
        hand.Add(card);
        card.SetCardActive(true);
        RepositionCards();
    }

    public void DrawCard() {
        if (deck.Count == 0 && discard.Count > 0) ShuffleDiscardToDeck();
        if (deck.Count > 0) {
            CardInstance card = deck[0];
            deck.RemoveAt(0);
            AddCardToHand(card);
        }
    }

    public float EvaluateTotalMass() {
        return (cardInPlay != null ? cardInPlay.EvaluateMass() : 0) + EvaluateListMass(hand) + EvaluateListMass(deck) + EvaluateListMass(discard);
    }

    float EvaluateListMass(List<CardInstance> cards) {
        return cards.Sum(card => card.EvaluateMass());
    }

    public CardInstance GiveNewCardToPlayer(CardData cardData, PlayerCardPlace place = PlayerCardPlace.Deck) {
        if (place == PlayerCardPlace.InPlay) throw new System.Exception("Use PlaySelectedCard() for this");
        CardInstance card = Instantiate(cardInstancePrefab);
        card.cardData = cardData;
        card.player = this;
        if (place == PlayerCardPlace.Hand) AddCardToHand(card);
        else  {
            card.SetCardActive(false);
            if (place == PlayerCardPlace.Deck) deck.Add(card);
            else if (place == PlayerCardPlace.Discard) discard.Add(card);
        }
        return card;
    }
}

public enum PlayerCardPlace {
    InPlay,
    Hand,
    Deck,
    Discard
}
