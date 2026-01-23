using Services.Timing.Events;
using UnityEngine;
using Utility;

namespace Services.Timing
{
    public class DeckServiceTiming : DeckServiceBase
    {
        private float _currentTimeScale = 1f;
        private bool _isRunning;
        private bool _isPaused;

        private float _currentSessionTime;

        public override void AfterGameSessionInitialized()
        {
            _isRunning = true;
            _isPaused = true;
            _currentSessionTime = 0;
        }

        public override void BeforeGameSessionDeinitialized()
        {
            _isRunning = false;
        }

        private void Update()
        {
            if (!_isRunning || _isPaused)
            {
                return;
            }

            var deltaTime = Time.deltaTime * _currentTimeScale;
            _currentSessionTime += deltaTime;
            DeckGUILogger.ins.SetDebugText("Time update", _currentSessionTime);
            DeckEventOnUpdate.Create(deltaTime).Send();
        }

        public void OnPlayPauseChangeRequested()
        {
            _isPaused = !_isPaused;
            _currentTimeScale = _isPaused ? 0f : 1f;
        }
    }
}