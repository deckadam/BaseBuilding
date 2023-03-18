using UnityEngine;

namespace Deck.Utility
{
    public static class DeckShaderConstants
    {
        public static readonly int SelectionTexture = Shader.PropertyToID("_SelectionTexture");
        public static readonly int MousePos = Shader.PropertyToID("_MousePos");
        public static readonly int Range = Shader.PropertyToID("_Range");
    }
}