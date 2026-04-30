using System;

[Serializable]
public class CardEffect
{
    public virtual void OnPlay(Player player) {}
    public virtual void OnResolve(Player player) {}
}
