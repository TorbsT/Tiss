using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzPools;

public class Shotgun : MonoBehaviour, IHolsterable
{
    public void Holster(Hands hands)
    {
        transform.SetParent(hands.GunSocket);
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector2.zero;
    }

    public void Unholster()
    {
        Manager.Instance.Enpool(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}