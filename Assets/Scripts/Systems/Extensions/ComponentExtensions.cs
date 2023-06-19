using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems.Extensions
{
    internal static class ComponentExtensions
    {
        internal static Vector2Int GetLoc(this Component component)
        {
            Vector2 pos = component.transform.position;
            Vector2Int loc = new(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
            return loc;
        }
    }
}
