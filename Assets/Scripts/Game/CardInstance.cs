using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CardInstance : MonoBehaviour
{
    public CardData cardData;
    public int remainInPlay;
    public float baseMass = 45f; // 30 - 60
    public Player player;

    SpriteRenderer sprite;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetCardActive(bool isActive) {
        sprite.enabled = isActive;
    }

    public float EvaluateMass() {
        return baseMass;
    }
}
