using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    internal class ChunkCalculator : MonoBehaviour
    {
        private enum GizmosMode
        {
            Always,
            Selected,
            Never
        }

        public event Action<ICollection<Vector2Int>, ICollection<Vector2Int>>
            DeltaCalculated;
        public static ChunkCalculator Instance { get; private set; }

        [field: SerializeField, Range(1f, 1000f)]
        public float fpsPriority { get; set; } = 1f;
        [field: SerializeField] public Vector2Int CenterPos { get; set; }
        [field: SerializeField, Range(-1, 100)]
        public int LoadRange { get; set; } = 10;
        [field: SerializeField, Range(-1, 100)]
        public int UnloadRange { get; set; } = 15;
        [SerializeField] private GizmosMode loadedGizmos = GizmosMode.Always;

        [Header("DEBUG")]
        private bool running;
        private float allowedDelay;
        private HashSet<Vector2Int> loaded = new();
        private float lastTime;

        private void Awake()
        {
            Debug.Log("gaa");
            Instance = this;
        }
        void OnDrawGizmos()
        {
            DrawGizmos(GizmosMode.Always);
        }
        void OnDrawGizmosSelected()
        {
            DrawGizmos(GizmosMode.Selected);
        }
        void DrawGizmos(GizmosMode mode)
        {
            if (loadedGizmos == mode)
            {
                Gizmos.color = Color.blue;
                foreach (var loc in loaded)
                {
                    Gizmos.DrawCube((Vector2)loc * 1f, Vector3.one * 0.8f);
                }
            }
        }
        void Update()
        {
            if (!running)
            {
                StartCoroutine(Routine());
            }
        }
        public ICollection<Vector2Int> GetLoaded()
        {
            HashSet<Vector2Int> result = new();
            foreach (var loc in loaded)
                result.Add(loc);
            return result;
        }
        IEnumerator Routine()
        {
            running = true;
            Vector2Int centerPos = CenterPos;

            HashSet<Vector2Int> load = new();
            HashSet<Vector2Int> unload = new();

            if (UnloadRange >= 0)
            foreach (Vector2Int loc in loaded)
            {
                if (TimeToStop()) yield return null;
                if (((Vector2)loc-centerPos).magnitude > UnloadRange)
                {
                    unload.Add(loc);
                }
            }
            for (int x = centerPos.x-LoadRange; x < centerPos.x+LoadRange+1; x++)
            {
                for (int z = centerPos.y-LoadRange; z < centerPos.y+LoadRange+1; z++)
                {
                    if (TimeToStop()) yield return null;
                    Vector2Int v = new(x, z);
                    if (loaded.Contains(v)) continue;
                    if (((Vector2)v-centerPos).magnitude <= LoadRange)
                    {
                        load.Add(v);
                    }
                }
            }

            foreach (Vector2Int loc in unload)
                loaded.Remove(loc);
            foreach (Vector2Int loc in load)
                loaded.Add(loc);

            if (load.Count > 0 || unload.Count > 0)
                DeltaCalculated?.Invoke(load, unload);
            running = false;
        }
        void OnEnable()
        {
            
        }
        void OnDisable()
        {
            StopAllCoroutines();
            running = false;
        }
        bool TimeToStop()
        {
            float timeNow = Time.realtimeSinceStartup;
            if (timeNow - lastTime > allowedDelay)
            {
                lastTime = timeNow;
                return true;
            }
            return false;
        }
        void OnValidate()
        {
            allowedDelay = 1f / fpsPriority;
        }
    }
}
