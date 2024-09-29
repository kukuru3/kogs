using UnityEngine;

namespace Kukuru3.Kogs.View {
    class ShortcutItem : MonoBehaviour {
        [field: SerializeField] internal TMPro.TextMeshProUGUI Caption { get; private set; }
        [field: SerializeField] internal TMPro.TextMeshProUGUI Description { get; private set; }

        internal void Init(DebugShortcut shortcut) {
            Caption.text = KogsUtility.PrintCombo(shortcut.trigger);
            Description.text = shortcut.description;
        }
    }
}