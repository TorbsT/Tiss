using Assets.Scripts.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems.Towers
{
    internal class BladeSystem : QuickSystem<Blade>
    {
        protected override void Tick(ICollection<Blade> blades)
        {
            foreach (var blade in blades)
            {
                foreach (var collided in blade.CollidedSinceLastCheck)
                {
                    EnemySystem.Instance.Damage(collided, blade.Damage);
                }
                blade.CollidedSinceLastCheck.Clear();
            }
        }
    }
}
