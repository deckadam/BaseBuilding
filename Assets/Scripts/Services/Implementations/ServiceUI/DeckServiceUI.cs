using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Deck.Components.Building;
using Deck.Utility.Class;
using Deck.Utility.Logger;

namespace Deck.Services
{
    public class DeckServiceUI : DeckServiceBase
    {
        private Dictionary<Type, DeckUIBase> _uiImplementations = new();

        public T GetUI<T>() where T : DeckUIBase
        {
            var typeOfT = typeof(T);
            return (T)_uiImplementations[typeOfT];
        }

        public async void SwapStatus<T>() where T : DeckUIBase
        {
            var typeOfT = typeof(T);
            var temp = _uiImplementations[typeOfT];

            await DisappearAllWindowsExceptRequired<T>(typeOfT);

            if (temp._isAppeared || temp.isAppearing)
            {
                await UniTask.WaitWhile(() => temp.isAppearing);
                await temp.Disappear();
            }
            else if (!temp._isAppeared || !temp.isDisappearing)
            {
                await UniTask.WaitWhile(() => temp.isDisappearing);
                await temp.Appear();
            }

            DeckLogger.UI("UI swap finished  " + typeOfT);
        }

        public async void ShowWindow<T>() where T : DeckUIBase
        {
            var typeOfT = typeof(T);
            var temp = _uiImplementations[typeOfT];
            if (temp._isAppeared || temp.isAppearing)
            {
                return;
            }

            await DisappearAllWindowsExceptRequired<T>(typeOfT);

            await _uiImplementations[typeOfT].Appear();

            DeckLogger.UI("Show window " + typeOfT);
        }

        private async UniTask DisappearAllWindowsExceptRequired<T>(Type typeOfT) where T : DeckUIBase
        {
            foreach (var impl in _uiImplementations)
            {
                if (impl.Value.GetType() != typeOfT)
                {
                    await impl.Value.Disappear();
                }
            }
        }

        public override void Initialize()
        {
            var implementations = DeckClassUtility.GetInheritedClasses<DeckUIBase>();

            foreach (var typeRef in implementations)
            {
                var instance = FindObjectOfType(typeRef) as DeckUIBase;
                if (instance == null)
                {
                    continue;
                }

                _uiImplementations[typeRef] = instance;
                instance.Initialize();
            }
        }

        public override void DeInitialize()
        {
            foreach (var impl in _uiImplementations)
            {
                impl.Value.DeInitialize();
            }
        }

        public override void BeforeGameSessionInitialized()
        {
            foreach (var keyValuePair in _uiImplementations)
            {
                keyValuePair.Value.BeforeGameSessionInitialized();
            }
        }

        public override void AfterGameSessionInitialized()
        {
            
            foreach (var keyValuePair in _uiImplementations)
            {
                keyValuePair.Value.AfterGameSessionInitialized();
            }
        }

        public override void BeforeGameSessionDeinitialized()
        {
            foreach (var keyValuePair in _uiImplementations)
            {
                keyValuePair.Value.BeforeGameSceneUnloaded();
            }
        }
    }
}