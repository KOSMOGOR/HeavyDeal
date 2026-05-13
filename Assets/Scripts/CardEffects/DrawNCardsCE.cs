using System;

[Serializable]
public class DrawNCardsCE : CardEffect
{
    public int cardsToDraw = 1;

    public override string Description => $"Вытяни {cardsToDraw} {(cardsToDraw == 1 ? "карту" : cardsToDraw <= 4 ? "карты" : "карт")}";

    public override void OnResolve(Player player) {
        for (int i = 0; i < cardsToDraw; i++) player.DrawCard();
    }
}