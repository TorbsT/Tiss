using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class Chest : MonoBehaviour
    {
        [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
        [field: SerializeField] public Sprite OpenSprite { get; set; }
        [field: SerializeField] public Sprite ClosedSprite { get; set; }

        private void OnValidate()
        {
            if (SpriteRenderer == null)
                SpriteRenderer = GetComponent<SpriteRenderer>();
            if (SpriteRenderer != null && ClosedSprite == null)
                ClosedSprite = SpriteRenderer.sprite;
        }
    }
}
