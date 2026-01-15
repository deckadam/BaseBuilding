using System.Collections.Generic;
using Deck.Utility;
using UnityEngine;

namespace Utility
{
    public class DeckGUILogger : DeckSingleton<DeckGUILogger>
    {
        public bool enabledByHotkey;
        public KeyCode enablingKey;

        private Dictionary<string, string> _debugTexts = new Dictionary<string, string>();

        private int _width;
        private int _height;
        private int _fontSize;
        public int fontSizeMultiplier = 3;
        public Color textColor = Color.black;

        public void SetDebugText(string key, object value, bool showKey = true)
        {
            var displayText = showKey ? (key + ": " + value) : value.ToString();
            _debugTexts[key] = displayText;
        }

        private bool _visibilitySwap;

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(enablingKey))
            {
                _visibilitySwap = !_visibilitySwap;
            }
        }
#endif


        private void Start()
        {
            _width = Screen.width;
            _height = Screen.height;
            _fontSize = _height * fontSizeMultiplier / 100;
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (enabledByHotkey && !_visibilitySwap) return;
            var counter = 0;
            foreach (var debug in _debugTexts.Keys)
            {
                var style = new GUIStyle();
                var rect = new Rect(0, _fontSize * counter, _width, _fontSize * counter++);
                style.alignment = TextAnchor.UpperLeft;
                style.fontSize = _fontSize;
                style.normal.textColor = textColor;
                GUI.Box(rect, _debugTexts[debug], style);
                // GUI.Label(rect, debugTexts[debug], style);
            }
        }
#endif
    }
}