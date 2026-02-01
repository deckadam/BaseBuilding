using System;
using System.Collections.Generic;
using System.Linq;
using Data.Buildable.Data;
using Data.Buildable.Data.Parameter;
using UnityEditor;

namespace Utility.Resource
{
    public static class DeckResourceLocator
    {
        public static IEnumerable<Type> GetAllParameterTypes()
        {
            return typeof(DeckBuildableParameter).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(DeckBuildableParameter)) && !t.IsAbstract);
        }

        public static IEnumerable<DeckBuildableCategory> GetAllCategories()
        {
            return AssetDatabase.FindAssets($"t:{nameof(DeckBuildableCategory)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<DeckBuildableCategory>)
                .OrderBy(x => x.name);
        }
    }
}