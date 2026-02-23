using System.Text;

public static class CardTextBuilder
{
    public static string Build(CardDefinition def)
    {
        if (def == null || def.effects == null) return "";

        StringBuilder sb = new StringBuilder();
        if (def.cardText.Length!=0)
        {
            return def.cardText;
        }
        if (def.castTimeTurns > 0)
            sb.AppendLine($"[{def.castTimeTurns}턴 후 발동]");

        foreach (var e in def.effects)
        {
            sb.AppendLine(EffectToText(e));
        }

        return sb.ToString();
    }

    private static string EffectToText(EffectDefinition e)
    {
        switch (e.type)
        {
            case EffectType.Damage:
                return $"적에게 {e.value} 피해";

            case EffectType.GainBlock:
                return $"방어도 {e.value} 획득";

            case EffectType.Dodge:
                return $"다음 공격 회피";

            case EffectType.ApplyFilter:
                return FilterText(e);

            default:
                return "";
        }
    }

    private static string FilterText(EffectDefinition e)
    {
        switch (e.filterType)
        {
            case FilterType.NextNAttacksBonus:
                return $"다음 공격 {e.stacks}회 피해 +{e.magnitude}";

            case FilterType.NextEnemyHitDouble:
                return $"적이 받는 다음 피해 {e.stacks}회 2배";

            default:
                return "";
        }
    }
}