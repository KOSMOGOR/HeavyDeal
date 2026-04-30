using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public float secondsToGameMinute = 1;
    float secondsPassed;

    List<GameEffectInstance> gameEffects;
    List<Player> players;

    void Update() {
        if (players.Count > 0 && players.All(p => p.cardInPlay != null)) {
            secondsPassed += Time.deltaTime;
            if (secondsPassed >= secondsToGameMinute) {
                secondsPassed -= secondsToGameMinute;
                ResolveMinutePass();
            }
        }
    }

    public void RegisterPlayer(Player player) {
        players.Add(player);
    }

    void ResolveMinutePass() {
        // game effects
        foreach (GameEffectInstance gameEffect in gameEffects) {
            gameEffect.ResolveMinutePass();
        }
        // player game effects
        foreach (Player player in players) {
            foreach (GameEffectInstance playerGameEffect in player.playerGameEffects) {
                playerGameEffect.ResolveMinutePass();
            }
        }
        // player cards in play
        foreach (Player player in players) {
            player.cardInPlay.remainInPlay -= 1;
            if (player.cardInPlay.remainInPlay <= 0) {
                ResolveCard(player, player.cardInPlay);
                player.DiscardCardInPlay();
            }
        }
    }

    public void PlayCard(Player player, CardInstance card) {
        if (!players.Contains(player)) return;
        card.cardData.cardEffects.ForEach(ce => ce.OnPlay(player));
    }

    void ResolveCard(Player player, CardInstance card) {
        if (!players.Contains(player)) return;
        card.cardData.cardEffects.ForEach(ce => ce.OnResolve(player));
        card.cardData.gameEffectsOnResolve.ForEach(ge => GameEffectInstance.CreateAndAdd(ge, gameEffects));
        player.AddPlayerGameEffects(card);
    }
}
