using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDataList", menuName = "Scriptable Objects/CardDataList")]
public class CardDataList : ScriptableObject
{
    public List<CardData> cardDatas;
}
