using Assets.Scripts.Components;
using Assets.Scripts.Components.Towers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems.Towers
{
    public class BladesawSystem : TowerSystem<Bladesaw>
    {
        [field: SerializeField] public Blade BladePrefab { get; private set; }
        public float MaxCollisionDamage => GetFloat("dmg").Value;
        public int BladeCount => GetInt("count").Value;
        public float MaxRPS => GetFloat("speed").Value;
        public float AccelerationTime => GetFloat("spinup").Value;
        public float DecelerationTime => GetFloat("spindown").Value;
        public float HeatGeneration => GetFloat("heat").Value;


        public void OnCollision(Impactor source, ICollection<Collider2D> collisions)
        {
            Bladesaw bladesaw = source.GetComponentInParent<Bladesaw>();
            float dmg = bladesaw.CurrentRPS / MaxRPS;
            foreach (Collider2D collision in collisions)
            {
                ((EnemySystem)EnemySystem.Instance).Damage(collision, dmg);
            }
        }

        protected override void Tick(ICollection<Bladesaw> towers)
        {
            foreach (var tower in towers)
            {
                bool active = tower.CachedAcquirer.HasTarget;
                tower.Spin = active;
                if (active)
                    Heat(tower, HeatGeneration * Time.deltaTime);
            }
        }
        protected override void FixedTick(ICollection<Bladesaw> towers)
        {
            foreach (var tower in towers)
            {
                bool spin = tower.Spin;
                Rigidbody2D rb = tower.Rigidbody;
                float vel = rb.angularVelocity;
                if (tower.SpinWithClock) vel = -vel;

                float MaxAngularVelocity = MaxRPS * 360f;
                if (spin)
                    vel += MaxAngularVelocity / AccelerationTime * Time.fixedDeltaTime;
                else
                    vel -= MaxAngularVelocity / DecelerationTime * Time.fixedDeltaTime;

                vel = Mathf.Clamp(vel, 0f, MaxAngularVelocity);
                tower.CurrentRPS = vel / 360f;
                foreach (var blade in tower.Blades)
                {
                    blade.Damage = MaxCollisionDamage * (vel / MaxAngularVelocity);
                }
                if (tower.SpinWithClock) vel = -vel;
                rb.angularVelocity = vel;
            }
        }
        protected override void JustEnabled()
        {
            SetFloat("dmg", 20f);
            SetInt("count", 4);
            SetFloat("speed", 1f);
            SetFloat("spinup", 2f);
            SetFloat("spindown", 4f);
            SetFloat("heat", 0.02f);
        }
        protected override void JustDepooledComponent(Bladesaw component)
        {
            RefreshBlades(component);
        }
        protected override void UpgradesChanged()
        {
            foreach (var bladesaw in CopyComponents())
                RefreshBlades(bladesaw);
        }
        private void RefreshBlades(Bladesaw component)
        {
            foreach (var blade in component.GetComponentsInChildren<Blade>())
                PoolSystem.Enpool(blade.gameObject);

            List<Blade> blades = new();
            int count = BladeCount;
            for (int i = 0; i < count; i++)
            {
                Blade blade = PoolSystem.Depool(BladePrefab.gameObject).GetComponent<Blade>();
                blade.transform.SetParent(component.transform);
                blade.transform.localPosition = Vector2.zero;
                float angle = 360f * i / count;
                blade.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
                blades.Add(blade);
            }
            component.Blades = blades;
        }
    }
}
