using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    using Components;
    internal class FogManager : MonoBehaviour
    {
        public Material fogMaterial;  // Reference to the FogMaterial you created
        public List<Room> visibleTiles;  // Array of tiles that should not be fogged

        private void Start()
        {
            DisturbanceSystem.Instance.StateChanged += DisturbanceChanged;
        }
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            // Set the fog material and fog color
            Graphics.Blit(src, dest, fogMaterial);

            // Disable fog on visible tiles
            for (int i = 0; i < visibleTiles.Count; i++)
            {
                Renderer renderer = visibleTiles[i].FogRenderer;
                if (renderer != null)
                    renderer.material.SetColor("_FogColor", Color.clear);
                if (renderer != null)
                    renderer.material.SetFloat("_FogStrength", 0f);
            }
        }
        private void DisturbanceChanged()
        {
            visibleTiles = new();
            foreach (var room in DisturbanceSystem.Instance.CopyRooms())
            {
                if (room.Disturbance > 0.01f)
                    visibleTiles.Add(room);
            }
        } 
    }
}
