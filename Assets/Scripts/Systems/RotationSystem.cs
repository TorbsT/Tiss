using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    using TorbuTils.Anime;
    using Components;
    internal class RotationSystem : MonoBehaviour
    {
        public static RotationSystem Instance { get; private set; }
        [field: SerializeField, Range(0f, 5f)]
        public float RotationTime { get; set; } = 1f;
        [field: SerializeField]
        public AnimationCurve RotationCurve { get; set; }
            = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public event Action<Vector2Int, GameObject> RotationChanged;

        private Dictionary<GameObject, Anim<float>> animsInProgress = new();
        private void Awake()
        {
            Instance = this;
        }
        public void AddRotation(GameObject go, int add, bool immediate)
        {
            SetRotation(go, go.GetComponent<Room>().Rotation+add, immediate);
        }
        public void SetRotation(GameObject go, int set, bool immediate)
        {
            Room r = go.GetComponent<Room>();
            r.Rotation = set % 4;
            Transform t = go.transform;
            float animTime = RotationTime;
            if (immediate) animTime = 0f;
            
            Anim<float> rotationAnim = new()
            {
                StartValue = t.rotation.eulerAngles.z,
                EndValue = -90f * r.Rotation,
                Action = (value) => t.rotation = 
                    Quaternion.identity* Quaternion.Euler(0f, 0f, value),
                Curve = RotationCurve,
                Duration = animTime
            };
            if (animsInProgress.ContainsKey(go))
            {
                if (animsInProgress[go].Progress < 1f)
                    animsInProgress[go].Stop();
                animsInProgress.Remove(go);
            }
            animsInProgress.Add(go, rotationAnim);
            rotationAnim.Start();
            RotationChanged?.Invoke(r.Loc, go);
        }
    }
}
