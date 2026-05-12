using System;
using System.Collections.Generic;

[Serializable]
public abstract class GameEffect
{
    public int duration;
    public bool isPermanent;

    public virtual float OnEvaluateMass(float mass) => mass;
}

public class GameEffectInstance
{
    public GameEffect gameEffect;
    public int remainingDuration;
    List<GameEffectInstance> effectCollection;

    public GameEffectInstance(GameEffect gameEffect) {
        this.gameEffect = gameEffect;
        remainingDuration = gameEffect.duration;
    }

    public static GameEffectInstance CreateAndAdd(GameEffect gameEffect, List<GameEffectInstance> effectCollection) {
        GameEffectInstance effectInstance = new(gameEffect) {
            effectCollection = effectCollection
        };
        effectCollection.Add(effectInstance);
        return effectInstance;
    }

    public void ResolveMinutePass() {
        if (gameEffect.isPermanent) return;
        remainingDuration -= 1;
        if (remainingDuration <= 0) effectCollection.Remove(this);
    }
}
