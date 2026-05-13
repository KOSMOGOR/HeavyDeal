using System;

[Serializable]
public class SetOxygenConsumptionGE : GameEffect
{
    public float oxygenConsumption;

    public override string DescriptionEffect => $"Устанавливает потребление кислорода на {oxygenConsumption}";

    public override float OnEvaluateOxygenConsumption(float oxygen) => oxygenConsumption;
}