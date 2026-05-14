using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [Header("Player Data")]
    public float currentOxygenTank = 0f;
    public int oxygenTanks = 2;
    public float oxygenPerTank = 60f;
    public float baseOxygenComsumptionMinute = 1f;
    public float baseOxygenProductionMinute = 0f;
    public float baseDistanceToSurface = 500f, distanceToSurface;
    public float massToRise = 200f;
    public float coefMassToSpeed = 0.5f;
    public int baseWaitingTime = 10;
    public int playerWaitingTime = 0;
    public int basePlayerDrawCardsWaiting = 5;
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
    public bool usePrebuiltStartDeck;
    public List<CardData> startDeck;
    [Serializable] public struct CardTypeCount { public CardType cardType; public int count; }
    public List<CardTypeCount> startCardTypesCount;
    public float startMass = 300f;
    public float maxCardMassDelta = 5f;
    public int startPlayerHandSize = 5;

    [Header("Renderers")]
    public TMP_Text endGameText;
    public TMP_Text cardInPlayText;
    public TMP_Text distanceToSurfaceText;
    public TMP_Text oxygenText;

    readonly List<CardInstance> hand = new(); // 0 - top of the deck
    public List<CardInstance> Hand => hand;
    readonly List<CardInstance> deck = new(); // 0 - left card
    readonly List<CardInstance> discard = new(); // 0 - bottom of discard

    void Start() {
        GameManager.I.RegisterPlayer(this);
        currentOxygenTank = oxygenPerTank;
        distanceToSurface = baseDistanceToSurface;
        CardData cardData = Resources.Load<CardData>("CardDatas/CardThink");
        #if !UNITY_EDITOR
            usePrebuiltStartDeck = false;
        #endif
        if (usePrebuiltStartDeck) {
            for (int i = 0; i < 5; i++) GiveNewCardToPlayer((startDeck.Count() > i && startDeck[i] != null) ? startDeck[i] : cardData);
        } else {
            foreach (CardTypeCount ctc in startCardTypesCount) {
                for (int i = 0; i < ctc.count; i++) GiveNewCardToPlayer(GameManager.I.GetRandomCardOfType(ctc.cardType));
            }
            float delta = EvaluateListMass(deck) - startMass;
            bool bigger = delta > 0;
            List<CardInstance> cards = deck.Where(card => bigger ? card.baseMass > card.cardData.cardType.minCardMass : card.baseMass < card.cardData.cardType.maxCardMass).ToList();
            if (cards.Count == 0 && delta != 0) throw new Exception($"Start hand can't have mass {startMass}, since it already has mass {delta + startMass}");
            while (delta != 0) {
                CardInstance cardToChange = cards.GetRandom();
                CardType cardType = cardToChange.cardData.cardType;
                float cardMassDeltaAbs = Mathf.Min(bigger ? cardToChange.baseMass - cardType.minCardMass : cardType.maxCardMass - cardToChange.baseMass, maxCardMassDelta, Math.Abs(delta));
                cardToChange.baseMass += bigger ? -cardMassDeltaAbs : cardMassDeltaAbs;
                if (cardToChange.baseMass == (bigger ? cardType.minCardMass : cardType.maxCardMass)) cards.Remove(cardToChange);
                delta = EvaluateListMass(deck) - startMass;
                bigger = delta > 0;
            }
        }
        for (int i = 0; i < startPlayerHandSize; i++) DrawCard();
        endGameText.enabled = false;
    }

    void Update() {
        UpdateRenderers();
    }

    void UpdateRenderers() {
        cardInPlayText.text = cardInPlay != null ? cardInPlay.remainInPlay.ToString() :
            playerWaitingTime > 0 ? playerWaitingTime.ToString() : "-";
        distanceToSurfaceText.text = $"Дистанция до поверхности: {distanceToSurface:F1}";
        oxygenText.text = $"Осталось кислорода: {currentOxygenTank:F1}\nОсталось баллонов: {oxygenTanks}";
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
        PlayCardFromHand(selectedCard);
    }

    public bool PlayCardFromHand(CardInstance card) {
        if (playerState != PlayerState.Regular) return false;
        if (card != null && hand.Contains(card) && card.cardData.canBePlayed && cardInPlay == null) {
            int remainingHandCardsAfterPlay = hand.Count - 1;
            int minimumOtherHandCardsRequired = card.cardData.cardEffects.Count > 0
                ? card.cardData.cardEffects.Max(effect => effect.MinimumOtherHandCardsToPlay)
                : 0;
            if (remainingHandCardsAfterPlay < minimumOtherHandCardsRequired) return false;

            if (selectedCard != null && selectedCard != card) selectedCard.transform.DOScale(1f, 0.5f);
            hand.Remove(card);
            cardInPlay = card;
            SetCardPlace(cardInPlay, PlayerCardPlace.InPlay);
            cardInPlay.transform.DOScale(1f, 0.25f);
            cardInPlay.transform.DOMove(cardPlayZone.position, 0.5f).SetEase(Ease.OutCubic);
            cardInPlay.transform.SetParent(cardPlayZone);
            selectedCard = null;
            GameManager.I.PlayCard(this, cardInPlay);
            CheckDeadCardsLoseCondition();
            return true;
        }
        return false;
    }

    public void DiscardCardInPlay() {
        if (cardInPlay != null) DiscardCard(cardInPlay);
    }

    public void DiscardCard(CardInstance card) {
        if (card == null || card.player != this) return;
        if (IsDeadCard(card)) return;

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
        CheckDeadCardsLoseCondition();
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
        card.cardData.cardEffects.ForEach(ce => ce.OnDraw(this, card));
        CheckDeadCardsLoseCondition();
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
        if (place == PlayerCardPlace.InPlay) throw new Exception("Use PlaySelectedCard() for this");
        if (cardData == null) throw new Exception("You can't give null card");
        CardInstance card = Instantiate(cardInstancePrefab);
        card.SetCardData(cardData);
        card.player = this;
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

    bool IsDeadCard(CardInstance card) => card != null && GameManager.I != null && card.cardData == GameManager.I.deadCard;

    void CheckDeadCardsLoseCondition() {
        if (DeadCardLoseCondition()) {
            GetCardsInCabin().ToList().ForEach(card => card.cardData.cardEffects.ForEach(ce => ce.OnDeath(this, card)));
            if (DeadCardLoseCondition()) ChangeState(PlayerState.Lose);
        }
    }

    bool DeadCardLoseCondition() {
        return hand.Count > 0 && hand.All(IsDeadCard);
    }

    bool RemoveCardFromPlaces(CardInstance card) {
        if (card == null || card.player != this || card == cardInPlay) return false;

        if (hand.Remove(card)) {
            if (selectedCard == card) selectedCard = null;
            RepositionCardsInHand();
            return true;
        }
        if (deck.Remove(card)) return true;
        if (discard.Remove(card)) return true;
        return false;
    }

    public void RemoveCard(CardInstance card) {
        if (!RemoveCardFromPlaces(card)) return;

        card.cardData.cardEffects.ForEach(ce => ce.OnRemove(this));
        CheckDeadCardsLoseCondition();
        card.transform.DOKill();
        Destroy(card.gameObject);
    }

    public void RemoveCards(IEnumerable<CardInstance> cards) {
        if (cards == null) return;

        CardInstance[] cardsToRemove = cards.Where(card => card != null).Distinct().ToArray();
        if (cardsToRemove.Length == 0) return;

        foreach (CardInstance card in cardsToRemove) {
            if (!RemoveCardFromPlaces(card)) continue;
            card.cardData.cardEffects.ForEach(ce => ce.OnRemove(this));
            card.transform.DOKill();
            Destroy(card.gameObject);
        }

        CheckDeadCardsLoseCondition();
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
        foreach (CardInstance card in GetCardsInCabin().ToArray()) {
            if (card == null) continue;
            card.cardData.cardEffects.ForEach(effect => effect.OnMinutePassInCabin(this, card));
        }
        if (playerState == PlayerState.Waiting) {
            if (playerWaitingTime > 0) playerWaitingTime--;
            if (playerWaitingTime <= 0) {
                ResolveWaitingEffect();
                ChangeState(PlayerState.Regular);
            }
        } else if (cardInPlay != null) {
            cardInPlay.remainInPlay -= 1;
            if (cardInPlay.remainInPlay <= 0)
            {
                GameManager.I.ResolveCard(cardInPlay);
                DiscardCardInPlay();
            }
        }
        if (distanceToSurface <= 0) {
            ChangeState(PlayerState.Win);
        } else {
            CheckDeadCardsLoseCondition();
        }
    }

    void ResolveWaitingEffect() {
        foreach (CardInstance card in hand) {
            bool shouldDiscard = GameManager.I.GetAllGameEffectsForPlayer(this).Aggregate(true, (discard, effect) => effect.gameEffect.OnEvaluateWaitingDiscard(discard, card));
            if (shouldDiscard) DiscardCard(card);
        }
        for (int i = 0; i < basePlayerDrawCardsWaiting; i++) DrawCard();
    }

    public void StartWaiting() {
        if (playerState == PlayerState.Regular && cardInPlay == null) ChangeState(PlayerState.Waiting);
    }

    public bool IsReadyForMinutePass =>
        playerState == PlayerState.Regular && cardInPlay && (!cardInPlay.cardData.CardRequiresTarget || cardInPlay.targetCard) ||
        playerState == PlayerState.Waiting;

    float EvaluateOxygenPerMinute() {
        return EvaluateOxygenProductionPerMinute() - EvaluateOxygenConsumptionPerMinute();
    }

    float EvaluateOxygenConsumptionPerMinute() {
        // Left as KOSMOGOR asked "leave for now"
        List<GameEffectInstance> effects = GameManager.I.GetAllGameEffectsForPlayer(this);
        SetOxygenConsumptionGE setConsumptionEffect = effects.Select(effect => effect.gameEffect).OfType<SetOxygenConsumptionGE>().LastOrDefault();
        float currentConsumption = setConsumptionEffect != null
            ? Mathf.Max(setConsumptionEffect.oxygenConsumption, 0f)
            : Mathf.Max(effects.Aggregate(baseOxygenComsumptionMinute, (oxygen, effect) => effect.gameEffect.OnEvaluateOxygenConsumption(oxygen)), 0f);
        // end of left part
        foreach (CardInstance card in GetCardsInCabin()) {
            currentConsumption = card.cardData.cardEffects
                .Aggregate(currentConsumption, (oxygen, effect) => effect.OnEvaluateOxygenConsumption(this, card, oxygen));
        }
        return Mathf.Max(currentConsumption, 0f);
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
        float currentMassToRise = GameManager.I.GetAllGameEffectsForPlayer(this)
            .Aggregate(massToRise, (mass, effect) => effect.gameEffect.OnEvaluateMassToRise(mass));
        foreach (CardInstance card in GetCardsInCabin()) {
            currentMassToRise = card.cardData.cardEffects
                .Aggregate(currentMassToRise, (mass, effect) => effect.OnEvaluateMassToRise(this, card, mass));
        }
        return currentMassToRise;
    }

    public void ChangeState(PlayerState newPlayerState) {
        playerState = newPlayerState;
        if (playerState == PlayerState.Win || playerState == PlayerState.Lose) {
            endGameText.enabled = true;
            endGameText.text = playerState == PlayerState.Win ? "Победа" : "Поражение";
        } else if (playerState == PlayerState.Waiting) {
            playerWaitingTime = baseWaitingTime;
        }
    }

    public List<CardInstance> GetCardListInPlace(PlayerCardPlace cardPlace) {
        return cardPlace switch {
            PlayerCardPlace.Hand => hand,
            PlayerCardPlace.Deck => deck,
            PlayerCardPlace.Discard => discard,
            PlayerCardPlace.InPlay => new() { cardInPlay },
            _ => throw new NotImplementedException()
        };
    }

    IEnumerable<CardInstance> GetCardsInCabin() {
        foreach (CardInstance card in hand) yield return card;
        foreach (CardInstance card in deck) yield return card;
        foreach (CardInstance card in discard) yield return card;
        if (cardInPlay != null) yield return cardInPlay;
    }
}

public enum PlayerState {
    Regular,
    Deal,
    Win,
    Lose,
    Waiting
}

public enum PlayerCardPlace {
    Hand,
    Deck,
    Discard,
    InPlay
}
