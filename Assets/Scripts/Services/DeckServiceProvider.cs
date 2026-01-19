using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Object = UnityEngine.Object;

namespace Services
{
    public static class DeckServiceProvider
    {
        private static Dictionary<Type, DeckServiceBase> _services;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            _services = new Dictionary<Type, DeckServiceBase>();

            var classes = DeckClassUtility.GetInheritedClasses<DeckServiceBase>();

            foreach (var typeRef in classes)
            {
                var instance = Object.FindObjectOfType(typeRef) as DeckServiceBase;
                if (instance == null)
                {
                    DeckLogger.Warning(typeRef.Name + "  service type not instantiated in scene");
                    continue;
                }

                _services[typeRef] = instance;
            }

            var maxWarmUpIndex = _services.Max(item => item.Value.GetWarmUpIndex());
            for (var i = maxWarmUpIndex; i >= 0; i--)
            {
                foreach (var deckServiceBase in _services)
                {
                    deckServiceBase.Value.ControlledWarmUp(i);
                }
            }

            foreach (var service in _services)
            {
                try
                {
                    service.Value.Initialize();
                    DeckLogger.Service(service.Value.GetType().Name + "  initialized successfully");
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
            return (T)_services[typeOfT];
        }


        public static void BeforeGameSessionInitialized()
        {
            foreach (var deckServiceBase in _services)
            {
                deckServiceBase.Value.BeforeGameSessionInitialized();
            }
        }

        public static void AfterGameSessionInitialized()
        {
            foreach (var deckServiceBase in _services)
            {
                deckServiceBase.Value.AfterGameSessionInitialized();
            }
        }

        public static void BeforeGameSceneUnloaded()
        {
            foreach (var deckServiceBase in _services)
            {
                deckServiceBase.Value.BeforeGameSessionDeInitialized();
            }
        }

        public static void BeforeSaveRequest()
        {
            foreach (var deckServiceBase in _services)
            {
                deckServiceBase.Value.BeforeSaveRequest();
            }
        }
    }
}