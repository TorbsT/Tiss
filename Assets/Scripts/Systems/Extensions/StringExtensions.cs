using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq.Expressions;

namespace Assets.Scripts.Systems.Extensions
{
    using Common;
    public static class StringExtensions
    {
        internal static Vector2Int? Loc(this string command)
        {
            Vector2Int? result;
            string value = command.Value(Key.loc);
            if (value == null) return null;
            string[] parts = value.Split(",");
            if (parts.Length != 2) return null;
            result = new(int.Parse(parts[0]), int.Parse(parts[1]));
            return result;
        }
        internal static int? Range(this string command)
            => IntValue(command, Key.range);
        internal static int? Count(this string command)
            => IntValue(command, Key.count);
        internal static float? Rot(this string command)
            => FloatValue(command, Key.rot);
        internal static int? IntValue(this string command, Key key)
        {
            int? result;
            string value = command.Value(key);
            if (value == null) return null;
            result = int.Parse(value);
            return result;
        }
        internal static float? FloatValue(this string command, Key key)
        {
            float? result;
            string value = command.Value(key);
            if (value == null) return null;
            result = float.Parse(value);
            return result;
        }
        public static Func<int, float> ContextValue(this string command, string key)
        {
            string value = command.Value(key);
            if (value == null) return null;
            return ExpressionParser.ParseExpression(value);
        }
        public static string Value(this string command, Key key)
            => command.Value(key.ToString());
        public static string Value(this string command, string key)
        {
            string[] arr = command.Split(" ");
            foreach (string arg in arr)
            {
                string[] pargs = arg.Split(":");
                if (pargs.Length < 2) continue;
                if (pargs[0] == key) return pargs[1];
            }
            return null;
        }
        internal static string SetArg
            (this string command, Key key, string value)
        {
            string oldValue = command.Value(key);
            if (oldValue == null)
                return command + $" {key}:{value}";
            string[] arr = command.Split(" ");
            for (int i = 1; i < arr.Length; i++)
            {
                string[] pargs = arr[i].Split(":");
                if (pargs.Length < 2) continue;
                if (pargs[0] == key.ToString())
                {
                    arr[i] = $"{key}:{value}";
                    return string.Join(" ", arr);
                }
            }
            Debug.LogError("Should never happen");
            return command;
        }
        public static string[] SplitAny(this string text, params string[] splitters)
        {
            return text.Split(splitters, StringSplitOptions.None);
        }
    }
}
