using System;
using UnityEngine;

[Serializable]
public class AddGameEffectDE : DealEffect
{
    [SerializeReference, SubclassSelector] public GameEffect gameEffect;

    public override string Description => gameEffect.Description;

    public override void ApplyEffect(Player player) {
        GameEffectInstance.CreateAndAdd(gameEffect, player.playerGameEffects);
    }
}
