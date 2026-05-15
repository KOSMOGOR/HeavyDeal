using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public Image cardImage;

    void Update() {
        cardMassText.text = baseMass.ToString("F1");
    }

    public void SetCardData(CardData newCardData) {
        cardData = newCardData;
        cardNameText.text = cardData.cardName;
        cardRemainInPlay.text = cardData.cardRemainInPlay.ToString();
        cardDescription.text = cardData.Description;
        cardImage.sprite = cardData.cardSpriteOverride != null ? cardData.cardSpriteOverride : cardData.cardType.cardSprite;
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
        transform.DOScale(1f, 0.5f);
    }
}
