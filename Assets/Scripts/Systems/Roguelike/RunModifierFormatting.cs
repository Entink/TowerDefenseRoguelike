using System.Text;

public static class RunModifierFormatting
{
    public static string BuildTooltip(RunModifierDef def, int stacks)
    {
        if (def == null) return "";
        var sb = new StringBuilder();

        sb.Append(def.displayName);
        if (stacks > 1) sb.Append($" x{stacks}");
        sb.AppendLine();
        sb.AppendLine();

        if(!string.IsNullOrEmpty(def.shortDescription))
        {
            sb.AppendLine(def.shortDescription);
        }

        var totals = GetTotals(def.id, stacks);
        if(!string.IsNullOrEmpty(totals))
        {
            if (!string.IsNullOrEmpty(def.shortDescription)) sb.AppendLine();
            sb.AppendLine(totals);
        }

        return sb.ToString();
    }

    static string GetTotals(RunModifierId id, int stacks)
    {
        if (stacks <= 0) stacks = 1;

        switch(id)
        {
            case RunModifierId.RecruitmentDrive:
                {
                    int cost = -15 * stacks;
                    int cd = +20 * stacks;
                    return $"Total:\n {Fmt(cost)} cost\n {Fmt(cd)} cooldown";
                }
            case RunModifierId.RushOrders:
                {
                    int cost = +10 * stacks;
                    int cd = -20 * stacks;
                    return $"Total:\n {Fmt(cost)} cost\n {Fmt(cd)} cooldown";
                }
            case RunModifierId.BulkDiscount:
                {
                    int cost = -10 * stacks;
                    return $"Total:\n {Fmt(cost)} cost";
                }
            case RunModifierId.CommandLogistics:
                {
                    int total = 1 * stacks;
                    return $"Total:\n +{total} Max Units\nCurrent limit: {GameManager.instance.GetMaxUnits()}";
                }
            case RunModifierId.GoldMine:
                {
                    int pct = 50 * stacks;
                    return $"Total:\n +{pct}% gold per second";
                }
            case RunModifierId.BetterPlatings:
                {
                    int pct = 20 * stacks;
                    return $"Total:\n +{pct}% Max HP";
                }
            case RunModifierId.SharpenedBlades:
                {
                    int pct = 10 * stacks;
                    return $"Total:\n +{pct}% Attack Damage";
                }
            case RunModifierId.BlackMarket:
                {
                    int cost = -50 * stacks;
                    int hp = -10 * stacks;
                    int dmg = -10 * stacks;
                    return $"Total:\n {Fmt(cost)} cost\n {Fmt(hp)} Max HP\n {Fmt(dmg)} Attack Damage";
                }
        }
        return "";
    }

    static string Fmt(int pct)
    {
        return (pct >= 0) ? $"+{pct}%" : $"{pct}%";
    }
}