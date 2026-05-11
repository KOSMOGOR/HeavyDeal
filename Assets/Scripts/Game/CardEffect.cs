using System;

[Serializable]
public abstract class CardEffect
{
    public virtual bool RequiresTarget => false;

    public virtual void OnDraw(Player player) {}
    public virtual void OnPlay(Player player) {}
    public virtual void OnActive(Player player) {}
    public virtual void OnResolve(Player player) {}
    public virtual void OnDiscard(Player player) {}
    public virtual void OnRemove(Player player) {}
}
