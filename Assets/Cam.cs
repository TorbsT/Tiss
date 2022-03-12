using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform following;
    private bool searching;
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
    }
}
