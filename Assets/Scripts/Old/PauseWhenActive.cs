using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseWhenActive : MonoBehaviour
{
    private void OnEnable()
    {
        PauseSystem.Instance.Pause();
    }
    private void OnDisable()
    {
        PauseSystem.Instance.Resume();
    }
}
