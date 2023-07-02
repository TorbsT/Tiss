using Assets.Scripts.Components;
using Assets.Scripts.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using TorbuTils.Anime;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public class WaveSystem : MonoBehaviour, IInteractionHandler
    {
        [System.Serializable]
        private class Instruction
        {
            [SerializeField, TextArea] public string txt;
        }
        public static WaveSystem Instance { get; private set; }
        [field: SerializeField] public AnimObject AnimObject { get; private set; }
        [field: SerializeField] public RectTransform Button { get; private set; }
        public float SpawnRate { get; set; } = -1;
        public int Wave { get; private set; }

        [SerializeField] private List<Instruction> waveInstructions = new();
        private string input;
        public void Select(GameObject go)
        {
            if (go == null) return;
            if (go.name != "NextTurn") return;
            NextWave();
        }
        public Tooltip.TooltipData Hover(GameObject go)
        {
            return null;
        }
        public void NextWave()
        {
            if (Wave >= waveInstructions.Count)
                Wave = 0;
            input = waveInstructions[Wave].txt;
            StartCoroutine(nameof(StartWave));
            Wave++;
        }
        private void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            Button.anchoredPosition = Vector2.right * Button.rect.width;
            ShopSystem.Instance.Built += BuiltFirst;
        }
        void OnDisable()
        {
            ShopSystem.Instance.Built -= BuiltFirst;
        }
        private void BuiltFirst()
        {
            ShopSystem.Instance.Built -= BuiltFirst;
            Anim<Vector2> slideAnim = new()
            {
                Action = (value) => { Button.anchoredPosition = value; },
                StartValue = Button.anchoredPosition,
                EndValue = Vector2.zero,
                Duration = AnimObject.Duration,
                Curve = AnimObject.Curve,
            };
            slideAnim.Start();
        }

        private IEnumerator StartWave()
        {
            string[] instructions = input.Split("\n");
            foreach (string instruction in instructions)
            {
                if (instruction.StartsWith("wait"))
                {
                    float time = float.Parse(instruction.Split(" ")[1]);
                    yield return new WaitForSeconds(time);
                    continue;
                }

                string enemy = instruction.Split(" ")[0];
                int count = int.Parse(instruction.Split(" ")[1]);
                // Adjust for spawnrate
                if (count > 0)
                    count = Mathf.Max(0, Mathf.RoundToInt(SpawnRate*count));
                for (int i = 0; i < count; i++)
                {
                    Vector2Int loc = ArrowGraph.Instance.Spawnpoint;
                    string command = $"enemy type:{enemy} loc:{loc.x},{loc.y}";
                    ConsoleActor.Instance.Execute(command);
                    yield return new WaitForSeconds(1f/count);
                }
            }
        }

    }
}