using System;
using System.Linq;

[Serializable]
public class ReviveCE : CardEffect
{
    public int oxygenTanksToAdd;

    public override string Description => $"При проигрыше: удаляет себя, все мёртвые карты из руки и даёт {oxygenTanksToAdd} {(oxygenTanksToAdd == 1 ? "кислородный баллон" : "кислородных баллона")}";

    public override void OnDeath(Player player, CardInstance card) {
        player.RemoveCards(player.GetCardListInPlace(PlayerCardPlace.Hand).Where(card => card.cardData == GameManager.I.deadCard));
        player.oxygenTanks += oxygenTanksToAdd;
    }
}
