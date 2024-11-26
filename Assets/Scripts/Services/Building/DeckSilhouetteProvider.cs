using System.Collections.Generic;
using Deck.Data.Buildable;
using Deck.Data.General;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace Deck.Services.Building
{
    public class DeckSilhouetteProvider
    {
        private readonly Dictionary<int, Stack<DeckSilhouettePiece>> _pieceInPool = new();

        private DeckDataBuilding _buildingData;

        [Inject]
        private void Inject(DeckDataBuilding buildingData)
        {
            _buildingData = buildingData;
        }
        
        public DeckSilhouettePiece GetSilhouettePiece(DeckBuildable activeBuildable)
        {
            var silhouetteData = activeBuildable.Silhouette;

            if (!_pieceInPool.TryGetValue(silhouetteData.Length, out var pool))
            {
                pool = new Stack<DeckSilhouettePiece>();
                _pieceInPool[silhouetteData.Length] = pool;
            }

            var stack = _pieceInPool[silhouetteData.Length];
            if (stack.Count > 0)
            {
                var piece = stack.Pop();

                for (var i = 0; i < silhouetteData.Length; i++)
                {
                    var data = silhouetteData[i];
                    piece.filters[i].mesh = data.GetMesh();
                    piece.filters[i].transform.localPosition = data.GetPosition();
                }

                piece.gameObject.SetActive(true);
                return piece;
            }

            var pieceParent = new GameObject();
            pieceParent.name = silhouetteData.Length.ToString();
            var newPiece = new DeckSilhouettePiece();
            newPiece.gameObject = pieceParent;
            newPiece.renderers = new MeshRenderer[silhouetteData.Length];
            newPiece.filters = new MeshFilter[silhouetteData.Length];

            for (var index = 0; index < silhouetteData.Length; index++)
            {
                var data = silhouetteData[index];

                CreateNewSilhouettePiece(activeBuildable.materialCount, data.GetPosition(), data.GetMesh(), index, pieceParent, newPiece);
            }

            return newPiece;
        }

        private void CreateNewSilhouettePiece(int materialCount, Vector3 localPosition, Mesh silhouetteMesh, int index, GameObject newGameObject, DeckSilhouettePiece newPiece)
        {
            var newObject = new GameObject();
            newObject.transform.SetParent(newGameObject.transform);
            newObject.transform.localPosition = localPosition;
            var newFilter = newObject.AddComponent<MeshFilter>();

            newPiece.filters[index] = newFilter;

            newFilter.mesh = silhouetteMesh;
            var newRenderer = newObject.AddComponent<MeshRenderer>();
            newRenderer.shadowCastingMode = ShadowCastingMode.Off;
            var materials = new Material[materialCount];

            newPiece.renderers[index] = newRenderer;

            for (var i = 0; i < materialCount; i++)
            {
                materials[i] = _buildingData.GetAvailableMaterial();
            }

            newRenderer.sharedMaterials = materials;
        }

        public DeckSilhouettePiece GetAccessAreaSilhouettePiece()
        {
            if (!_pieceInPool.TryGetValue(1, out var pool))
            {
                pool = new Stack<DeckSilhouettePiece>();
                _pieceInPool[1] = pool;
            }

            var stack = _pieceInPool[1];
            if (stack.Count > 0)
            {
                var piece = stack.Pop();

                piece.filters[0].mesh = _buildingData.GetAccessCellMesh();
                piece.filters[0].transform.localPosition = Vector3.zero;

                piece.gameObject.SetActive(true);
                return piece;
            }

            var newGameObject = new GameObject();
            newGameObject.name = "1";
            var newPiece = new DeckSilhouettePiece();
            newPiece.gameObject = newGameObject;
            newPiece.renderers = new MeshRenderer[1];
            newPiece.filters = new MeshFilter[1];

            CreateNewSilhouettePiece(1, Vector3.zero, _buildingData.GetAccessCellMesh(), 0, newGameObject, newPiece);
            return newPiece;
        }

        public void ReturnSilhouettePieceToPool(DeckSilhouettePiece piece)
        {
            if (piece == null || !piece.gameObject)
            {
                return;
            }

            piece.gameObject.SetActive(false);
            _pieceInPool[piece.filters.Length].Push(piece);
        }
    }
}

public class DeckSilhouettePiece
{
    public GameObject gameObject;
    public MeshRenderer[] renderers;
    public MeshFilter[] filters;
}