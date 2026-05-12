public class SetOxygenConsumptionGE : GameEffect
{
    public float oxygenConsumption;

    public override float OnEvaluateOxygenConsumption(float oxygen) => oxygenConsumption;
}