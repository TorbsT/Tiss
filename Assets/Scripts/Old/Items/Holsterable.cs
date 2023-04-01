using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Holsterable : MonoBehaviour
{
    public void Holster(Hands hands)
    {
        transform.SetParent(hands.GunSocket);
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector2.zero;
    }
}
