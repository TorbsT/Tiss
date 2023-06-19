using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components.Towers
{
    using ScriptableObjects;
    public class Look : MonoBehaviour
    {
        [field: SerializeField] public LookObject Object { get; set; }
        [field: SerializeField] public Vector2 Target { get; set; }
        public Orientation CachedOrientation { get; private set; }
        public bool LookingDirectly { get; set; }

        private void Awake()
        {
            if (CachedOrientation == null)
                CachedOrientation = GetComponent<Orientation>();
        }
    }
}
