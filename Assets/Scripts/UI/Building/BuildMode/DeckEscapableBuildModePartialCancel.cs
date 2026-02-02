using Services.Building.Events;
using UnityEngine;
using Utility;

namespace UI.Building.BuildMode
{
    public abstract class DeckEscapableBuildModePartialCancel : DeckEscapableBuildMode
    {
        private bool _hasPartialCanceled;

        protected override void PartialCancel()
        {
            _isDown = false;
            BuildingService.PartialClear();
            _hasPartialCanceled = true;
            DeckEventOnBuildModeStopped.Create().Send();
        }

        protected override void OnPartialContinue()
        {
            if (!_hasPartialCanceled) return;
            DeckEventOnBuildModeStarted.Create().Send();
            _hasPartialCanceled = false;
        }
    }
}