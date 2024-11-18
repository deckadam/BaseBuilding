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
                DeckBuildMode.InCell => new DeckEscapableBuildModeInCell(),
                DeckBuildMode.OnTop => new DeckEscapableBuildModeOnTop(),
                DeckBuildMode.OnWall => new DeckEscapableBuildModeOnWall(),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }
    }

    public enum DeckBuildMode
    {
        Free,
        InCell,
        OnWall,
        OnTop,
        Rect,
        Line
    }
}