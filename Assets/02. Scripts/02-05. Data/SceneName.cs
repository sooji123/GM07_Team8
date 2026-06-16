using System.Collections.Generic;
public static class SceneName
{
    private static readonly Dictionary<EScenes, string> _sceneTable =
        new Dictionary<EScenes, string>()
        {
            { EScenes.Title, "TitleScene" },
            { EScenes.Game, "GameScene" },
            { EScenes.GameOver, "GameOverScene" },
        };

    public static string GetSceneName(EScenes type)
    {
        return _sceneTable[type];
    }
}
