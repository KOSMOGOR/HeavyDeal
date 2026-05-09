using System;

[Serializable]
public abstract class CardEffect
{
    public virtual bool RequiresTarget => false;

    public virtual void OnPlay(Player player) {}
    public virtual void OnResolve(Player player) {}
}
