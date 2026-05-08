using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public string cardName = "Cool Card Name";
    public int cardRemainInPlay = 10;
    [SerializeReference, SubclassSelector] public List<CardEffect> cardEffects;
    [SerializeReference, SubclassSelector] public List<GameEffect> gameEffectsOnResolve;
    [SerializeReference, SubclassSelector] public List<GameEffect> playerGameEffectsOnResolve;
}
