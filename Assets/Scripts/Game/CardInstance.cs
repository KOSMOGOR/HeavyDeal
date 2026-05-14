using System.Linq;
using TMPro;
using UnityEngine;

public class CardInstance : MonoBehaviour
{
    public CardData cardData;
    public int remainInPlay;
    public float baseMass = 45f;
    public Player player;
    public PlayerCardPlace currentPlace;
    public CardInstance targetCard;

    [Header("Renderers")]
    public Transform renderRoot;
    public TMP_Text cardNameText;
    public TMP_Text cardRemainInPlay;
    public TMP_Text cardDescription;
    public TMP_Text cardMassText;

    void Update() {
        cardMassText.text = baseMass.ToString("F1");
    }

    public void SetCardData(CardData newCardData) {
        cardData = newCardData;
        cardNameText.text = cardData.cardName;
        cardRemainInPlay.text = cardData.cardRemainInPlay.ToString();
        cardDescription.text = cardData.Description;
        baseMass = Random.Range(cardData.cardType.minCardMass, cardData.cardType.maxCardMass);
        CardReset();
    }

    public void SetCardActive(bool isActive) {
        renderRoot.gameObject.SetActive(isActive);
    }

    public float EvaluateMass() {
        return GameManager.I.GetAllGameEffectsForPlayer(player)
            .Aggregate(baseMass, (mass, effect) => effect.gameEffect.OnEvaluateMass(this, mass));
    }

    public void SelectThisCard() {
        player.TrySelectCard(this);
    }

    public void CardReset() {
        remainInPlay = cardData.cardRemainInPlay;
        targetCard = null;
    }
}
