using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems.Towers
{
    using Assets.Scripts.Components;
    using Assets.Scripts.Components.Towers;
    internal class LaserEyeSystem : TowerSystem<LaserEye>
    {
        protected override void JustEnabled()
        {
            SetFloat("dmg", 400f);
            SetFloat("heat", 0.006f);
            SetFloat("fov", 40f);
        }
        protected override void Tick(ICollection<LaserEye> turrets)
        {
            foreach (var eye in turrets)
            {
                eye.CachedOrientation.FOV = GetFloat("fov").Value;
                eye.CachedLaser.DPS = GetFloat("dmg").Value;
                Heat(eye, GetFloat("heat").Value * Time.deltaTime);
            }
        }
    }
}
