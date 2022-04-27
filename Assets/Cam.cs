using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform following;
    private new Camera camera;
    private bool searching;
    [SerializeField] private AnimationCurve scrollCurve;
    [SerializeField, Range(0f, 10f)] private float scrollSpeed;
    [SerializeField, Range(5f, 100f)] private float minZoom;
    [SerializeField, Range(1f, 5f)] private float maxZoom;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void Start()
    {
        Search();
    }

    private void Search()
    {
        searching = true;
        Player p = FindObjectOfType<Player>();
        if (p == null) Invoke(nameof(Search), 1f);
        following = p.transform;
        searching = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (following == null)
        {
            if (!searching) Search();
            return;
        }
        transform.position = following.position + new Vector3(0f, 0f, -10f);

        float scroll = -Input.mouseScrollDelta.y;
        scroll *= scrollSpeed;
        scroll *= scrollCurve.Evaluate((camera.orthographicSize-maxZoom) / (minZoom - maxZoom));
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + scroll, maxZoom, minZoom);
    }
}
