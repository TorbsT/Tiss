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
            Invoke(nameof(Execute), 1f);
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
