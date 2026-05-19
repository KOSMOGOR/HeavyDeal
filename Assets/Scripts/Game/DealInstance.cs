using TMPro;
using UnityEngine;

public class DealInstance : MonoBehaviour
{
    public TMP_Text dealText;
    public DealEffect positiveEffect;
    public DealEffect negativeEffect;
    public Player player;

    public void SetDeal(DealEffect positiveEffect, DealEffect negativeEffect, Player player) {
        this.positiveEffect = positiveEffect;
        this.negativeEffect = negativeEffect;
        dealText.text = positiveEffect.Description + "\n\n" + negativeEffect.Description;
        this.player = player;
    }

    public void SelectThisDeal() {
        DealsManager.I.SelectDeal(this);
    }

    public void ApplyDealEffects(Player player) {
        positiveEffect.ApplyEffect(player);
        negativeEffect.ApplyEffect(player);
    }
}
