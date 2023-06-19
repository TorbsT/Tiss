using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace Assets.Scripts.Systems
{
    internal class VFXSystem : MonoBehaviour
    {
        private class Effect
        {
            public VisualEffect VFX { get; set; }
            public Transform Follow { get; set; }
            public bool StartLinger { get; set; }
            public float LingerAge { get; set; }
            public float LingerLifetime { get; set; }
        }
        public static VFXSystem Instance { get; private set; }

        [field: SerializeField] public VisualEffect IceBullet { get; private set; }
        [field: SerializeField] public VisualEffect IceImpact { get; private set; }
        [field: SerializeField] public VisualEffect CommonUpgrade { get; private set; }
        [field: SerializeField] public float GlobalLingerLifetime { get; private set; } = 10;

        private Dictionary<VisualEffect, Effect> follows = new();
        private Dictionary<Transform, List<VisualEffect>> enpoolLookup = new();

        public void Play(VisualEffect effectPrefab, Vector2 pos)
        => Play(effectPrefab, null, pos);
        public void Play(VisualEffect effectPrefab, Transform follow)
        => Play(effectPrefab, follow, follow.position);
        private void Play(VisualEffect effectPrefab, Transform follow, Vector2 pos)
        {
            VisualEffect vfx = PoolSystem.Depool
                (effectPrefab.gameObject).GetComponent<VisualEffect>();
            Effect value = new()
            {
                VFX = vfx,
                Follow = follow,
                StartLinger = follow == null,
                LingerAge = 0,
                LingerLifetime = GlobalLingerLifetime,
            };
            follows.Add(vfx, value);
            if (follow != null)
            {
                if (!enpoolLookup.ContainsKey(follow))
                    enpoolLookup.Add(follow, new());
                enpoolLookup[follow].Add(vfx);
            }
            vfx.transform.position = pos;
            vfx.Play();
        }
        private void Awake()
        {
            Instance = this;
        }
        private void OnEnable()
        {
            PoolSystem.Instance.JustEnpooled += Enpooled;
        }
        private void OnDisable()
        {
            PoolSystem.Instance.JustEnpooled -= Enpooled;
        }
        private void LateUpdate()
        {
            HashSet<VisualEffect> despawns = new();
            foreach (var item in follows)
            {
                VisualEffect effect = item.Key;
                Effect value = item.Value;
                Transform follow = value.Follow;
                if (value.StartLinger)
                {
                    if (value.LingerAge >= value.LingerLifetime)
                        despawns.Add(effect);
                    value.LingerAge += Time.deltaTime;
                }
                else if (follow != null)
                {
                    effect.transform.position = follow.position;
                    continue;
                }
            }
            foreach (var item in despawns)
            {
                Transform follow = follows[item].Follow;
                if (follow != null)
                {
                    enpoolLookup[follow].Remove(item);
                    if (enpoolLookup[follow].Count == 0)
                        enpoolLookup.Remove(follow);
                }
                follows.Remove(item);
                PoolSystem.Enpool(item.gameObject);
            }
        }
        private void Enpooled(GameObject go)
        {
            if (!enpoolLookup.ContainsKey(go.transform)) return;
            foreach (var vfx in enpoolLookup[go.transform])
            {
                Effect effect = follows[vfx];
                effect.StartLinger = true;
                vfx.Stop();
            }
        }
    }
}
