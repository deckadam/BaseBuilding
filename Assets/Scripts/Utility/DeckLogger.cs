using UnityEngine;

namespace Deck.Utility.Logger
{
    public static class DeckLogger
    {
        public static void System(string log, GameObject obj = null)
        {
            Debug.Log("#System#" + log, obj);
        }

        public static void Map(string log, GameObject obj = null)
        {
            Debug.Log("#Map#" + log, obj);
        }

        public static void Navigation(string log, GameObject obj = null)
        {
            Debug.Log("#Navigation#" + log, obj);
        }

        public static void Level(string log, GameObject obj = null)
        {
            Debug.Log("#Level#" + log, obj);
        }

        public static void Grid(string log, GameObject obj = null)
        {
            Debug.Log("#Grid#" + log, obj);
        }

        public static void UI(string log, GameObject obj = null)
        {
            Debug.Log("#UI#" + log, obj);
        }

        public static void Inform(string log, GameObject obj = null)
        {
            Debug.Log("#Inform#" + log, obj);
        }
    }
}