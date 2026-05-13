using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public string cardName = "Cool Card Name";
    public CardType cardType;
    public bool canBePlayed = true;
    public int cardRemainInPlay = 10;
    [SerializeReference, SubclassSelector] public List<CardEffect> cardEffects;
    [SerializeReference, SubclassSelector] public List<GameEffect> gameEffectsOnResolve;
    [SerializeReference, SubclassSelector] public List<GameEffect> playerGameEffectsOnResolve;

    string DescriptionCE => cardEffects.Count > 0 ? "При разыгрывании: " + string.Join('\n', cardEffects.Select(ce => ce.Description)) : null;
    string DescriptionGE => gameEffectsOnResolve.Count > 0 ? "При разрешении для всех: " + string.Join('\n', gameEffectsOnResolve.Select(ge => ge.Description)) : null;
    string DescriptionPGE => playerGameEffectsOnResolve.Count > 0 ? "При разрешении: " + string.Join('\n', playerGameEffectsOnResolve.Select(ge => ge.Description)) : null;
    public string Description => string.Join('\n', new[] { DescriptionCE, DescriptionGE, DescriptionPGE }.Select(s => !string.IsNullOrEmpty(s)));

    public bool CardRequiresTarget => cardEffects.Any(ce => ce.RequiresTarget);
}
