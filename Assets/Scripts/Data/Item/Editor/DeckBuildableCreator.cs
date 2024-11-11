using System;
using System.Collections.Generic;
using Deck.Components;
using Deck.Components.Building;
using Deck.Data.Buildable;
using Deck.ItemVisualProviders;
using Deck.Save;
using Deck.UI.Building;
using Deck.UI.Building.BuildingSets;
using Deck.UI.Building.BuildingSets.BuildingMiscellaneous;
using Deck.UI.Building.BuildingSets.BuildingWallsAndDoors;
using Deck.UI.Building.BuildingSets.DeckBuildingFurniture;
using Deck.Utility.Logger;
using UnityEditor;
using UnityEngine;

namespace Deck.Data.Item.Editor
{
    public class DeckBuildableCreator : EditorWindow
    {
        private List<DeckActionTag> _tags = new();
        private GameObject _itemVisual;
        private GameObject _model;
        private bool _isBoxCollider;
        private Sprite _icon;
        private int _amount;
        private bool _rotatable = true;
        private bool _canBeHangedToWall;
        private bool _isBasicItem = true;

        private DeckBuildableType _selectedType;


        [MenuItem("Deck/Item Creator")]
        private static void ShowWindow()
        {
            var window = GetWindow<DeckBuildableCreator>();
            window.titleContent = new GUIContent("Deck Buildable Creator");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(25);
            EditorGUILayout.Space(10);
            _icon = (Sprite)EditorGUILayout.ObjectField("Icon: ", _icon, typeof(Sprite), false);
            EditorGUILayout.Space(10);
            _amount = EditorGUILayout.IntField("Amount: ", _amount);
            _isBoxCollider = EditorGUILayout.Toggle("Is box collider: ", _isBoxCollider);
            _rotatable = EditorGUILayout.Toggle("Is rotatable: ", _rotatable);
            _canBeHangedToWall = EditorGUILayout.Toggle("Can be hanged to wall: ", _canBeHangedToWall);
            _isBasicItem = EditorGUILayout.Toggle("Is basic item: ", _isBasicItem);
            _model = (GameObject)EditorGUILayout.ObjectField("Item model: ", _model, typeof(GameObject), false);
            _selectedType = (DeckBuildableType)EditorGUILayout.EnumPopup("Buildable type: ", _selectedType);

            for (var i = 0; i < _tags.Count; i++)
            {
                _tags[i] = (DeckActionTag)EditorGUILayout.EnumFlagsField("Item visual: ", DeckActionTag.Invalid);
            }

            if (GUILayout.Button("Add tag"))
            {
                AddTagToEnd();
            }

            if (GUILayout.Button("Remove tag"))
            {
                RemoveTagFromEnd();
            }

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Create Item"))
            {
                CreateItem();
            }

            EditorGUILayout.EndVertical();
        }

