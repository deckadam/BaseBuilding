using System;
using Deck.UI.Building.BuildMode;

namespace UI.Building.BuildMode
{
    public static class DeckEscapableBuildModeProvider
    {
        public static DeckEscapableBuildMode GetEscapableBuildMode(DeckBuildMode mode)
        {
            return mode switch
            {
                DeckBuildMode.Free => new DeckEscapableBuildModeFree(),
                DeckBuildMode.Line => new DeckEscapableBuildModeLine(),
                DeckBuildMode.Rect => new DeckEscapableBuildModeRect(),
                DeckBuildMode.InCell => new DeckEscapableBuildModeInCell(false),
                DeckBuildMode.InCellCanReplace => new DeckEscapableBuildModeInCell(true),
                DeckBuildMode.OnTop => new DeckEscapableBuildModeOnTop(),
                DeckBuildMode.OnWall => new DeckEscapableBuildModeOnWall(),
                DeckBuildMode.BuildOnTopWithAccessArea => new DeckEscapableBuildModeBuildOnTopWithAccessArea(),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }
    }

    public enum DeckBuildMode
    {
        Free = 0,
        InCell = 100,
        InCellCanReplace = 101,
        OnWall = 200,
        OnTop = 300,
        Rect = 400,
        Line = 500,
        BuildOnTopWithAccessArea = 600,
    }

    public enum DeckRotationMode
    {
        Continuous,
        NinetyDegree,
        None = 100
    }
}