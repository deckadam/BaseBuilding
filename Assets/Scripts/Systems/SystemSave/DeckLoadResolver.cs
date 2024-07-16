using System.Linq;
using Deck.ItemVisualProviders;
using Deck.Save.Data;
using Deck.Utility.Logger;
using UnityEngine;
using Zenject;

namespace Deck.Save
{
    public class DeckLoadResolver
    {
        private DeckInstanceCreator _instanceCreator;
        private DeckItemVisualProviderBasic[] _itemVisualProviders;

        [Inject]
        private void Inject(DeckInstanceCreator instanceCreator, DeckItemVisualProviderBasic[] itemVisualProviders)
        {
            _instanceCreator = instanceCreator;
            _itemVisualProviders = itemVisualProviders;
        }

        public void ResolveAndLoad(DeckComponentHolderSaveDatas agentDatas, DeckItemVisualSaveDatas itemVisualDatas)
        {
            foreach (var deckComponentHolderSaveData in agentDatas.datas)
            {
                if (deckComponentHolderSaveData.prefabId == 0)
                {
                    DeckLogger.Component($"Agent Id has not been set, skipping. {deckComponentHolderSaveData.GetType()}");
                    continue;
                }

                if (deckComponentHolderSaveData.uniqueId == 0)
                {
                    DeckLogger.Component($"Agent Id has not been set, skipping. {deckComponentHolderSaveData.GetType()}");
                    continue;
                }


                var agentInstance = _instanceCreator.CreateNewAgentInstance(deckComponentHolderSaveData.prefabId, deckComponentHolderSaveData.uniqueId);
                agentInstance.LoadData(deckComponentHolderSaveData);
            }


            foreach (var deckItemVisualSaveData in itemVisualDatas.saveDatas)
            {
                var provider = _itemVisualProviders.First(item => item.GetType().ToString() == deckItemVisualSaveData.typeName);

                if (provider == null)
                {
                    DeckLogger.Error("No provider found for item visual data " + deckItemVisualSaveData.typeName);
                    continue;
                }
                
                provider.LoadData(deckItemVisualSaveData.saveData);
            }
        }
    }
}