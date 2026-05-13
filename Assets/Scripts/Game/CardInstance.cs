using System.Linq;
using TMPro;
using UnityEngine;

public class CardInstance : MonoBehaviour
{
    public CardData cardData;
    public int remainInPlay;
    public float baseMass = 45f;
    public Player player;
    public CardInstance targetCard;

    [Header("Renderers")]
    public Transform renderRoot;
    public TMP_Text cardNameText;
    public TMP_Text cardRemainInPlay;
    public TMP_Text cardDescription;

    public void SetCardData(CardData newCardData) {
        cardData = newCardData;
        cardNameText.text = cardData.cardName;
        cardRemainInPlay.text = cardData.cardRemainInPlay.ToString();
        cardDescription.text = cardData.Description;
        CardReset();
    }

    public void SetCardActive(bool isActive) {
        renderRoot.gameObject.SetActive(isActive);
    }

    public float EvaluateMass() {
        return GameManager.I.GetAllGameEffectsForPlayer(player)
            .Aggregate(baseMass, (mass, effect) => effect.gameEffect.OnEvaluateMass(mass));
    }

    public void SelectThisCard() {
        player.TrySelectCard(this);
    }

    public void CardReset() {
        remainInPlay = cardData.cardRemainInPlay;
        targetCard = null;
    }
}
