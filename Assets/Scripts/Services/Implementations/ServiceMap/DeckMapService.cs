using System.Collections.Generic;
using Deck.Data.Map;
using Deck.InputHandling.Events;
using Deck.Map;
using Deck.Agent;
using Deck.UI.Hotkey.Events;
using Deck.Utility;
using Deck.Utility.Logger;
using GameEvents;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Deck.Services.Implementations.MapService
{
    public class DeckMapService : DeckServiceBase
    {
        [Inject] private DeckBinderMap _binderMap;

        public static DeckMap map { get; private set; }

        private List<DeckAgentCore> _agents;
        public DeckCoreGrid deckCoreGrid { get; private set; }

        public override void Initialize()
        {
            _agents = new List<DeckAgentCore>();
        }

        public void AddCoreAgent(DeckAgentCore agent)
        {
            DeckLogger.Level("Adding player");
            _agents.Add(agent);
            DeckOnCoreAgentCreatedEvent.Create(agent).Send();
            DeckOnActiveHotkeyCountChanged.Create(_agents.Count).Send();
        }

        public void RemoveCoreAgent(DeckAgentCore agent)
        {
            DeckLogger.Level("Removing player");
            _agents.Remove(agent);
            DeckOnActiveHotkeyCountChanged.Create(_agents.Count).Send();
        }

        public void SetGrid(DeckCoreGrid deckCoreGrid)
        {
            DeckLogger.Level("Setting grid");
            this.deckCoreGrid = deckCoreGrid;
        }

        public void CreateMap()
        {
            if (map) Destroy(map.gameObject);
            var temp = new GameObject {name = "Test map"};
            map = temp.AddComponent<DeckMap>();
            DeckOnMapLoadedEvent.Create().Send();
        }

        public void InitializeMap(DeckCoreGrid coreGrid, NavMeshSurface surface, GameObject ground)
        {
            map.Initialize(coreGrid, surface, ground);
        }

        public void CreateGround(DeckCoreGrid deckCoreGrid, out GameObject ground, out Material groundMaterial)
        {
            DeckLogger.Map("Starting ground creation");
            ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.parent = map.transform;
            ground.layer = 6;
            ground.transform.localScale = new Vector3(deckCoreGrid.size.x, 1f, deckCoreGrid.size.y) / 10f;

            var renderer = ground.GetComponent<MeshRenderer>();
            renderer.material = _binderMap.basePlaneMaterial;
            renderer.material.SetVector("_Tiling", new Vector4(deckCoreGrid.size.x, deckCoreGrid.size.y));

            groundMaterial = renderer.material;
            DeckLogger.Map("Finished ground creation");
        }

        public void PopulateMap(DeckCoreGrid deckCoreGrid)
        {
            DeckLogger.Map("Starting to populate map");

            var noise = DeckUtility.GenerateNoiseTexture(deckCoreGrid.size.x, deckCoreGrid.size.y, _binderMap.noiseScale);

            var blockadeParent = new GameObject();
            blockadeParent.transform.parent = map.transform;

            for (var i = 0; i < deckCoreGrid.size.x; i++)
            {
                for (var j = 0; j < deckCoreGrid.size.y; j++)
                {
                    if (!(noise[i, j] > _binderMap.blockadeThreshhold)) continue;
                    Instantiate(_binderMap.blockade, deckCoreGrid.cells[i, j].position, Quaternion.identity, blockadeParent.transform);
                }
            }

            DeckLogger.Map("Finished populating them map");
        }

        public IEnumerable<DeckAgentCore> GetAgents() => _agents;
    }
}