using System;

[Serializable]
public enum CardEffectTrigger
{
    OnDraw,
    OnPlay,
    OnActive,
    OnResolve,
    OnDiscard,
    OnRemove
}

[Serializable]
public abstract class CardEffect
{
    public virtual bool RequiresTarget => false;
    public virtual int MinimumOtherHandCardsToPlay => RequiresTarget ? 1 : 0;

    public virtual string Description => "Ничего не делает";

    public virtual void OnDraw(Player player) {}
    public virtual void OnDraw(Player player, CardInstance drawnCard) => OnDraw(player);
    public virtual void OnPlay(Player player) {}
    public virtual void OnActive(Player player) {}
    public virtual void OnResolve(Player player) {}
    public virtual void OnDiscard(Player player) {}
    public virtual void OnRemove(Player player) {}
    public virtual void OnMinutePassInCabin(Player player, CardInstance card) {}
    public virtual float OnEvaluateOxygenConsumption(Player player, CardInstance card, float oxygen) => oxygen;
    public virtual float OnEvaluateMassToRise(Player player, CardInstance card, float massToRise) => massToRise;
}

[Serializable]
public abstract class TriggeredCardEffect : CardEffect
{
    public CardEffectTrigger trigger = CardEffectTrigger.OnResolve;

    protected abstract void Apply(Player player);
    protected virtual void Apply(Player player, CardInstance sourceCard) => Apply(player);

    protected string DescribeWithTrigger(string description) => $"{TriggerToText(trigger)}: {description}";

    static string TriggerToText(CardEffectTrigger currentTrigger) => currentTrigger switch
    {
        CardEffectTrigger.OnDraw => "При получении",
        CardEffectTrigger.OnPlay => "При розыгрывании",
        CardEffectTrigger.OnActive => "Во время действия",
        CardEffectTrigger.OnResolve => "При разрешении",
        CardEffectTrigger.OnDiscard => "При сбросе",
        CardEffectTrigger.OnRemove => "При удалении",
        _ => "Эффект"
    };

    void TryApply(Player player, CardEffectTrigger currentTrigger)
    {
        if (trigger == currentTrigger) Apply(player);
    }

    public sealed override void OnDraw(Player player) => TryApply(player, CardEffectTrigger.OnDraw);
    public sealed override void OnDraw(Player player, CardInstance drawnCard) {
        if (trigger == CardEffectTrigger.OnDraw) Apply(player, drawnCard);
    }
    public sealed override void OnPlay(Player player) => TryApply(player, CardEffectTrigger.OnPlay);
    public sealed override void OnActive(Player player) => TryApply(player, CardEffectTrigger.OnActive);
    public sealed override void OnResolve(Player player) => TryApply(player, CardEffectTrigger.OnResolve);
    public sealed override void OnDiscard(Player player) => TryApply(player, CardEffectTrigger.OnDiscard);
    public sealed override void OnRemove(Player player) => TryApply(player, CardEffectTrigger.OnRemove);
}
