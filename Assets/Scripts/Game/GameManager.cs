using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public float secondsToGameMinute = 1f;
    public float delayAfterPlay = 1f;
    public TMP_Text secondsPassedText;
    public CardData deadCard;

    float secondsPassed;

    readonly List<GameEffectInstance> gameEffects = new();
    readonly List<Player> players = new();
    List<CardData> regularCards;
    public List<CardData> RegularCards => regularCards;

    protected override void AwakeNew() {
        regularCards = Resources.LoadAll<CardData>("CardDatas").ToList();
        Resources.Load<CardDataList>("SpecialCards").cardDatas.ForEach(cd => regularCards.Remove(cd));
    }

    void Update() {
        if (players.Count > 0 && players.All(p => p.IsReadyForMinutePass)) {
            secondsPassed += Time.deltaTime;
            if (secondsPassed >= 0 && players.Any(p => p.cardInPlay.cardData.cardRemainInPlay == 0)) {
                players.ForEach(p => {
                    if (p.cardInPlay.cardData.cardRemainInPlay == 0) {
                        ResolveCard(p.cardInPlay);
                        p.DiscardCardInPlay();
                    }
                });
            } else if (secondsPassed >= secondsToGameMinute) {
                secondsPassed = 0;
                ResolveMinutePass();
            }
        } else secondsPassed = -delayAfterPlay;
        if (secondsPassedText) secondsPassedText.text = secondsPassed.ToString("F2");
    }

    public void RegisterPlayer(Player player) {
        players.Add(player);
    }

    void ResolveMinutePass() {
        // player cards in play
        foreach (Player player in players) {
            player.ResolveMinutePassPlayer();
        }
        // game and player effects
        GetAllGameEffectsForAllPlayers().ForEach(effect => effect.ResolveMinutePass());
        // order is better for already applied effects, but still spends 1 minute on newly applied ones, change back, if this works worse
    }

    public void PlayCard(Player player, CardInstance card) {
        if (!players.Contains(player)) return;
        card.cardData.cardEffects.ForEach(ce => ce.OnPlay(player));
    }

    public void ResolveCard(CardInstance card) {
        if (!players.Contains(card.player)) return;
        card.cardData.cardEffects.ForEach(ce => ce.OnResolve(card.player));
        card.cardData.gameEffectsOnResolve.ForEach(ge => GameEffectInstance.CreateAndAdd(ge, gameEffects));
        card.player.AddPlayerGameEffectsForCard(card);
    }

    public List<GameEffectInstance> GetAllGameEffectsForPlayer(Player player) {
        List<GameEffectInstance> effects = new();
        effects.AddRange(gameEffects);
        effects.AddRange(player.playerGameEffects);
        return effects;
    }

    public List<GameEffectInstance> GetAllGameEffectsForAllPlayers() {
        List<GameEffectInstance> effects = new();
        effects.AddRange(gameEffects);
        players.ForEach(player => effects.AddRange(player.playerGameEffects));
        return effects;
    }

    public CardData GetRandomCardOfType(CardType cardType) {
        return RegularCards.Where(card => card.cardType == cardType).GetRandomOrDefault();
    }
}
