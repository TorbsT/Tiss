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
    internal class DisturbanceSystem : MonoBehaviour
    {
        public static DisturbanceSystem Instance { get; private set; }

        public event Action StateChanged;

        [SerializeField] private Gradient disturbanceGradient;
        [SerializeField, Range(-1f, 1f)] private float disturbancePerSecond;
        private readonly HashSet<Room> rooms = new();

        private bool changed;

        public void Heat(Vector2Int loc, float amount)
        {
            Room room = loc.GetRoom();
            if (room == null)
            {
                Debug.LogWarning("Can't heat up, room is null");
                return;
            }
            room.Disturbance += amount;
            if (room.Disturbance >= 1f)
            {
                room.Disturbance -= 1f;
                RotationSystem.Instance.AddRotation(room.gameObject, 1, false);
            }
            if (room.Disturbance <= 0f) room.Disturbance = 0f;
            room.GetComponent<SpriteRenderer>().color =
                disturbanceGradient.Evaluate(room.Disturbance);
            
            if (amount != 0f)
                changed = true;
        }
        public ICollection<Room> CopyRooms()
        {
            HashSet<Room> rooms = new();
            foreach (Room room in this.rooms)
                rooms.Add(room);
            return rooms;
        }
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            ChunkLoader.Instance.LoadedDelta += JustLoaded;
            JustLoaded(ChunkLoader.Instance.CopyLoaded());
        }
        private void Update()
        {
            foreach (Room room in rooms)
            {
                Heat(room.Loc, disturbancePerSecond*Time.deltaTime);
            }
            if (changed)
            {
                StateChanged?.Invoke();
                changed = false;
            }
        }
        void JustLoaded(Dictionary<Vector2Int, GameObject> delta)
        {
            foreach (GameObject go in delta.Values)
            {
                int rot = UnityEngine.Random.Range(0, 4);
                RotationSystem.Instance.SetRotation(go, rot, true);
                rooms.Add(go.GetComponent<Room>());
            }
        }
    }
}
