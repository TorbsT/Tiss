using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterSystem : MonoBehaviour
{
    void Start()
    {
        EventSystem.DeclareEvent(Event.MasterStarted);
    }
}
