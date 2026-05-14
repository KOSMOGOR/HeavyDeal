using System;

[Serializable]
public class ChangeDistanceToSurfaceCE : TriggeredCardEffect
{
    public float distanceDelta;

    public override string Description => DescribeWithTrigger(distanceDelta >= 0
        ? $"Опускает на {distanceDelta} м"
        : $"Поднимает на {-distanceDelta} м");

    protected override void Apply(Player player)
    {
        if (player == null) return;
        player.distanceToSurface += distanceDelta;
    }
}
