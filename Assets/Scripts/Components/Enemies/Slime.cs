using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Components.Enemies
{
    [RequireComponent(typeof(Enemy), typeof(Locomotion), typeof(Rigidbody2D))]
    public class Slime : MonoBehaviour
    {
        [field: SerializeField] public List<Sprite> Sprites
        { get; set; } = new();
        public Enemy CachedEnemy { get; private set; }
        public Locomotion CachedLocomotion { get; private set; }
        public Rigidbody2D CachedRb { get; private set; }

        private void Awake()
        {
            GetComponent<SpriteRenderer>().sprite = 
                Sprites[Random.Range(0, Sprites.Count)];
            CachedEnemy = GetComponent<Enemy>();
            CachedLocomotion = GetComponent<Locomotion>();
            CachedRb = GetComponent<Rigidbody2D>();
        }
    }
}