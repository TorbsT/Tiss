using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlacer : MonoBehaviour
{
    public GameObject PrefabToPreview { get => prefabToPreview; set { prefabToPreview = value; } }

    [SerializeField] private float range;
    [SerializeField] private GameObject prefabToPreview;
    [SerializeField] private GameObject previewGO;

    private Vector2 pos;
    private GameObject oldPrefab;
    
    void Update()
    {
        if (prefabToPreview != oldPrefab)
        {
            if (previewGO != null) previewGO.GetComponent<Destroyable>().Destroy();
            previewGO = null;
            if (prefabToPreview != null)
            {
                if (prefabToPreview.GetComponent<Placeable>() == null) Debug.LogWarning(prefabToPreview + " isn't really placeable");


                previewGO = EzPools.Instance.Depool(prefabToPreview);
                Placeable placeable = previewGO.GetComponent<Placeable>();
                placeable.Active = true;
                Drop drop = previewGO.GetComponent<Drop>();
                if (drop != null) drop.Active = false;
                Interactable interactable = previewGO.GetComponent<Interactable>();
                if (interactable != null) interactable.Active = false;

            }
            oldPrefab = prefabToPreview;
        }

        if (previewGO != null)
        {
            Vector2 mousePos = UI.Instance.RectCalculator.ScreenPointToWorld(Input.mousePosition);
            Vector2 position = transform.position;
            if ((mousePos-position).magnitude < range)
            {
                pos = mousePos;
                previewGO.transform.position = pos;
            }
        }
    }
    public void Place()
    {
        if (previewGO == null) return;
        GameObject go = EzPools.Instance.Depool(prefabToPreview);
        go.transform.position = pos;
        Drop drop = go.GetComponent<Drop>();
        if (drop != null) drop.Active = false;
        Placeable placeable = go.GetComponent<Placeable>();
        if (placeable != null) placeable.Active = false;
    }
}
