public static class ElementRelations
{
    public static ERelationType EvaluateRelation(EElement attacker, EElement defender)
    {
        if(attacker== defender)
        {
            if(attacker== EElement.None)
            {
                return ERelationType.None;
            }

            return ERelationType.Same;
        }

        switch (attacker)
        {
            case EElement.Water:
                if (defender == EElement.Fire)
                {
                    return ERelationType.Advantage;
                }
                if (defender == EElement.Earth)
                {
                    return ERelationType.Disadvantage;
                }
                break;
            case EElement.Fire:
                if (defender == EElement.Grass)
                {
                    return ERelationType.Advantage;
                }
                if (defender == EElement.Water)
                {
                    return ERelationType.Disadvantage;
                }
                break;
            case EElement.Grass:
                if (defender == EElement.Earth)
                {
                    return ERelationType.Advantage;
                }
                if (defender == EElement.Fire)
                {
                    return ERelationType.Disadvantage;
                }
                break;
            case EElement.Earth:
                if (defender == EElement.Water)
                {
                    return ERelationType.Advantage;
                }
                if (defender == EElement.Grass)
                {
                    return ERelationType.Disadvantage;
                }
                break;
        }

        return ERelationType.None;
    }

    public static float GetDamageMultiplier(ERelationType relation) 
    {
        switch (relation)
        {
            case ERelationType.Same:
                return 0;
            case ERelationType.Advantage:
                return 1.5f;
            case ERelationType.Disadvantage:
                return 0.5f;
        }

        return 1f;
    }
}
