using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Room : MonoBehaviour
{
    public Vector2Int Id { get => id; set { id = value; } }
    public int NewRotation { get => newRotation; set { newRotation = value; } }
    public int Rotation => rotation;

    [SerializeField] private Vector2Int id;
    [SerializeField] private int newRotation;
    [SerializeField, Range(0.01f, 10)] private float animationDuration;
    [SerializeField] private AnimationCurve rotationCurve;

    private Coroutine animRoutine;

    private int rotation;
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

    void OnEnable()
    {
        
    }

    public void StartRotationAnimation()
    {
        if (animRunning) return;
        animRoutine = StartCoroutine(RotationRoutine());
    }
    private IEnumerator RotationRoutine()
    {
        animRunning = true;

        int rotations = newRotation - rotation;
        animTime = 0f;
        animStartRotation = -90f * rotation;
        animTargetRotation = -90f * newRotation;

        if (rotations != 0)
        {
            while (animTime < animationDuration)
            {
                animTime += Time.deltaTime;
                float animProgress = rotationCurve.Evaluate(animTime / animationDuration);
                float rotationValue = animStartRotation + animProgress * (animTargetRotation - animStartRotation);
                transform.rotation = Quaternion.Euler(0f, 0f, rotationValue);
                yield return null;
            }
            transform.rotation = Quaternion.Euler(0f, 0f, animTargetRotation);
        }

        rotation = newRotation;
        animRunning = false;
    }

}
