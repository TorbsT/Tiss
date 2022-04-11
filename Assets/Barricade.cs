using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour, IHPListener
{
    [SerializeField] private float maxHP;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<HP>().AddListener(this);
    }

    void OnEnable()
    {
        GetComponent<HP>().Set(maxHP);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void NewHP(float oldHP, float newHP)
    {
        if (newHP <= 0f)
        {
            GetComponent<Destroyable>().Destroy();
        }
    }
}
