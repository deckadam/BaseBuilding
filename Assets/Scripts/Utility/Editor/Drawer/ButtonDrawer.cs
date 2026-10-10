using System.Reflection;
using UnityEditor;
using UnityEngine;
using Utility.Attribute;

namespace Utility.Editor.Drawer
{
    [CustomEditor(typeof(Object), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class ButtonDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var methods = target.GetType().GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<ButtonAttribute>() != null)
                {
                    if (GUILayout.Button(method.Name))
                        method.Invoke(target, null);
                }
            }
        }
    }
}