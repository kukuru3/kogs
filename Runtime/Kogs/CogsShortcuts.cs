using K3.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Kukuru3.Kogs {
    internal class CogsShortcuts : KogsKomponent, IExecutesFrame {
        readonly List<DebugShortcut> shortcutList = new();
        readonly K3.Collections.Multidict<KeyCombo, DebugShortcut> uniqueCombos = new();

        public void Frame() {
            var scs = EnumerateActivatedShortcuts();
            var count = scs.Count();
            if (count == 1) {
                scs.First().call();
            } else if (count > 1) {
                Debug.LogWarning($"Will activate multiple shortcuts belonging to this combo, but in the future we'll display a dialog");
                // for now:
                foreach (var item in scs) item.call();
                // in the future: display a UI element
            }
        }

        internal void AddShortcut(DebugShortcut s) {
            shortcutList.Add(s);
            uniqueCombos.Add(s.trigger, s);
        }

        internal IEnumerable<DebugShortcut> EnumerateActivatedShortcuts() {
            foreach (var combo in uniqueCombos.AllKeys())
                if (KogsUtility.IsCombo(combo))
                    return uniqueCombos.All(combo);
            return Array.Empty<DebugShortcut>();
        }
    }
}
