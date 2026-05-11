using UnityEngine;

[CreateAssetMenu(fileName = "CardType", menuName = "Scriptable Objects/CardType")]
public class CardType : ScriptableObject
{
    public string cardTypeName = "Cool Card Type";
    public float minCardMass = 30f, maxCardMass = 60f;
}
