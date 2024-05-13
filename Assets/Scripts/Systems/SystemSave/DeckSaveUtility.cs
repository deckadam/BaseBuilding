using UnityEngine;

namespace Deck.Save
{
    public static class DeckSaveUtility
    {
        public static string GetSerializedData(object data)
        {
            return JsonUtility.ToJson(data);
        }

        public static T GetDeserializedData<T>(string data)
        {
            return JsonUtility.FromJson<T>(data);
        }
    }
}