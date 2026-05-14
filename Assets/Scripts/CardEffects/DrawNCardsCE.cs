using System;

[Serializable]
public class DrawNCardsCE : TriggeredCardEffect
{
    public int cardsToDraw = 1;

    public override string Description => DescribeWithTrigger($"Вытяни {cardsToDraw} {(cardsToDraw == 1 ? "карту" : cardsToDraw <= 4 ? "карты" : "карт")}");

    protected override void Apply(Player player) {
        for (int i = 0; i < cardsToDraw; i++) player.DrawCard();
    }
}
