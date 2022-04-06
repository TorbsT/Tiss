using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlacer : MonoBehaviour
{
    public GameObject PrefabToPreview { get => prefabToPreview; set { prefabToPreview = value; } }

    [SerializeField] private float range;
    [SerializeField] private GameObject prefabToPreview;
    [SerializeField] private Placeable preview;

    private Vector2 pos;
    private GameObject oldPrefab;
    
    void Update()
    {
        if (prefabToPreview != oldPrefab)
        {
            if (preview != null) preview.GetComponent<Destroyable>().Destroy();
            preview = null;
            if (prefabToPreview != null)
            {
                if (prefabToPreview.GetComponent<Placeable>() == null) Debug.LogWarning(prefabToPreview + " isn't really placeable");


                preview = EzPools.Instance.Depool(prefabToPreview).GetComponent<Placeable>();
                preview.Active = true;
                preview.Valid = false;
                Drop drop = preview.GetComponent<Drop>();
                if (drop != null) drop.Active = false;
                Collider2D coll = preview.GetComponent<Collider2D>();
                if (coll != null) coll.enabled = false;
                Interactable interactable = preview.GetComponent<Interactable>();
                if (interactable != null) interactable.Active = false;

            }
            oldPrefab = prefabToPreview;
        }

        if (preview != null)
        {
            Vector2 mousePos = UI.Instance.RectCalculator.ScreenPointToWorld(Input.mousePosition);
            Vector2 position = transform.position;
            Transform t = preview.transform;
            pos = mousePos;
            t.position = pos;
            if ((mousePos-position).magnitude < range)
            {
                preview.Valid = true;
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Quaternion r = t.rotation;
                    r = Quaternion.Euler(0f, 0f, -90f) * r;
                    t.rotation = r;
                }
            } else
            {
                preview.Valid = false;
            }

        }
    }
    public void Place()
    {
        if (preview == null) return;
        if (!preview.Valid) return;
        GameObject go = EzPools.Instance.Depool(prefabToPreview);
        Transform t = go.transform;
        t.position = pos;
        Drop drop = go.GetComponent<Drop>();
        if (drop != null) drop.Active = false;
        Collider2D coll = go.GetComponent<Collider2D>();
        if (coll != null) coll.enabled = true;
        Placeable placeable = go.GetComponent<Placeable>();
        if (placeable != null)
        {
            if (placeable.ParentToRoom)
            {
                Vector2 p = t.position;
                Quaternion rotation = preview.transform.rotation;
                Room r = RoomManager.Instance.PosToRoom(p);
                if (r != null) t.parent = r.transform;
                t.rotation = rotation;
            }
            placeable.Active = false;
        }
    }
}
