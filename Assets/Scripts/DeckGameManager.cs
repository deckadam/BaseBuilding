using System.Collections.Generic;
using Deck.Components;
using Deck.Data.Item;
using Deck.MVC;
using UnityEngine;

namespace Deck.Test
{
    public class DeckGameManager : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeGame()
        {
            DeckMVC<DeckItem, IEnumerable<DeckItem>>.ResetController();
            DeckMVC<DeckHealthComponent, IEnumerable<DeckHealthComponent>>.ResetController();
        }
    }
}