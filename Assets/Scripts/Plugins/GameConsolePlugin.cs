using Assets.Scripts.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Plugins
{
    using Assets.Scripts.Systems.Extensions;
    using Assets.Scripts.Systems.Towers;
    using System;

    public class GameConsolePlugin : MonoBehaviour, IConsolePlugin
    {
        public string Namespace => "game";

        public bool Execute(string command)
        {
            return false;
        }
    }
}