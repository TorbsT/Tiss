using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TorbuTils.JUAI;

namespace Assets.Scripts.Systems
{
    using Extensions;
    using Components;
    using Common;
    internal class ConsoleActor : MonoBehaviour, IConsoleActor
    {
        public event Action<string, string, string> Executed;
        public static ConsoleActor Instance { get; private set; }
        private List<IConsolePlugin> consolePlugins = new();

        public bool Execute(string command)
        {
            string[] multiCommands = command.Split("\n");
            if (multiCommands.Length > 1)
            {
                bool succ = true;
                foreach (string multiCommand in multiCommands)
                    succ &= Execute(multiCommand);
                return succ;
            }

            command = command.Trim();
            string[] args = command.Split(" ");
            string mainarg = args[0];
            if (mainarg == "enemy")
            {
                int count = GetCountDefault1(command);
                ICollection<Vector2Int> locs = GetLocDefaultAll(command);
                string name = GetEnemyDefaultNull(command);
                foreach (Vector2Int loc in locs)
                {
                    for (int i = 0; i < count; i++)
                        ((EnemySystem)EnemySystem.Instance).Summon(loc, name);
                }
                return true;
            }
            else if (mainarg == "balance")
            {
                int count = GetCountDefault1(command);
                CurrencySystem.Instance.Balance = count;
                return true;
            }
            else if (mainarg == "kill")
            {
                foreach (var enemy in EnemySystem.Instance.CopyComponents())
                {
                    ((EnemySystem)EnemySystem.Instance).Kill(enemy);
                }
                return true;
            }
            else if (mainarg == "tower")
            {
                Vector2Int? locq = command.Loc();
                if (!locq.HasValue)
                {
                    ICollection<Vector2Int> emptyLocs = BuildSystem.Instance.CopyBuildableLocs();
                    if (emptyLocs.Count == 0)
                        return false;
                    locq = emptyLocs.GetRandom();
                }
                Vector2Int loc = locq.Value;

                int? radiusq = command.Range();
                int range = radiusq ?? 0;
                if (range > 1)
                {
                    bool success = true;
                    for (int x = loc.x - range; x < loc.x + range + 1; x++)
                    {
                        for (int y = loc.y - range; y < loc.y + range + 1; y++)
                        {
                            success = success && Execute(command.SetArg(Key.range, "0"));
                        }
                    }
                    return success;
                }

                string tower = command.Value(Key.tower);
                if (tower == null)
                {
                    tower = BuildSystem.Instance.CopyTowerNames().GetRandom();
                }

                float rot = GetRotDefault0(command);

                BuildSystem.Instance.Build(tower, loc, rot);
                string log = $"Built {loc} => {tower}";
                string stack = Environment.StackTrace;
                string category = "Game";
                Executed?.Invoke(log, stack, category);
                return true;
            } else if (mainarg == "upgrade")
            {
                string tower = args[1];
                if (!Upgrade.TowerExists(tower))
                    Debug.LogWarning($"{tower} does not exist");
                for (int i = 2; i < args.Length; i++)
                {
                    string arg = args[i];
                    string[] operators = { "=", "+", "-", "*", "/" };
                    foreach (var op in operators)
                    {
                        string[] divided = arg.Split(op);
                        if (divided.Length < 2) continue;
                        string name = divided[0];
                        string value = divided[1];

                        bool? oldBool = Upgrade.GetBool(tower, name);
                        if (oldBool.HasValue)
                        {
                            Upgrade.SetBool(tower, name, bool.Parse(value));
                            continue;
                        }
                        float? oldFloat = Upgrade.GetFloat(tower, name);
                        if (oldFloat.HasValue)
                        {
                            bool floatSucc =
                                float.TryParse(value, out float floatArg);
                            if (!floatSucc)
                                Debug.LogWarning($"Error on float {value}, {tower}");
                            var result = op switch
                            {
                                "=" => floatArg,
                                "+" => oldFloat.Value + floatArg,
                                "-" => oldFloat.Value - floatArg,
                                "*" => oldFloat.Value * floatArg,
                                "/" => oldFloat.Value / floatArg,
                                _ => oldFloat.Value,
                            };
                            Upgrade.SetFloat(tower, name, result);
                            continue;
                        }
                        int? oldInt = Upgrade.GetInt(tower, name);
                        if (oldInt.HasValue)
                        {
                            int intArg = int.Parse(value);
                            var result = op switch
                            {
                                "=" => intArg,
                                "+" => oldInt.Value + intArg,
                                "-" => oldInt.Value - intArg,
                                "*" => oldInt.Value * intArg,
                                "/" => oldInt.Value / intArg,
                                _ => oldInt.Value,
                            };
                            Upgrade.SetInt(tower, name, result);
                            continue;
                        }
                        Debug.LogWarning($"{tower} does not have {name}");
                        return false;
                    }
                }
                return true;
            }
            else
            {
                string nameSpace = command.Split(" ")[0].Split("::")[0];
                foreach (var plugin in consolePlugins)
                    if (plugin.Namespace == nameSpace)
                        return plugin.Execute(command);
                return false;
            }
        }

        public bool Validate(string command)
        {
            return true;
        }

        private void Awake()
        {
            Instance = this;
            consolePlugins = new();
            foreach (var plugin in GetComponents<IConsolePlugin>())
                consolePlugins.Add(plugin);
        }

        private UpgradeSystem Upgrade => UpgradeSystem.Instance;
        private float GetRotDefault0(string command)
            => command.Rot().HasValue ? command.Rot().Value : 0f;
        private int GetCountDefault1(string command)
            => command.Count().HasValue ? command.Count().Value : 1;
        private string GetEnemyDefaultNull(string command)
            => command.Value("type") != null ? command.Value("type") : null;
        private ICollection<Vector2Int> GetLocDefaultAll(string command)
            => command.Loc().HasValue ?
            new HashSet<Vector2Int>() { command.Loc().Value } :
            ChunkLoader.Instance.CopyLoaded().Keys;
    }
    public interface IConsolePlugin
    {
        bool Execute(string command);
        string Namespace { get; }
    }
}
