using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems.Extensions
{
    using Components;
    internal static class Vector2IntExtensions
    {
        internal static ICollection<Vector2Int> Neighbours(this Vector2Int origin)
        {
            var result = new List<Vector2Int>
            {
                origin + Vector2Int.up,
                origin + Vector2Int.right,
                origin + Vector2Int.down,
                origin + Vector2Int.left
            };
            return result;
        }
        internal static Room GetRoom(this Vector2Int loc)
            => LocTracking.Instance.GetRoom(loc);
    }
}
