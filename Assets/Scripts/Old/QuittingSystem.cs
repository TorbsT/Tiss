using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuittingSystem : MonoBehaviour
{
    public static bool Quitting;
    private void OnApplicationQuit()
    {
        Quitting = true;
    }
}
