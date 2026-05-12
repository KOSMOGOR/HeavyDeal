using System;

[Serializable]
public class GainOxygenDE : DealEffect
{
    public float oxygenToAdd;
    public int oxygenTanksToAdd;

    public override string Description {
        get {
            string oxygenText = oxygenToAdd != 0 ? $"{oxygenToAdd} кислорода" : null;
            string oxygenTanksText = oxygenTanksToAdd != 0 ? $"{oxygenTanksToAdd} баллона кислорода" : null;
            return oxygenText != null && oxygenTanksText != null ?
                $"Добавляет {oxygenText} и {oxygenTanksText}" : $"Добавляет {oxygenTanksText ?? oxygenText}";
        }
    }

    public override void ApplyEffect(Player player) {
        // player.currentOxygenTank += oxygenToAdd;
        player.oxygenTanks += oxygenTanksToAdd;
    }
}
