using System;
using UnityEngine;

[Serializable]
public class ChangeCardMassInPlaceDE : DealEffect
{
    public float massCoefToAdd;
    public PlayerCardPlace placeToChange;

    public override string Description => $"{(massCoefToAdd >= 0 ? "Повышает" : "Понижает")} вес карт в {CardPlace(placeToChange)} на {(int)(massCoefToAdd * 100)}%";

    public override void ApplyEffect(Player player) {
        player.GetCardListInPlace(placeToChange).ForEach(card => card.baseMass *= 1 + massCoefToAdd);
    }

    string CardPlace(PlayerCardPlace cardPlace) {
        return cardPlace switch {
            PlayerCardPlace.Hand => "руке",
            PlayerCardPlace.Deck => "колоде",
            PlayerCardPlace.Discard => "сбросе",
            PlayerCardPlace.InPlay => "игре",
            _ => "-"
        };
    }
}
