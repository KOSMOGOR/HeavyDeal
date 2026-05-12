using System.Linq;
using TMPro;
using UnityEngine;

public class CardInstance : MonoBehaviour
{
    public CardData cardData;
    public int remainInPlay;
    public float baseMass = 45f; // 30 - 60
    public float baseOxygen = 0f; // [-1f, 1f]
    public Player player;
    public CardInstance targetCard;

    [Header("Renderers")]
    public Transform renderRoot;
    public TMP_Text cardNameText;

    public void SetCardData(CardData newCardData) {
        cardData = newCardData;
        cardNameText.text = cardData.cardName;
        CardReset();
    }

    public void SetCardActive(bool isActive) {
        renderRoot.gameObject.SetActive(isActive);
    }

    public float EvaluateMass() {
        return GameManager.I.GetAllGameEffectsForPlayer(player)
            .Aggregate(baseMass, (mass, effect) => effect.gameEffect.OnEvaluateMass(mass));
    }

    public float EvaluateOxygen()
    {
        return GameManager.I.GetAllGameEffectsForPlayer(player)
            .Aggregate(baseOxygen, (oxygen, effect) => effect.gameEffect.OnEvaluateOxygen(oxygen));
    }

    public void SelectThisCard() {
        player.TrySelectCard(this);
    }

    public void CardReset() {
        remainInPlay = cardData.cardRemainInPlay;
        targetCard = null;
    }
    enum CardState
    {
        InHand,
        Playing,
        Active,
        Discarded
    }
}
