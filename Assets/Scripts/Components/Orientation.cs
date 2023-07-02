using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class Orientation : MonoBehaviour
    {
        private enum GizmosMode
        {
            Always,
            Selected,
            Never
        }
        [field: SerializeField, Range(0f, 360f)] public float FOV { get; set; } = 360f;
        [field: SerializeField] private GizmosMode Mode { get; set; }
        [field: SerializeField] private float GizmosDistance { get; set; } = 0.5f;
        [field: SerializeField] public float Local { get; set; }
        [field: SerializeField] public float Goal { get; set; }
        [field: SerializeField] public bool TargetOutOfFOV { get; set; }
        public float Global => transform.parent != null ? 
            Local + transform.parent.eulerAngles.z :
            Local;

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
            if (mode != Mode) return;
            Gizmos.color = Color.magenta;

            Vector2 pos0 = GetGizmosPosition(Global - FOV / 2f);
            Vector2 pos1 = GetGizmosPosition(Global + FOV / 2f);
            Gizmos.DrawLine(transform.position, pos0);
            Gizmos.DrawLine(transform.position, pos1);
            Gizmos.DrawLine(pos0, pos1);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, GetGizmosPosition(Goal, 1.25f));
        }
        private Vector2 GetGizmosPosition(float degree, float length = 1f)
        {
            float radians = Mathf.Deg2Rad * degree;

            // Calculate the direction using the converted angle
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

            // Calculate the desired position by adding the direction scaled by the distance
            Vector2 desiredPosition = (Vector2)transform.position + (GizmosDistance * length * direction);

            return desiredPosition;
        }
    }
}
