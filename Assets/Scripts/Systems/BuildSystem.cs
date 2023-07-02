using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    using Components;
    using Extensions;
    using TorbuTils.Giraphe;

    internal class BuildSystem : MonoBehaviour
    {
        public static BuildSystem Instance { get; private set; }

        public event Action<Vector2Int, GameObject> JustBuilt;

        [SerializeField] private List<GameObject> towerPrefabs = new();
        private Dictionary<Vector2Int, GameObject> towers = new();
        private HashSet<Vector2Int> loadedLocs = new();
        private HashSet<Vector2Int> emptylocs = new();
        private HashSet<Vector2Int> litlocs = new();
        private HashSet<Vector2Int> buildablelocs = new();

        public Dictionary<Vector2Int, GameObject> Towers => towers;
        private void Awake()
        {
            Instance = this;
        }
        private void OnEnable()
        {
            ChunkLoader.Instance.LoadedDelta += LoadDelta;
            LightGraph.Instance.Updated += LightUpdated;
            LoadDelta(ChunkLoader.Instance.CopyLoaded());
        }
        private void OnDisable()
        {
            ChunkLoader.Instance.LoadedDelta -= LoadDelta;
            LightGraph.Instance.Updated -= LightUpdated;
        }
        public ICollection<Vector2Int> CopyBuildableLocs() => buildablelocs.Copy();
        public bool CanBuild(Vector2Int loc) => buildablelocs.Contains(loc);
        public void Build(string towerName, Vector2Int loc, float rot)
        {
            GameObject prefab;
            if (towerName == null)
                prefab = towerPrefabs[UnityEngine.Random.Range(0, towerPrefabs.Count)];
            else
                prefab = towerPrefabs.Find(match => match.name == towerName);

            if (!emptylocs.Contains(loc)) return;
            Room room = LocTracking.Instance.GetRoom(loc);

            GameObject go = PoolSystem.Depool(prefab);
            go.transform.position = Vector2.zero;
            go.transform.SetParent(room.transform, false);
            go.transform.rotation = Quaternion.Euler(0f, 0f, rot);
            Orientation orientation = go.GetComponent<Orientation>();
            if (orientation != null)
                orientation.Local = rot - room.transform.eulerAngles.z;
            emptylocs.Remove(loc);
            towers.Add(loc, go);
            RefreshBuildable();
            JustBuilt?.Invoke(loc, go);
        }
        public ICollection<string> CopyTowerNames()
        {
            List<string> names = new();
            towerPrefabs.ForEach(prefab => names.Add(prefab.name));
            return names;
        }
        private void LoadDelta(Dictionary<Vector2Int, GameObject> loaded)
        {
            foreach (var loc in loaded.Keys)
            {
                loadedLocs.Add(loc);
                emptylocs.Add(loc);  // Change if implementing unload
            }
            RefreshBuildable();
        }
        private void LightUpdated()
        {
            litlocs = new();
            foreach (var loc in LightGraph.Instance.Graph.CopyNodes())
            {
                object obj = LightGraph.Instance.Graph
                    .GetSatellite(loc, Settings.CostSatellite);
                if (obj == null) continue;
                int darkness = (int)obj;
                if (darkness > 1) continue;
                litlocs.Add(loc);
            }
            RefreshBuildable();
        }
        private void RefreshBuildable()
        {
            buildablelocs = new();
            foreach (var loc in emptylocs)
                buildablelocs.Add(loc);
            buildablelocs.IntersectWith(litlocs);
        }
    }
}
