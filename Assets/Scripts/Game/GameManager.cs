using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public float secondsToGameMinute = 1f;
    public float delayAfterPlay = 1f;
    public TMP_Text secondsPassedText;

    float secondsPassed;

    readonly List<GameEffectInstance> gameEffects = new();
    readonly List<Player> players = new();

    void Update() {
        if (players.Count > 0 && players.All(p => p.cardInPlay && (!p.cardInPlay.cardData.CardRequiresTarget || p.cardInPlay.targetCard))) {
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
        // game and player effects
        GetAllGameEffectsForAllPlayers().ForEach(effect => effect.ResolveMinutePass());
        // player cards in play
        foreach (Player player in players) {
            player.cardInPlay.remainInPlay -= 1;
            if (player.cardInPlay.remainInPlay <= 0) {
                ResolveCard(player.cardInPlay);
                player.DiscardCardInPlay();
            }
        }
    }

    public void PlayCard(Player player, CardInstance card) {
        if (!players.Contains(player)) return;
        card.cardData.cardEffects.ForEach(ce => ce.OnPlay(player));
    }

    void ResolveCard(CardInstance card) {
        if (!players.Contains(card.player)) return;
        card.cardData.cardEffects.ForEach(ce => ce.OnResolve(card.player));
        card.cardData.gameEffectsOnResolve.ForEach(ge => GameEffectInstance.CreateAndAdd(ge, gameEffects));
        card.player.AddPlayerGameEffects(card);
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
}
