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
    public int baseOxygenPerMinute = 1;
    // public float distanceToSurface = 500f;
    public float minCardMass = 30f, maxCardMass = 60f;
    public PlayerState playerState = PlayerState.Regular;
    public CardInstance selectedCard;
    public CardInstance cardInPlay;
    public List<GameEffectInstance> playerGameEffects = new();

    [Header("Card Places")]
    public CardInstance cardInstancePrefab;
    public Transform handCenter;
    public float handDeltaDistance = 1f;
    public Transform cardDeckZone;
    public Transform cardDiscardZone;
    public Transform cardPlayZone;

    readonly List<CardInstance> hand = new(); // 0 - top of the deck
    readonly List<CardInstance> deck = new(); // 0 - left card
    readonly List<CardInstance> discard = new(); // 0 - bottom of discard

    void Start() {
        GameManager.I.RegisterPlayer(this);
        CardData cardData = Resources.Load<CardData>("CardDatas/CardThink");
        for (int i = 0; i < 5; i++) GiveNewCardToPlayer(cardData);
    }

    public void TrySelectCard(CardInstance card) {
        if (playerState != PlayerState.Regular) return;
        if (cardInPlay && cardInPlay.cardData.CardRequiresTarget && !cardInPlay.targetCard) {
            cardInPlay.targetCard = card;
        } else {
            if (selectedCard) selectedCard.transform.DOScale(1f, 0.5f);
            selectedCard = card;
            card.transform.DOScale(1.1f, 0.5f);
        }
    }

    public void PlaySelectedCard() {
        if (playerState != PlayerState.Regular) return;
        if (selectedCard != null && hand.Contains(selectedCard) && cardInPlay == null) {
            hand.Remove(selectedCard);
            cardInPlay = selectedCard;
            cardInPlay.transform.DOScale(1f, 0.25f);
            cardInPlay.transform.DOMove(cardPlayZone.position, 0.5f).SetEase(Ease.OutCubic);
            cardInPlay.transform.SetParent(cardPlayZone);
            selectedCard = null;
            GameManager.I.PlayCard(this, cardInPlay);
        }
    }

    public void DiscardCardInPlay() {
        if (cardInPlay != null) {
            discard.Add(cardInPlay);
            cardInPlay.SetCardActive(false);
            cardInPlay.transform.position = cardDiscardZone.transform.position;
            cardInPlay.transform.SetParent(cardDiscardZone);
            cardInPlay = null;
        }
    }

    public void AddPlayerGameEffects(CardInstance card) {
        card.cardData.playerGameEffectsOnResolve.ForEach(ge => GameEffectInstance.CreateAndAdd(ge, playerGameEffects));
    }

    void RepositionCardsInHand() {
        if (hand.Count == 0) return;
        float startX = handCenter.position.x - (hand.Count - 1) * handDeltaDistance / 2;
        for (int i = 0; i < hand.Count; i++) hand[i].transform.DOMove(new(startX + handDeltaDistance * i, handCenter.position.y, handCenter.position.z), 1f).SetEase(Ease.OutCubic);
    }

    void ShuffleDiscardToDeck() {
        deck.AddRange(discard);
        discard.ForEach(card => {
            card.transform.position = cardDeckZone.transform.position;
            card.transform.SetParent(cardDeckZone);
        });
        discard.Clear();
        deck.OrderBy(_ => Random.value);
    }

    public void AddCardToHand(CardInstance card) {
        hand.Add(card);
        card.SetCardActive(true);
        card.transform.SetParent(handCenter);
        RepositionCardsInHand();
    }

    public void DrawCard() {
        if (deck.Count == 0 && discard.Count > 0) ShuffleDiscardToDeck();
        if (deck.Count > 0) {
            CardInstance card = deck[0];
            deck.RemoveAt(0);
            card.CardReset();
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
        card.SetCardData(cardData);
        card.player = this;
        card.baseMass = Random.Range(minCardMass, maxCardMass);
        if (place == PlayerCardPlace.Hand) AddCardToHand(card);
        else  {
            card.SetCardActive(false);
            if (place == PlayerCardPlace.Deck) {
                card.transform.position = cardDeckZone.transform.position;
                card.transform.SetParent(cardDeckZone.transform);
                deck.Add(card);
            }
            else if (place == PlayerCardPlace.Discard) {
                card.transform.position = cardDiscardZone.transform.position;
                card.transform.SetParent(cardDiscardZone.transform);
                discard.Add(card);
            }
        }
        return card;
    }

    public void RemoveCard(CardInstance card) {
        if (card.player != this || card == cardInPlay) return;
        if (hand.Contains(card)) {
            hand.Remove(card);
            RepositionCardsInHand();
        } else {
            deck.Remove(card);
            discard.Remove(card);
        }
    }

    public void ResolveMinutePassPlayer() {
        if (cardInPlay != null) {
            cardInPlay.remainInPlay -= 1;
            if (cardInPlay.remainInPlay <= 0) {
                GameManager.I.ResolveCard(cardInPlay);
                DiscardCardInPlay();
            }
        }
        currentOxygenTank -= EvaluateOxygenPerMinute();
        if (currentOxygenTank <= 0) {
            if (oxygenTanks > 0) {
                oxygenTanks--;
                currentOxygenTank += oxygenPerTank;
                DealsManager.I.StartDealSelect(this);
            } else currentOxygenTank = 0;
        }
    }

    public bool IsReadyForMinutePass => playerState == PlayerState.Regular && cardInPlay && (!cardInPlay.cardData.CardRequiresTarget || cardInPlay.targetCard);

    int EvaluateOxygenPerMinute() {
        return baseOxygenPerMinute;
    }

    public void ChangeState(PlayerState newPlayerState) {
        playerState = newPlayerState;
    }
}

public enum PlayerState {
    Regular,
    Deal
}

public enum PlayerCardPlace {
    Hand,
    Deck,
    Discard,
    InPlay
}
