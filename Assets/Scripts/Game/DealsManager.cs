using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DealsManager : SingletonMonoBehaviour<DealsManager>
{
    public int dealsCount = 3;
    public Transform dealsCenter;
    public float dealsDeltaOffset = 1f;
    public DealInstance dealInstancePrefab;

    List<Deal> allDeals;
    readonly List<DealInstance> currentDeals = new();

    protected override void AwakeNew() {
        allDeals = Resources.LoadAll<Deal>("Deals").ToList();
    }

    List<Deal> GetDeals() {
        List<Deal> deals = new(allDeals);
        return deals.OrderBy(_ => Random.value).Take(dealsCount).ToList();
    }

    public void StartDealSelect(Player player) {
        player.ChangeState(PlayerState.Deal);
        List<Deal> deals = GetDeals();
        float startX = dealsCenter.position.x - (deals.Count - 1) * dealsDeltaOffset / 2;
        currentDeals.Clear();
        for (int i = 0; i < deals.Count; i++) {
            Deal deal = deals[i];
            DealInstance dealInstance = Instantiate(dealInstancePrefab);
            dealInstance.SetDeal(deal, player);
            dealInstance.transform.position = new(startX + i * dealsDeltaOffset, dealsCenter.position.y, dealsCenter.position.z);
            dealInstance.transform.SetParent(dealsCenter);
            currentDeals.Add(dealInstance);
        }
    }

    public void SelectDeal(DealInstance dealInstance) {
        if (!currentDeals.Contains(dealInstance)) return;
        Player player = dealInstance.player;
        dealInstance.deal.dealEffects.ForEach(de => de.ApplyEffect(player));
        currentDeals.ForEach(di => Destroy(di));
        currentDeals.Clear();
        player.ChangeState(PlayerState.Regular);
    }
}
