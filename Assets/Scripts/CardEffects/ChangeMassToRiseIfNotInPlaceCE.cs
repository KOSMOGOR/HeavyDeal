using System;

[Serializable]
public class ChangeMassToRiseIfNotInPlaceCE : CardEffect
{
    public PlayerCardPlace ignoredPlace = PlayerCardPlace.Hand;
    public float massToRiseDelta;

    public override string Description => $"Пока карта не в {PlaceToText(ignoredPlace)}: сила подъёма {(massToRiseDelta >= 0 ? "повышается" : "снижается")} на {Math.Abs(massToRiseDelta)}";

    public override float OnEvaluateMassToRise(Player player, CardInstance card, float massToRise)
    {
        if (card == null || card.currentPlace == ignoredPlace) return massToRise;
        return massToRise + massToRiseDelta;
    }

    static string PlaceToText(PlayerCardPlace place) => place switch
    {
        PlayerCardPlace.Hand => "руке",
        PlayerCardPlace.Deck => "колоде",
        PlayerCardPlace.Discard => "сбросе",
        PlayerCardPlace.InPlay => "розыгрыше",
        _ => "зоне"
    };
}
