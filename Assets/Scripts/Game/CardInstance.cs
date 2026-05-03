using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CardInstance : MonoBehaviour
{
    public CardData cardData;
    public int remainInPlay;

    SpriteRenderer sprite;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetCardActive(bool isActive) {
        sprite.enabled = isActive;
    }
}
