using System;

[Serializable]
public abstract class CardEffect
{
    public virtual void OnPlay(Player player) {}
    public virtual void OnResolve(Player player) {}
}
