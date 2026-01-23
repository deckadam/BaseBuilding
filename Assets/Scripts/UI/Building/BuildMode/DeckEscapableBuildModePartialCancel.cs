namespace UI.Building.BuildMode
{
    public abstract class DeckEscapableBuildModePartialCancel:DeckEscapableBuildMode
    {
        protected override void PartialCancel()
        {
            _isDown = false;
            BuildingService.PartialClear();
        }
    }
}