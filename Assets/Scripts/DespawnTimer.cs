using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnTimer : MonoBehaviour
{
    [Header("CONFIG")]
    [SerializeField] private float despawnTime;

    [Header("DEBUG")]
    [SerializeField] private float life;

    // Cached for performance
    private Destroyable destroyable;

    private void Awake()
    {
        destroyable = GetComponent<Destroyable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        life = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        life += Time.deltaTime;
        if (life > despawnTime)
        {
            if (destroyable == null) Debug.LogWarning(gameObject + " needs a Destroyable component");
            else destroyable.Destroy();
        }
    }
}
