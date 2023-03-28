using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Deck;
using Deck.Data.Map;
using Deck.Services;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Deck.Events.MapService
{
    public class DeckServiceMap : DeckServiceBase
    {
        public static DeckMap GetMap()
        {
            if (map != null)
            {
                return map;
            }

            map = FindObjectOfType<DeckMap>();
            return map;
        }

        private static DeckMap map;

        private List<DeckAgentCore> _agents;

        private DiContainer _container;

        [Inject]
        private void Inject(DiContainer container)
        {
            _container = container;
        }

        public override void Initialize()
        {
            _agents = new List<DeckAgentCore>();
        }

        public async UniTask LoadMap()
        {
            var isLoaded = SceneManager.GetSceneByName("MapScene").isLoaded;
            if (isLoaded)
            {
                DeckLogger.Map("Old map scene unloading");
                await SceneManager.UnloadSceneAsync("MapScene");
                DeckLogger.Map("Old map scene unloaded");
            }

            DeckLogger.Map("New map scene loading");
            await SceneManager.LoadSceneAsync("MapScene", LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("MapScene"));
            DeckLogger.Map("New map scene loaded");
        }

        public void CreateGround(DeckBinderMap binderMap)
        {
            DeckLogger.Map("Starting ground creation");
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.parent = map.transform;
            ground.layer = 6;
            ground.transform.localScale = new Vector3(binderMap.GetSize().x, 1f, binderMap.GetSize().y) / 10f;

            DeckLogger.Map("Finished ground creation");
        }
    }
}