using System;
using UnityEngine;

[Serializable]
public class AddPlayerGameEffectOnRemoveCE : CardEffect
{
    [SerializeReference, SubclassSelector] public GameEffect gameEffect;

    public override string Description => gameEffect == null ? "При удалении: ничего не делает" : $"При удалении: {gameEffect.Description}";

    public override void OnRemove(Player player) {
        if (player == null || gameEffect == null) return;
        player.AddPlayerGameEffect(gameEffect);
    }
}
