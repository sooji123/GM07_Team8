public static class ElementRelations
{
    public static ERelationType EvaluateRelation(EElement attacker, EElement defender)
    {
        if (attacker == EElement.None || defender == EElement.None)
        {
            return ERelationType.None;
        }

        if (attacker == defender)
        {
            return ERelationType.Same;
        }

        switch (attacker)
        {
            case EElement.Water:
                if (defender == EElement.Fire) return ERelationType.Advantage;
                if (defender == EElement.Electric) return ERelationType.Disadvantage;
                break;

            case EElement.Fire:
                if (defender == EElement.Grass) return ERelationType.Advantage;
                if (defender == EElement.Water) return ERelationType.Disadvantage;
                break;

            case EElement.Grass:
                if (defender == EElement.Electric) return ERelationType.Advantage;
                if (defender == EElement.Fire) return ERelationType.Disadvantage;
                break;

            case EElement.Electric:
                if (defender == EElement.Water) return ERelationType.Advantage;
                if (defender == EElement.Grass) return ERelationType.Disadvantage;
                break;
        }

        return ERelationType.None;
    }

    public static float GetDamageMultiplier(ERelationType relation)
    {
        switch (relation)
        {
            case ERelationType.Same:
                return 0.5f;
            case ERelationType.Advantage:
                return 1.5f;
            case ERelationType.Disadvantage:
                return 0.5f;
            case ERelationType.None:
                return 1.0f;
        }

        return 1.0f;
    }
}