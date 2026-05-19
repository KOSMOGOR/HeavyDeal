using System;

[Serializable]
public abstract class DealEffect
{
    public virtual string Description => "Даёт ничего";

    public abstract void ApplyEffect(Player player);
}