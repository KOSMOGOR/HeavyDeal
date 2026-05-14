using System;

[Serializable]
public class ChangeDistanceToSurfaceCE : CardEffect
{
    public float distanceDelta;

    public override string Description => distanceDelta >= 0
        ? $"Опускает на {distanceDelta} м"
        : $"Поднимает на {-distanceDelta} м";

    public override void OnResolve(Player player)
    {
        if (player == null) return;
        player.distanceToSurface += distanceDelta;
    }
}
