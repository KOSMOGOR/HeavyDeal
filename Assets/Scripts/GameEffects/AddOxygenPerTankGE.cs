using System;
using UnityEngine;

[Serializable]
public class AddOxygenPerTankGE : GameEffect
{
    public float oxygenPerTankDelta;

    public override string DescriptionEffect => $"{(oxygenPerTankDelta >= 0 ? "Повышает" : "Понижает")} объём баллона на {Mathf.Abs(oxygenPerTankDelta)}";

    public override float OnEvaluateOxygenPerTank(float oxygen) => oxygen + oxygenPerTankDelta;
}
