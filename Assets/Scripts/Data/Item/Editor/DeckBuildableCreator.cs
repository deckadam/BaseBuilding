using System;
using System.Collections.Generic;
using Deck.Components;
using Deck.Components.Building;
using Deck.Data.Buildable;
using Deck.Instancing;
using Deck.ItemVisualProviders;
using Deck.UI.Building;
using Deck.UI.Building.BuildingSets;
using Deck.UI.Building.BuildingSets.BuildingMiscellaneous;
using Deck.UI.Building.BuildingSets.BuildingWallsAndDoors;
using Deck.UI.Building.BuildingSets.DeckBuildingBarTable;
using Deck.UI.Building.BuildingSets.DeckBuildingFurniture;
using Deck.Utility;
using UI.Building.BuildMode;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Deck.Data.Item.Editor
{
    public class DeckBuildableCreator : EditorWindow
    {
        private List<DeckActionTag> _tags = new();
        private GameObject _itemVisual;
        private GameObject _model;
        private bool _isBoxCollider = true;
        private Sprite _icon;
        private int _amount;
        private bool _isCellBased;
        private bool _rotatable = true;
        private bool _canBeHangedToWall;
        private bool _isBasicItem = true;
        private string _nameSuffix;
        private DeckBuildableType _selectedType = DeckBuildableType.Error;
        private DeckBuildMode _selectedBuildMode = DeckBuildMode.Free;


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
            _isCellBased = EditorGUILayout.Toggle("Is cell based: ", _isCellBased);
            _nameSuffix = EditorGUILayout.TextField("Name suffix: ", _nameSuffix);
            _model = (GameObject)EditorGUILayout.ObjectField("Item model: ", _model, typeof(GameObject), false);
            _selectedType = (DeckBuildableType)EditorGUILayout.EnumPopup("Buildable type: ", _selectedType);
            _selectedBuildMode = (DeckBuildMode)EditorGUILayout.EnumPopup("Build mode: ", _selectedBuildMode);

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

            DeckBuildingPage page;
            switch (_selectedType)
            {
                case DeckBuildableType.Error:
                    throw new Exception("Dont forget to select the page");
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
                    page = Resources.FindObjectsOfTypeAll<DeckBuildingPageBarTable>()[0];
                    suffix = "BarTable";
                    break;

                default:
                    throw new Exception("Page not found");
            }


            var itemVisualPrefabName = "Assets/Prefabs/ItemVisualPrefabs/DeckItemVisual" + suffix + _nameSuffix + ".prefab";
            var agentPrefabName = "Assets/Prefabs/AgentPrefabs/DeckAgent" + suffix + _nameSuffix + ".prefab";
            var assetName = "Assets/Resources/Data/Items/DeckBuildable" + suffix + _nameSuffix + ".asset";

            var itemVisualInstance = CreateItemVisualInstance();
            var itemVisualPrefab = PrefabUtility.SaveAsPrefabAsset(itemVisualInstance.gameObject, itemVisualPrefabName).GetComponent<DeckItemVisual>();
            itemVisualPrefab.SetNewUniqueId();

            var buildingInstance = CreateBuildingInstance();
            var buildingPrefab = PrefabUtility.SaveAsPrefabAsset(buildingInstance, agentPrefabName).GetComponent<DeckBuilding>();
            buildingPrefab.SetTags(_tags.ToArray());
            buildingPrefab.SetNewUniqueId();

            var buildingData = DeckBuildable.Create(_nameSuffix, _icon, itemVisualPrefab, buildingPrefab, _selectedBuildMode);
            AssetDatabase.CreateAsset(buildingData, assetName);

            buildingPrefab.SetBuildingData(buildingData);

            UpdatePrefabData(itemVisualPrefab, buildingPrefab, buildingData, itemVisualInstance, buildingInstance);

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
                DeckLogger.Warning("Please add item to proper item visual provider!!!");
            }

            var instanceProvider = Resources.FindObjectsOfTypeAll<DeckInstanceProvider>()[0];
            instanceProvider.AddAgent(buildingPrefab);
            instanceProvider.AddItemVisual(itemVisualPrefab);

            page.AddBuildable(buildingData);


            UpdateEditor();

            DeckLogger.Success(_nameSuffix + "  item and prefab successfully created");
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

        private GameObject CreateBuildingInstance()
        {
            var newObject = new GameObject();
            newObject.name = _nameSuffix;

            var agentInstance = newObject.AddComponent<DeckBuilding>();
            agentInstance.OnValidate();

            return newObject;
        }

        private GameObject CreateItemVisualInstance()
        {
            var newObject = new GameObject();
            newObject.name = _nameSuffix;

            var itemVisualInstance = newObject.AddComponent<DeckItemVisual>();
            var modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(_model);
            modelInstance.transform.parent = newObject.transform;
            if (newObject == null)
            {
                return null;
            }

            if (_isBoxCollider)
            {
                modelInstance.AddComponent<BoxCollider>();
                modelInstance.AddComponent<NavMeshObstacle>().carving = true;
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