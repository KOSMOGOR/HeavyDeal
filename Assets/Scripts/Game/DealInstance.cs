using System.Linq;
using TMPro;
using UnityEngine;

public class DealInstance : MonoBehaviour
{
    public TMP_Text dealText;
    public Deal deal;
    public Player player;

    public void SetDeal(Deal deal, Player player) {
        this.deal = deal;
        dealText.text = string.Join('\n', deal.dealEffects.Select(de => de.Description));
        this.player = player;
    }

    public void SelectThisDeal() {
        DealsManager.I.SelectDeal(this);
    }
}
