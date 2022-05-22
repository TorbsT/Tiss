using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindBomb : MonoBehaviour
{
    private float life;
    [SerializeField] Transform rangeVisualizer;
    [SerializeField] private float range;
    [SerializeField] private float speed;
    [SerializeField] private float duration;
    [SerializeField] private float lifetime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        life = 0f;
        rangeVisualizer.transform.localScale = Vector3.one * range/0.15f;
    }

    // Update is called once per frame
    void Update()
    {
        life += Time.deltaTime;
        if (life >= lifetime)
        {
            // Explode
            RewindSystem.Instance.StartRewind(transform.position, range, speed, duration);
            GetComponent<Destroyable>().Destroy();
        }
    }
}
