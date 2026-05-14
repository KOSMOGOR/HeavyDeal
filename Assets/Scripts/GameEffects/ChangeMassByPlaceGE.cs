using System;
using UnityEngine;

[Serializable]
public class ChangeMassByPlaceGE : GameEffect
{
    public PlayerCardPlace targetPlace;
    public float massCoefToAdd;

    public override string DescriptionEffect => $"{(massCoefToAdd >= 0 ? "Повышает" : "Понижает")} вес карт в {CardPlace(targetPlace)} на {(int)(Mathf.Abs(massCoefToAdd) * 100)}%";

    public override float OnEvaluateMass(CardInstance card, float mass)
    {
        if (card == null || card.currentPlace != targetPlace) return mass;
        return mass * (1 + massCoefToAdd);
    }

    static string CardPlace(PlayerCardPlace cardPlace)
    {
        return cardPlace switch {
            PlayerCardPlace.Hand => "руке",
            PlayerCardPlace.Deck => "колоде",
            PlayerCardPlace.Discard => "сбросе",
            PlayerCardPlace.InPlay => "игре",
            _ => "-"
        };
    }
}
