using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
    public static class VectorExtensions
    {
        public static Vector2 Add(this Vector3 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
        public static Vector2 Add(this Vector2 a, Vector3 b) => new Vector2(a.x + b.x, a.y + b.y);
        public static Vector2 Subtract(this Vector3 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
        public static Vector2 Subtract(this Vector2 a, Vector3 b) => new Vector2(a.x - b.x, a.y - b.y);
    }
}

