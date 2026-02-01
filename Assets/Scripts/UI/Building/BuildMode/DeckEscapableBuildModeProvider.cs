using System;
using UI.Building.BuildMode.Free;
using UI.Building.BuildMode.InCell;
using UI.Building.BuildMode.InCellMultiple;
using UI.Building.BuildMode.Line;
using UI.Building.BuildMode.OnSurface;
using UI.Building.BuildMode.OnTop;
using UI.Building.BuildMode.OnTopWithAccessArea;
using UI.Building.BuildMode.Rect;

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
                DeckBuildMode.InCellMultiple => new DeckEscapableBuildModeInCellMultiple(),
                DeckBuildMode.OnTop => new DeckEscapableBuildModeOnTop(),
                DeckBuildMode.OnWall => new DeckEscapableBuildModeOnSurface(),
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
        InCellMultiple = 102,
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