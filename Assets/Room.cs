using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int Rotation { get => rotation; set { rotation = value; } }
    [SerializeField, Range(0, 3)] private int rotation;
    [SerializeField, Range(0.01f, 10)] private float animationDuration;

    private Coroutine animRoutine;

    [SerializeField] private float animStartRotation;
    [SerializeField] private float animTargetRotation;
    [SerializeField] private float animTime;
    [SerializeField] private bool animRunning;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartRotationAnimation()
    {
        animTargetRotation = -90f * rotation;

        if (animRunning) return;
        animTime = 0f;
        animStartRotation = transform.rotation.eulerAngles.z;
        animRoutine = StartCoroutine(RotationRoutine());
    }
    private IEnumerator RotationRoutine()
    {
        animRunning = true;
        while (animTime < animationDuration)
        {
            animTime += Time.deltaTime;
            float animProgress = animTime / animationDuration;
            float rotationValue = animStartRotation + animProgress * (animTargetRotation - animStartRotation);
            transform.rotation = Quaternion.Euler(0f, 0f, rotationValue);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, animTargetRotation);
        animRunning = false;
    }

}