        private void CreateItem()
        {
            string suffix;

            DeckBuildingPage page = null;
            switch (_selectedType)
            {
                case DeckBuildableType.WallsAndDoors:
                    page = Resources.FindObjectsOfTypeAll<DeckBuildingPageWallsAndDoors>()[0];
                    suffix = "Wall";
                    break;

                case DeckBuildableType.Furniture:
                    page = Resources.FindObjectsOfTypeAll<DeckBuildingPageFurniture>()[0];
                    suffix = "Furniture";
                    break;

                case DeckBuildableType.Miscellaneous:
                    page = Resources.FindObjectsOfTypeAll<DeckBuildingPageMiscellaneous>()[0];
                    suffix = "Misc";
                    break;

                case DeckBuildableType.BarTable:
                    page = Resources.FindObjectsOfTypeAll<DeckBuildingPageMiscellaneous>()[0];
                    suffix = "Misc";
                    break;

                default:
                    throw new Exception("Page not found");
            }


            var itemVisualPrefabName = "Assets/Prefabs/ItemVisualPrefabs/DeckItemVisual" + suffix + _model.name + ".prefab";
            var agentPrefabName = "Assets/Prefabs/AgentPrefabs/DeckAgent" + suffix + _model.name + ".prefab";
            var assetName = "Assets/Resources/Data/Items/DeckBuildable" + suffix + _model.name + ".asset";

            var itemVisualInstance = CreateItemVisualInstance();
            var itemVisualPrefab = PrefabUtility.SaveAsPrefabAsset(itemVisualInstance.gameObject, itemVisualPrefabName).GetComponent<DeckItemVisual>();
            itemVisualPrefab.SetNewUniqueId();

            var agentInstance = CreateAgentInstance();
            var agentPrefab = PrefabUtility.SaveAsPrefabAsset(agentInstance, agentPrefabName).GetComponent<DeckBuilding>();
            agentPrefab.SetNewUniqueId();

            var buildingData = DeckBuildable.Create(_model.name, _icon, itemVisualPrefab, agentPrefab, _rotatable, _canBeHangedToWall);
            AssetDatabase.CreateAsset(buildingData, assetName);

            agentPrefab.SetBuildingData(buildingData);

            UpdatePrefabData(itemVisualPrefab, agentPrefab, buildingData, itemVisualInstance, agentInstance);

            if (_isBasicItem)
            {
                var visualProvider = Resources.FindObjectsOfTypeAll<DeckItemVisualProviderBasic>();

                foreach (var deckItemVisualProviderBasic in visualProvider)
                {
                    if (deckItemVisualProviderBasic.GetType() == typeof(DeckItemVisualProviderBasic))
                    {
                        deckItemVisualProviderBasic.AddItemVisual(itemVisualPrefab);
                    }
                }
            }
            else
            {
                Debug.LogError("Please add item to proper item visual provider!!!");
            }

            // visualProvider.AddItemVisual(itemVisualPrefab);

            var instanceProvider = Resources.FindObjectsOfTypeAll<DeckInstanceProvider>()[0];
            instanceProvider.AddAgent(agentPrefab);
            instanceProvider.AddItemVisual(itemVisualPrefab);

            page.AddBuildable(buildingData);


            UpdateEditor();

            DeckLogger.Success(_model.name + "  item and prefab successfully created");
        }

        private static void UpdateEditor()
        {
            AssetDatabase.Refresh();
            EditorApplication.RepaintProjectWindow();
        }

        private static void UpdatePrefabData(DeckItemVisual itemVisualPrefab, DeckAgent agentPrefab, DeckBuildable newScriptableObject, GameObject itemVisualInstance, GameObject agentInstance)
        {
            newScriptableObject.SetAgent(agentPrefab);
            newScriptableObject.SetItemVisual(itemVisualPrefab);

            DestroyImmediate(agentInstance);
            DestroyImmediate(itemVisualInstance);
        }

        private GameObject CreateAgentInstance()
        {
            var newObject = new GameObject();
            newObject.name = _model.name;

            var agentInstance = newObject.AddComponent<DeckBuilding>();
            agentInstance.OnValidate();

            return newObject;
        }

        private GameObject CreateItemVisualInstance()
        {
            var newObject = new GameObject();
            newObject.name = _model.name;
            var itemVisualInstance = newObject.AddComponent<DeckItemVisual>();
            var modelInstance = Instantiate(_model, newObject.transform);
            if (newObject == null)
            {
                return null;
            }

            if (_isBoxCollider)
            {
                modelInstance.AddComponent<BoxCollider>();
            }
            else
            {
                modelInstance.AddComponent<CapsuleCollider>();
            }

            var mat = Resources.Load<Material>("AtlasMaterial");
            modelInstance.GetComponent<MeshRenderer>().material = mat;
            newObject.transform.localScale = Vector3.one;

            itemVisualInstance.OnValidate();
            return newObject;
        }

        private void AddTagToEnd()
        {
            _tags.Add(DeckActionTag.Invalid);
        }

        private void RemoveTagFromEnd()
        {
            if (_tags.Count == 0)
            {
                return;
            }

            _tags.RemoveAt(_tags.Count - 1);
        }
    }
}