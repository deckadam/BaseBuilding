using UnityEngine;

namespace Deck.Utility.Logger
{
    public static class DeckLogger
    {
        public static void Service(string log, GameObject obj = null)
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

        public static void UI(string log, GameObject obj = null)
        {
            Debug.Log("#UI#" + log, obj);
        }

        public static void Error(string log, GameObject obj = null)
        {
            Debug.Log("#Error#" + log, obj);
        }

        public static void Inform(string log, GameObject obj = null)
        {
            Debug.Log("#Inform#" + log, obj);
        }
        
        public static void Warning(string log, GameObject obj = null)
        {
            Debug.Log("#Warning#" + log, obj);
        }

        public static void Success(string log, GameObject obj = null)
        {
            Debug.Log("#Success#" + log, obj);
        }

        public static void Component(string log, GameObject obj = null)
        {
            Debug.Log("#Component#" + log, obj);
        }

        public static void Command(string log, GameObject obj = null)
        {
            Debug.Log("#Command#" + log, obj);
        }

        
        public static void Save(string log, GameObject obj = null)
        {
            Debug.Log("#Save#" + log, obj);
        }

        public static void System(string log, GameObject obj = null)
        {
            Debug.Log("#System#" + log, obj);
        }
    }
}