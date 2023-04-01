using System.Collections.Generic;
using Deck.Item;
using Deck.Utility.Logger;
using UnityEditor;
using UnityEngine;

namespace Deck.Data.Item.Editor
{
    public class DeckItemCreator : EditorWindow
    {
        private Sprite _icon;
        private int _amount;
        private string _name;
        private bool _holdable;
        private bool _isBoxCollider;
        private string _animationName;
        private List<string> _tags = new();
        private GameObject _itemVisual;

        [MenuItem("Deck/Item Creator")]
        private static void ShowWindow()
        {
            var window = GetWindow<DeckItemCreator>();
            window.titleContent = new GUIContent("Deck Item Creator");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(25);
            _name = EditorGUILayout.TextField("Name: ", _name);
            EditorGUILayout.Space(10);
            _icon = (Sprite)EditorGUILayout.ObjectField("Icon: ", _icon, typeof(Sprite), false);
            EditorGUILayout.Space(10);
            _amount = EditorGUILayout.IntField("Amount: ", _amount);
            _holdable = EditorGUILayout.Toggle("Is holdable: ", _holdable);
            _isBoxCollider = EditorGUILayout.Toggle("Is box collider: ", _holdable);
            _animationName = EditorGUILayout.TextField("Animation name", _animationName);

            _itemVisual = (GameObject)EditorGUILayout.ObjectField("Item visual: ", _itemVisual, typeof(GameObject), false);

            for (var i = 0; i < _tags.Count; i++)
            {
                _tags[i] = EditorGUILayout.TextField("Item visual: ", _tags[i]);
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
            var prefabName = "Assets/Prefabs/ItemVisualPrefabs/DeckItemVisual" + _name + ".prefab";
            var assetName = "Assets/Data/Items/DeckItem" + _name + ".asset";

            ClearPrefabIfExists(prefabName);
            ClearScriptableIfExists(assetName);

            var instance = InitializeItemInstance();
            if (instance)
            {
                return;
            }

            var prefab = PrefabUtility.SaveAsPrefabAsset(instance, prefabName).GetComponent<DeckItemVisual>();
            var newScriptableObject = DeckDataItem.Create(_name, _icon, _amount, prefab, _holdable, _animationName, _tags.ToArray());
            
            AssetDatabase.CreateAsset(newScriptableObject, assetName);

            UpdatePrefabData(prefab, newScriptableObject, prefabName, instance);

            DeckLogger.Success(_name + "  item and prefab succesfully created");
            
            UpdateEditor();
        }

        private static void UpdateEditor()
        {
            AssetDatabase.Refresh();
            EditorApplication.RepaintProjectWindow();
        }

        private static void UpdatePrefabData(DeckItemVisual prefab, DeckDataItem newScriptableObject, string prefabName, GameObject instance)
        {
            prefab.SetItem(newScriptableObject);
            var copy = Instantiate(prefab);
            prefab.SetItem(newScriptableObject);
            PrefabUtility.SaveAsPrefabAssetAndConnect(copy.gameObject, prefabName, InteractionMode.UserAction);

            ClearLeftoverObjects(copy, instance);
        }

        private static void ClearLeftoverObjects(DeckItemVisual copy, GameObject instance)
        {
            DestroyImmediate(copy.gameObject);
            DestroyImmediate(instance);
        }

        private void ClearScriptableIfExists(string assetName)
        {
            var existingObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetName);
            if (existingObject != null)
            {
                AssetDatabase.DeleteAsset(assetName);
            }
        }

        private GameObject InitializeItemInstance()
        {
            var instance = Instantiate(_itemVisual, Vector3.zero, Quaternion.identity);
            if (instance == null)
            {
                return null;
            }

            if (_isBoxCollider)
            {
                instance.AddComponent<BoxCollider>();
            }
            else
            {
                instance.AddComponent<CapsuleCollider>();
            }

            instance.AddComponent<DeckItemVisual>();
            var rb = instance.GetComponent<Rigidbody>();
            rb.isKinematic = true;

            var mat = Resources.Load<Material>("AtlasMaterial");
            instance.GetComponent<MeshRenderer>().material = mat;
            instance.transform.localScale = Vector3.one;
            return instance;
        }

        private void ClearPrefabIfExists(string prefabName)
        {
            var existingPrefab = AssetDatabase.LoadAssetAtPath<ScriptableObject>(prefabName);
            if (existingPrefab != null)
            {
                AssetDatabase.DeleteAsset(prefabName);
                DeckLogger.Inform("Replaced item visual prefab");
            }
        }

        private void AddTagToEnd()
        {
            _tags.Add("");
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