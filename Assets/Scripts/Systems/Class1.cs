using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    internal class Class1 : MonoBehaviour
    {
        [SerializeField, TextAreaAttribute] private string startCommand;

        private void Start()
        {
            ChunkLoader.Instance.LoadedDelta += Loaded;
        }
        private void OnDisable()
        {
            ChunkLoader.Instance.LoadedDelta -= Loaded;
        }
        private void Loaded(Dictionary<Vector2Int, GameObject> loaded)
        {
            ChunkLoader.Instance.LoadedDelta -= Loaded;
            Execute();
        }
        private void Execute()
        {
            string[] commands = startCommand.Split(";");
            foreach (string command in commands)
            {
                ConsoleActor.Instance.Execute(command);
            }
        }
    }
}
