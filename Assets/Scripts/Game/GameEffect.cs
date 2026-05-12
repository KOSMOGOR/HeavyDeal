using System;
using System.Collections.Generic;

[Serializable]
public abstract class GameEffect
{
    public int playDuration, activeDuration;

    public virtual float OnEvaluateMass(float mass) => mass;
    public virtual float OnEvaluateOxygen(float oxygen) => oxygen;
}

public class GameEffectInstance
{
    public GameEffect gameEffect;
    public int remainingPlayDuration, remainingActiveDuration;
    List<GameEffectInstance> effectCollection;

    public GameEffectInstance(GameEffect gameEffect) {
        this.gameEffect = gameEffect;
        remainingPlayDuration = gameEffect.playDuration;
    }

    public static GameEffectInstance CreateAndAdd(GameEffect gameEffect, List<GameEffectInstance> effectCollection) {
        GameEffectInstance effectInstance = new(gameEffect) {
            effectCollection = effectCollection
        };
        effectCollection.Add(effectInstance);
        return effectInstance;
    }

    public void ResolveMinutePass()
    {
        if (remainingPlayDuration <= 0 && remainingPlayDuration <= 0)
        {
            effectCollection.Remove(this);
        }
        if (remainingPlayDuration > 0) {
            remainingPlayDuration -= 1;
        }
        if (remainingActiveDuration > 0) {
            remainingActiveDuration -= 1;
        }
    }
}
