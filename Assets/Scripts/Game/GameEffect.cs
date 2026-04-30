using System;
using System.Collections.Generic;

[Serializable]
public class GameEffect
{
    public int duration;
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
        remainingDuration -= 1;
        if (remainingDuration <= 0) effectCollection.Remove(this);
    }
}
