using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deal", menuName = "Scriptable Objects/Deal")]
public class Deal : ScriptableObject
{
    [SerializeReference, SubclassSelector] public List<DealEffect> dealEffects;
}
