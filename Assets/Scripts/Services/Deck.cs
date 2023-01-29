using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Services;
using Deck.Utility;
using Deck.Utility.Logger;
using Sirenix.Utilities;
using UnityEngine;

namespace Deck
{
    public static class Deck
    {
        private static Dictionary<Type, DeckServiceBase> _services;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _services = new Dictionary<Type, DeckServiceBase>();

            var classes = DeckUtility.GetInheritedClasses<DeckServiceBase>();

            foreach (var typeRef in classes)
            {
                var instance = GameObject.FindObjectOfType(typeRef) as DeckServiceBase;
                if (instance == null)
                {
                    Debug.LogWarning(typeRef.Name + "  service type not instantiated in scene");
                    continue;
                }

                _services[typeRef] = instance;
            }

            var maxWarmUpIndex = _services.Max(item => item.Value.GetWarmUpIndex());
            for (var i = maxWarmUpIndex; i >= 0; i--)
            {
                _services.ForEach(item => item.Value.ControlledWarmUp(i));
            }

            foreach (var service in _services)
            {
                try
                {
                    service.Value.Initialize();
                    DeckLogger.Service(service.Value.GetType().Name + "  initialized succesfully");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }


        public static T GetService<T>() where T : DeckServiceBase
        {
            var typeOfT = typeof(T);
            return (T) _services[typeOfT];
        }
    }
}