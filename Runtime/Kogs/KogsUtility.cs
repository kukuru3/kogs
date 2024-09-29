using System;
using System.Collections.Generic;

using UnityEngine;

namespace Kukuru3.Kogs {
    static class KogsUtility {
        static internal Timings InferLoopTiming() => UnityEngine.Time.inFixedTimeStep ? Timings.Tick : Timings.Frame;

        static internal string PrintCombo(KeyCombo s) {
            lst.Clear();
            if (s.modifiers.HasFlag(ModifierKeys.Ctrl)) lst.Add("Ctrl");
            if (s.modifiers.HasFlag(ModifierKeys.Shift)) lst.Add("Shift");
            if (s.modifiers.HasFlag(ModifierKeys.Alt)) lst.Add("Alt");
            lst.Add(s.key.ToString());
            return string.Join('+', lst);
        }

        static readonly List<string> lst = new(4);

        static bool ModifiersMatchState(ModifierKeys keys) {
            var result = true;
            if (keys.HasFlag(ModifierKeys.Shift)) result = result && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
            if (keys.HasFlag(ModifierKeys.Ctrl)) result = result && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
            if (keys.HasFlag(ModifierKeys.Alt)) result = result && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
            return result;
        }

        static bool IsKeyPressed(KeyCode key) => Input.GetKeyDown(key);

        static internal bool IsCombo(KeyCombo kombo) => ModifiersMatchState(kombo.modifiers) && IsKeyPressed(kombo.key);

        internal static KeyCombo ReverseEngineerCombo(string fromString) {
            var items = fromString.ToLowerInvariant().Split('-', '+', ' ');
            KeyCombo result = default;
            var countUniqueIDs = 0;
            foreach (var c in items) {
                if (c.Length == 0) continue;
                if (c == "shift") result.modifiers |= ModifierKeys.Shift;
                else if (c == "ctrl" || c == "control") result.modifiers |= ModifierKeys.Ctrl;
                else if (c == "alt") result.modifiers |= ModifierKeys.Alt;
                else {
                    if (Enum.TryParse<KeyCode>(c, true, out var code)) {
                        result.key = code;
                        countUniqueIDs++;
                    }
                }
            }
            if (countUniqueIDs > 1) result.key = KeyCode.None;
            return result;
        }
    }
}
