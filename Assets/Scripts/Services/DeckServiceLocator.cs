using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Deck.Utility.Logger;
using Sirenix.Utilities;
using UnityEngine;

namespace Deck.Services
{
    public static class DeckServiceLocator
    {
        private static Dictionary<Type, DeckServiceBase> _services;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _services = new Dictionary<Type, DeckServiceBase>();

            var classes = GetInheritedClasses();

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
                    DeckLogger.System(service.Value.GetType().Name + "  initialized succesfully");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private static IEnumerable<Type> GetInheritedClasses()
        {
            //if you want the abstract classes drop the !TheType.IsAbstract but it is probably to instance so its a good idea to keep it.
            return Assembly.GetAssembly(typeof(DeckServiceBase))
                .GetTypes()
                .Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(typeof(DeckServiceBase)));
        }

        public static T GetService<T>() where T : DeckServiceBase
        {
            var typeOfT = typeof(T);
            return (T) _services[typeOfT];
        }
    }
}