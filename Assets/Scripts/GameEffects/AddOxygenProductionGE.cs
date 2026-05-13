using System;
using UnityEngine;

[Serializable]
public class AddOxygenProductionGE : GameEffect
{
    public float oxygenProductionDelta;

    public override string DescriptionEffect => $"{(oxygenProductionDelta >= 0 ? "Повышает" : "Понижает")} генерацию кислорода на {Mathf.Abs(oxygenProductionDelta)}";

    public override float OnEvaluateOxygenProduction(float oxygen) => oxygen + oxygenProductionDelta;
}