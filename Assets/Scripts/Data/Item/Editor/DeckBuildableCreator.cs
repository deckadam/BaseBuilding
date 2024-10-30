using System.Collections.Generic;
using Deck.Components;
using Deck.Components.Building;
using Deck.Data.Buildable;
using Deck.ItemVisualProviders;
using Deck.Save.Data;
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
        private string _animationName;
        private bool _isBoxCollider;
        private Sprite _icon;
        private int _amount;
        private bool _rotatable;
        private bool _canBeHangedToWall;

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
            _animationName = EditorGUILayout.TextField("Animation name", _animationName);

            _model = (GameObject)EditorGUILayout.ObjectField("Item model: ", _model, typeof(GameObject), false);

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
            var itemVisualPrefabName = "Assets/Prefabs/ItemVisualPrefabs/DeckItemVisual" + _model.name + ".prefab";
            var agentPrefabName = "Assets/Prefabs/AgentPrefabs/DeckAgent" + _model.name + ".prefab";
            var assetName = "Assets/Resources/Data/Items/DeckItem" + _model.name + ".asset";

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
            
            var visualProvider = Resources.FindObjectsOfTypeAll<DeckItemVisualProviderBasic>()[0];
            visualProvider.AddItemVisual(itemVisualPrefab);

            var instanceProvider = Resources.FindObjectsOfTypeAll<DeckInstanceProvider>()[0];
            instanceProvider.AddAgent(agentPrefab);
            instanceProvider.AddItemVisual(itemVisualPrefab);
            
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
            agentPrefab.SetItemVisual(itemVisualPrefab);

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