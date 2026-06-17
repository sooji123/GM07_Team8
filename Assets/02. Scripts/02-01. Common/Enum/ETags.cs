using System.Collections.Generic;

public enum ETags
{
    None,
    Enemy,
}

public static class Tags
{
    private static readonly Dictionary<ETags, string> _tagTable =
        new Dictionary<ETags, string>()
        {
            { ETags.Enemy, "Enemy" },
        };

    public static string GetTagName(ETags type)
    {
        return _tagTable[type];
    }
}
