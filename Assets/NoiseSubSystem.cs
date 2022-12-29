using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NoiseSubSystem : MonoBehaviour
{
    // SubSystem means there can be multiple instances

    public Dictionary<Vector2Int, float> noises = new();
    public Vector2 origin;
    public float scale = 1.0f;
    [Range(0, 100)] public int previewSize = 10;
    public GizmosMode mode = GizmosMode.Always;


    private void OnDrawGizmos()
    {
        DrawGizmos(GizmosMode.Always);
    }
    private void OnDrawGizmosSelected()
    {
        DrawGizmos(GizmosMode.Selected);
    }
    private void DrawGizmos(GizmosMode mode)
    {
        if (mode == this.mode)
        {
            foreach (Vector2Int loc in noises.Keys)
            {
                float value = noises[loc];
                Gizmos.color = new(value, value, value);
                Gizmos.DrawCube((Vector2)loc * ChunkSystem.Instance.chunkSize, Vector3.one*ChunkSystem.Instance.chunkSize);
            }
        }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        noises = new();
        Generate();
    }
    private void OnDisable()
    {
        noises = new();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Generate()
    {
        for (int x = -previewSize; x <= previewSize; x++)
        {
            for (int y = -previewSize; y <= previewSize; y++)
            {
                Get(new(x, y));
            }
        }
    }
    public float Get(Vector2Int loc)
    {
        float value;
        if (noises.ContainsKey(loc))
        {
            value = noises[loc];
        } else
        {
            float xCoord = origin.x + loc.x * scale;
            float yCoord = origin.y + loc.y * scale;
            value = Mathf.PerlinNoise(xCoord, yCoord);
            noises.Add(loc, value);
        }
        return value;
    }
    private void OnValidate()
    {
        if (scale % 1f == 0f) scale += 0.1f;
        noises.Clear();
        Generate();
    }
}
