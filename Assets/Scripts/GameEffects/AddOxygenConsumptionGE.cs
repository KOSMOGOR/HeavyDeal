using System;

[Serializable]
public class AddOxygenConsumptionGE : GameEffect
{
    public float oxygenConsumptionDelta;

    public override string DescriptionEffect => $"{(oxygenConsumptionDelta >= 0 ? "Повышает" : "Понижает")} потребление кислорода на {oxygenConsumptionDelta}";

    public override float OnEvaluateOxygenConsumption(float oxygen) => oxygen + oxygenConsumptionDelta;
}