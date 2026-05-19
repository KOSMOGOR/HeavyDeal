using System;
using UnityEngine;

[Serializable]
public class AddDealsCountGE : GameEffect
{
    public int dealsCountDelta;

    public override string DescriptionEffect => $"Некто предлагает на {Mathf.Abs(dealsCountDelta)} {(dealsCountDelta >= 0 ? "больше" : "меньше")} сделок";

    public override int OnEvaluateDealsCount(int deals) => deals + dealsCountDelta;
}
