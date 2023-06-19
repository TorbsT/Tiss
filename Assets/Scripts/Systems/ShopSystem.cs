using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    using Components;
    internal class ShopSystem : MonoBehaviour
    {
        public enum State
        {
            None,
            Building
        }
        public static ShopSystem Instance { get; private set; }

        [field: SerializeField] public GameObject PreviewPrefab { get; private set; }
        public State CurrentState { get; private set; }
        private float rot = 0f;
        private ShopItem selectedItem;
        private GameObject hoverRoom;
        private GameObject preview;

        public void Select(GameObject go)
        {
            if (CurrentState == State.None)
            {
                if (go == null) return;
                ShopItem item = go.GetComponent<ShopItem>();
                if (item == null) return;
                SetSelected(item);
                rot = 0f;
            } else //if (CurrentState == State.Building)
            {
                if (go == null) return;
                ShopItem item = go.GetComponent<ShopItem>();
                if (item != null)
                {
                    SetSelected(item);
                    return;
                }
                if (go.name == "TowerGreen")
                {
                    Vector2Int loc = go.transform.parent.GetComponent<Room>().Loc;
                    string command = $"tower loc:{loc.x},{loc.y} tower:{selectedItem.Prefab.name} rot:{rot}";
                    ConsoleActor.Instance.Execute(command);
                    SetSelected(null);
                    return;
                }
            }
        }
        public void Hover(GameObject go)
        {
            // Check if you're hovering a room
            if (go == null) hoverRoom = null;
            else if (CurrentState == State.None) hoverRoom = null;
            else hoverRoom = go.name == "TowerGreen"
                ? go.transform.parent.gameObject : null;
        }

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            preview = Instantiate(PreviewPrefab);
            SetSelected(null);
        }
        private void Update()
        {
            if (CurrentState == State.None) return;
            Vector2 previewPos = hoverRoom != null ?
                hoverRoom.transform.position : Camera.main.ScreenToWorldPoint(Input.mousePosition);
            preview.transform.position = previewPos;
            if (Input.GetKeyDown(KeyCode.R))
            {
                rot -= 90f;
                rot %= 360;
                preview.transform.rotation = Quaternion.Euler(new(0f, 0f, rot));
            }
        }
        private void SetSelected(ShopItem item)
        {
            selectedItem = item;
            if (item == null)
            {
                CurrentState = State.None;
                preview.SetActive(false);
                foreach (var chunk in ChunkLoader.Instance.CopyLoaded())
                {
                    Room room = chunk.Value.GetComponent<Room>();
                    room.TowerGreen.SetActive(false);
                    room.TowerRed.SetActive(false);
                }
            }   
            else
            {
                CurrentState = State.Building;
                preview.SetActive(true);
                preview.GetComponent<SpriteRenderer>().sprite = item.BuildPreviewSprite;
                foreach (var chunk in ChunkLoader.Instance.CopyLoaded())
                {
                    Room room = chunk.Value.GetComponent<Room>();
                    bool allowBuild = BuildSystem.Instance.IsEmpty(chunk.Key);
                    room.TowerGreen.SetActive(allowBuild);
                    room.TowerRed.SetActive(!allowBuild);
                }
                preview.transform.rotation = Quaternion.Euler(new(0f, 0f, rot));
            }
        }
    }
}
