using System.Collections.Generic;
using UnityEngine;

namespace Deck.Utility
{
    public static class DeckUtility
    {
        public static float[,] GenerateNoiseTexture(int width, int height, float noiseScale)
        {
            var result = new float[width, height];
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    result[i, j] = Mathf.PerlinNoise(i * noiseScale, j * noiseScale) - 0.5f;
                }
            }

            return result;
        }

        public static Texture2D GenerateTexture(Vector2Int size)
        {
            var texture2D = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false, true);
            texture2D.filterMode = FilterMode.Point;
            return texture2D;
        }

        public static T GetRandom<T>(this T[] array)
        {
            var index = Random.Range(0, array.Length);
            return array[index];
        }

        public static T GetRandom<T>(this List<T> list)
        {
            var index = Random.Range(0, list.Count);
            return list[index];
        }
    }
}