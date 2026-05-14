using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Data")]
    public float currentOxygenTank = 20f;
    public int oxygenTanks = 2;
    public float oxygenPerTank = 20f;
    public float baseOxygenComsumptionMinute = 1f;
    public float baseOxygenProductionMinute = 0f;
    public float distanceToSurface = 500f;
    public float massToRise = 200f;
    public float coefMassToSpeed = 0.5f;
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

    [Header("Initial")]
    public List<CardData> startDeck;

    readonly List<CardInstance> hand = new(); // 0 - top of the deck
    public List<CardInstance> Hand => hand;
    readonly List<CardInstance> deck = new(); // 0 - left card
    readonly List<CardInstance> discard = new(); // 0 - bottom of discard

    void Start() {
        GameManager.I.RegisterPlayer(this);
        CardData cardData = Resources.Load<CardData>("CardDatas/CardThink");
        for (int i = 0; i < 5; i++) GiveNewCardToPlayer((startDeck.Count() > i && startDeck[i] != null) ? startDeck[i] : cardData);
    }

    public void TrySelectCard(CardInstance card) {
        if (playerState != PlayerState.Regular || !hand.Contains(card)) return;
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
        if (selectedCard != null && hand.Contains(selectedCard) && selectedCard.cardData.canBePlayed && cardInPlay == null) {
            hand.Remove(selectedCard);
            cardInPlay = selectedCard;
            SetCardPlace(cardInPlay, PlayerCardPlace.InPlay);
            cardInPlay.transform.DOScale(1f, 0.25f);
            cardInPlay.transform.DOMove(cardPlayZone.position, 0.5f).SetEase(Ease.OutCubic);
            cardInPlay.transform.SetParent(cardPlayZone);
            selectedCard = null;
            GameManager.I.PlayCard(this, cardInPlay);
        }
    }

    public void DiscardCardInPlay() {
        if (cardInPlay != null) DiscardCard(cardInPlay);
    }

    public void DiscardCard(CardInstance card) {
        if (card == null || card.player != this) return;

        if (card == cardInPlay) {
            card.cardData.cardEffects.ForEach(ce => ce.OnDiscard(this));
            discard.Add(card);
            SetCardPlace(card, PlayerCardPlace.Discard);
            card.SetCardActive(false);
            card.transform.position = cardDiscardZone.transform.position;
            card.transform.SetParent(cardDiscardZone);
            cardInPlay = null;
            return;
        }

        bool wasDiscarded = false;

        if (hand.Remove(card)) {
            wasDiscarded = true;
            if (selectedCard == card) selectedCard = null;
            RepositionCardsInHand();
        }
        else if (deck.Remove(card)) wasDiscarded = true;

        if (!wasDiscarded) return;

        card.cardData.cardEffects.ForEach(ce => ce.OnDiscard(this));
        discard.Add(card);
        SetCardPlace(card, PlayerCardPlace.Discard);
        card.SetCardActive(false);
        card.transform.position = cardDiscardZone.transform.position;
        card.transform.SetParent(cardDiscardZone);
    }

    public void AddPlayerGameEffectsForCard(CardInstance card) {
        card.cardData.playerGameEffectsOnResolve.ForEach(AddPlayerGameEffect);
    }

    public void AddPlayerGameEffect(GameEffect gameEffect) {
        if (gameEffect == null) return;
        GameEffectInstance.CreateAndAdd(gameEffect, playerGameEffects);
    }

    void RepositionCardsInHand() {
        if (hand.Count == 0) return;
        float startX = handCenter.position.x - (hand.Count - 1) * handDeltaDistance / 2;
        for (int i = 0; i < hand.Count; i++) hand[i].transform.DOMove(new(startX + handDeltaDistance * i, handCenter.position.y, handCenter.position.z), 1f).SetEase(Ease.OutCubic);
    }

    void ShuffleDiscardToDeck() {
        deck.AddRange(discard);
        discard.ForEach(card => {
            SetCardPlace(card, PlayerCardPlace.Deck);
            card.transform.position = cardDeckZone.transform.position;
            card.transform.SetParent(cardDeckZone);
        });
        discard.Clear();
        deck.OrderBy(_ => Random.value);
    }

    public void AddCardToHand(CardInstance card) {
        hand.Add(card);
        SetCardPlace(card, PlayerCardPlace.Hand);
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
        if (cardData == null) throw new System.Exception("You can't give null card");
        CardInstance card = Instantiate(cardInstancePrefab);
        card.SetCardData(cardData);
        card.player = this;
        card.baseMass = Random.Range(cardData.cardType.minCardMass, cardData.cardType.maxCardMass);
        if (place == PlayerCardPlace.Hand) AddCardToHand(card);
        else  {
            card.SetCardActive(false);
            if (place == PlayerCardPlace.Deck) {
                SetCardPlace(card, PlayerCardPlace.Deck);
                card.transform.position = cardDeckZone.transform.position;
                card.transform.SetParent(cardDeckZone.transform);
                deck.Add(card);
            }
            else if (place == PlayerCardPlace.Discard) {
                SetCardPlace(card, PlayerCardPlace.Discard);
                card.transform.position = cardDiscardZone.transform.position;
                card.transform.SetParent(cardDiscardZone.transform);
                discard.Add(card);
            }
        }
        return card;
    }

    void SetCardPlace(CardInstance card, PlayerCardPlace place) {
        if (card == null) return;
        card.currentPlace = place;
    }

    public void RemoveCard(CardInstance card) {
        if (card == null || card.player != this || card == cardInPlay) return;

        bool wasRemoved = false;

        if (hand.Remove(card)) {
            wasRemoved = true;
            if (selectedCard == card) selectedCard = null;
            RepositionCardsInHand();
        }
        else if (deck.Remove(card)) wasRemoved = true;
        else if (discard.Remove(card)) wasRemoved = true;

        if (!wasRemoved) return;

        card.cardData.cardEffects.ForEach(ce => ce.OnRemove(this));
        card.transform.DOKill();
        Destroy(card.gameObject);
    }

    public void ResolveMinutePassPlayer() {
        if (cardInPlay != null) cardInPlay.cardData.cardEffects.ForEach(ce => ce.OnActive(this));
        currentOxygenTank += EvaluateOxygenPerMinute();
        if (currentOxygenTank <= 0f && oxygenTanks > 0) {
            oxygenTanks--;
            currentOxygenTank += EvaluateOxygenPerTank();
            DealsManager.I.StartDealSelect(this);
        } // I think, that it is better to withdraw all oxygen, not zero out(cancel out) all requirements just because of changes of gallon...
        float currentMass = EvaluateTotalMass();
        float currentMassToRise = EvaluateMassToRise();
        if (currentMass < currentMassToRise) {
            float speed = (currentMassToRise - currentMass) * coefMassToSpeed;
            distanceToSurface -= speed;
        }
        if (cardInPlay != null)
        {
            cardInPlay.remainInPlay -= 1;
            if (cardInPlay.remainInPlay <= 0)
            {
                GameManager.I.ResolveCard(cardInPlay);
                DiscardCardInPlay();
            }
        }
    }

    public bool IsReadyForMinutePass => playerState == PlayerState.Regular && cardInPlay && (!cardInPlay.cardData.CardRequiresTarget || cardInPlay.targetCard);

    float EvaluateOxygenPerMinute() {
        return EvaluateOxygenProductionPerMinute() - EvaluateOxygenConsumptionPerMinute();
    }

    float EvaluateOxygenConsumptionPerMinute() {
        // Left as KOSMOGOR asked "leave for now"
        List<GameEffectInstance> effects = GameManager.I.GetAllGameEffectsForPlayer(this);
        SetOxygenConsumptionGE setConsumptionEffect = effects.Select(effect => effect.gameEffect).OfType<SetOxygenConsumptionGE>().LastOrDefault();
        if (setConsumptionEffect != null) return Mathf.Max(setConsumptionEffect.oxygenConsumption, 0f);
        // end of left part
        return Mathf.Max(effects.Aggregate(baseOxygenComsumptionMinute, (oxygen, effect) => effect.gameEffect.OnEvaluateOxygenConsumption(oxygen)), 0f);
    }

    float EvaluateOxygenProductionPerMinute() {
        return Mathf.Max(GameManager.I.GetAllGameEffectsForPlayer(this)
            .Aggregate(baseOxygenProductionMinute, (oxygen, effect) => effect.gameEffect.OnEvaluateOxygenProduction(oxygen)), 0f);
    }

    float EvaluateOxygenPerTank() {
        return Mathf.Max(GameManager.I.GetAllGameEffectsForPlayer(this)
            .Aggregate(oxygenPerTank, (oxygen, effect) => effect.gameEffect.OnEvaluateOxygenPerTank(oxygen)), 0f);
    }

    float EvaluateMassToRise() {
        return GameManager.I.GetAllGameEffectsForPlayer(this)
            .Aggregate(massToRise, (mass, effect) => effect.gameEffect.OnEvaluateMassToRise(mass));
    }

    public void ChangeState(PlayerState newPlayerState) {
        playerState = newPlayerState;
    }

    public List<CardInstance> GetCardListInPlace(PlayerCardPlace cardPlace) {
        return cardPlace switch {
            PlayerCardPlace.Hand => hand,
            PlayerCardPlace.Deck => deck,
            PlayerCardPlace.Discard => discard,
            PlayerCardPlace.InPlay => new() { cardInPlay },
            _ => throw new System.NotImplementedException()
        };
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
