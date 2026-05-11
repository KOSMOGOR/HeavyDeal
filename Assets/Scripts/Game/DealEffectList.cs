using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DealEffectList", menuName = "Scriptable Objects/DealEffectList")]
public class DealEffectList : ScriptableObject
{
    [SerializeReference, SubclassSelector] public List<DealEffect> dealEffects;
}
