using System;
using UnityEngine;

[Serializable]
public class AddMassToRiseGE : GameEffect
{
    public float massToRiseDelta;

    public override string DescriptionEffect => $"{(massToRiseDelta >= 0 ? "Повышает" : "Понижает")} силу подъёма на {Mathf.Abs(massToRiseDelta)}";

    public override float OnEvaluateMassToRise(float massToRise) => massToRise + massToRiseDelta;
}
