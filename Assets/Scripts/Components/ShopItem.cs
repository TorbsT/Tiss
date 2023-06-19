using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    // For the moment only towers
    public class ShopItem : MonoBehaviour
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public Sprite BuildPreviewSprite { get; private set; }
    }
}
