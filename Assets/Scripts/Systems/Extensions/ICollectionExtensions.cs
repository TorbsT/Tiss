using UnityEngine;
using System.Collections.Generic;


namespace Assets.Scripts.Systems.Extensions
{
    internal static class ICollectionExtensions
    {
        /// <summary>
        /// Chooses a random element, or returns default if count == 0.
        /// </summary>
        internal static T GetRandom<T>(this ICollection<T> collection)
        {
            int count = collection.Count;
            if (count == 0)
                return default;
            int selected = Random.Range(0, count);
            int i = 0;
            foreach (var item in collection)
            {
                if (i == selected)
                    return item;
                i++;
            }
            Debug.LogError("No weieieeee");
            return default;
        }
        internal static ICollection<T> Copy<T>(this ICollection<T> collection)
        {
            List<T> result = new();
            foreach (var item in collection)
                result.Add(item);
            return result;
        }
    }
}
