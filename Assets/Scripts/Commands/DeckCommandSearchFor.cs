using System;
using System.Threading;
using Base;
using Cysharp.Threading.Tasks;
using Data.Agent.Search;
using Services;
using Services.Finder;
using UnityEngine;
using Utility;

namespace Commands
{
    public class DeckCommandSearchFor<T> : DeckCommand where T: DeckAgent
    {
        private DeckAgent _agent;
        private Action<T> _onFound;
        private DeckDataSoldierSearch _data;

        public DeckCommandSearchFor(DeckAgent agent, Action<T> OnFound)
        {
            _onFound = OnFound;
            _agent = agent;
            _data = _agent.GetAgentData<DeckDataSoldierSearch>();
            Debug.LogError("Command created");
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            var finder = DeckServiceProvider.GetService<DeckServiceFinder>();
            var found = false;

            while (true)
            {
                Debug.LogError("Searching");
                await UniTask.NextFrame(token);

                var searchedFor = finder.GetAgentsWithTag(DeckActionTag.Raider);
                foreach (var temp in searchedFor)
                {
                    DeckGUILogger.ins.SetDebugText(temp.name,Vector3.Distance(temp.GetPosition(), _agent.GetPosition()));
                    if (!(Vector3.Distance(temp.GetPosition(), _agent.GetPosition()) < _data.SearchRadius)) continue;
                    
                    _onFound?.Invoke(temp as T);
                    found = true;
                    break;
                }

                if (found)
                {
                    break;
                }
            }

            return true;
        }
    }
}