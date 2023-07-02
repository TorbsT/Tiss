using Assets.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Components
{
    public class Item : MonoBehaviour
    {
        public UpgradeObject UpgradeObject { get; set; }
        [field: SerializeField] public Image Image { get; private set; }
        [field: SerializeField] public Image LevelBG { get; private set; }
        [field: SerializeField] public TextMeshProUGUI LevelText { get; private set; }
        public int Level 
        { 
            get => int.Parse(LevelText.text); 
            set => LevelText.text = value.ToString();
        }
    }
}
