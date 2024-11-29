using System;
using Deck.Base;
using Deck.Components;
using Deck.General;
using Deck.InGame.Agent.Building;
using Deck.Save;
using Deck.Services.Finder;
using Deck.Services.Tables.Events;
using Deck.Utility;
using Deck.Utility.Constants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Deck.InGame.Agent.Furniture
{
    public class DeckAgentChair : DeckBuilding, IDeckSittable
    {
        [SerializeField, ReadOnly] private DeckAgentHumanoid _humanoid;

        public bool InProcess => _inProcess;
        public bool IsAvailable => _isAvailable && !_inProcess;

        private bool _inProcess;
        private bool _isAvailable = true;


        protected override void InternalAfterBuildingInitialized()
        {
            DeckEventOnChairPlaced.Create(this).Send();
        }

        protected override void OnAgentDestroyed()
        {
            DeckEventOnChairDestroyed.Create(this).Send();
        }

        public void SetOccupied()
        {
            _inProcess = true;
        }

        public void OnSit(DeckAgentHumanoid humanoid)
        {
            _isAvailable = false;
            _inProcess = false;
            _humanoid = humanoid;
        }

        public void OnGetUp(DeckAgentHumanoid humanoid)
        {
            _isAvailable = true;
            _inProcess = false;
            _humanoid = null;
        }

        public void OnTableDestroyed()
        {
            if (IsAvailable)
            {
                return;
            }

            _humanoid.GetUp();
        }

        protected override string InternalGetAdditionalBuildingData()
        {
            if (IsAvailable)
            {
                return null;
            }

            if (_isAvailable)
            {
                return DeckSaveUtility.GetSerializedData(new ChairData()
                {
                    sittingHumanoidUniqueId = 0,
                    inProcess = _inProcess,
                    isAvailable = _isAvailable
                });
            }

            return DeckSaveUtility.GetSerializedData(new ChairData()
            {
                sittingHumanoidUniqueId = _humanoid.UniqueId.ID,
                inProcess = _inProcess,
                isAvailable = _isAvailable
            });
        }

        protected override void InternalLoadAdditionalBuildingData(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            var chairData = DeckSaveUtility.GetDeserializedData<ChairData>(data);
            _inProcess = chairData.inProcess;
            _isAvailable = chairData.isAvailable;

            if (chairData.sittingHumanoidUniqueId == 0) return;

            var humanoid = (DeckAgentHumanoid)Deck.GetService<DeckServiceFinder>().GetAgent(chairData.sittingHumanoidUniqueId);

            SetOccupied();
            OnSit(humanoid);
            if (!_inProcess && !_isAvailable)
            {
                humanoid.GetDeckComponent<IDeckAnimationSetTrigger>().Trigger(DeckConstantsAnimation.Sit);
            }
        }

        [Serializable]
        private class ChairData
        {
            public bool isAvailable;
            public bool inProcess;
            public int sittingHumanoidUniqueId;
        }
    }
}