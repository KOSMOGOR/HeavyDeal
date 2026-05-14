using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DealsManager : SingletonMonoBehaviour<DealsManager>
{
    public int baseDealsCount = 3;
    public Transform dealsCenter;
    public float dealsDeltaOffset = 1f;
    public DealInstance dealInstancePrefab;

    DealEffectList positiveDealEffects;
    DealEffectList negativeDealEffects;
    public List<DealInstance> currentDeals = new();

    protected override void AwakeNew() {
        positiveDealEffects = Resources.Load<DealEffectList>("Deals/PositiveDealEffectList");
        negativeDealEffects = Resources.Load<DealEffectList>("Deals/NegativeDealEffectList");
    }

    List<(DealEffect, DealEffect)> GetDeals(Player player) {
        List<(DealEffect positiveDealEffect, DealEffect negativeDealEffect)> deals = new();
        for (int i = 0; i < EvaluateDealsCount(player); i++)
            deals.Add((positiveDealEffects.dealEffects.OrderBy(_ => Random.value).First(), negativeDealEffects.dealEffects.OrderBy(_ => Random.value).First()));
        return deals;
    }

    int EvaluateDealsCount(Player player) {
        return GameManager.I.GetAllGameEffectsForPlayer(player)
            .Aggregate(baseDealsCount, (dealsCount, effect) => effect.gameEffect.OnEvaluateDealsCount(dealsCount));
    }

    public void StartDealSelect(Player player) {
        player.ChangeState(PlayerState.Deal);
        List<(DealEffect positiveDealEffect, DealEffect negativeDealEffect)> deals = GetDeals(player);
        float startX = dealsCenter.position.x - (deals.Count - 1) * dealsDeltaOffset / 2;
        currentDeals.Clear();
        for (int i = 0; i < deals.Count; i++) {
            (DealEffect positiveDealEffect, DealEffect negativeDealEffect) = deals[i];
            DealInstance dealInstance = Instantiate(dealInstancePrefab);
            dealInstance.SetDeal(positiveDealEffect, negativeDealEffect, player);
            dealInstance.transform.position = new(startX + i * dealsDeltaOffset, dealsCenter.position.y, dealsCenter.position.z);
            dealInstance.transform.SetParent(dealsCenter);
            currentDeals.Add(dealInstance);
        }
    }

    public void SelectDeal(DealInstance dealInstance) {
        if (!currentDeals.Contains(dealInstance)) return;
        Player player = dealInstance.player;
        dealInstance.ApplyDealEffects(player);
        currentDeals.ForEach(di => Destroy(di.gameObject));
        currentDeals.Clear();
        player.ChangeState(PlayerState.Regular);
    }
}
