using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
    public static class Vector2IntExtension
    {
        public static Vector2Int[] Neighbours(this Vector2Int middle)
        {
            return new Vector2Int[] {
                middle + Vector2Int.up,
                middle + Vector2Int.right,
                middle + Vector2Int.down,
                middle + Vector2Int.left,
            };
        }
    }
}

