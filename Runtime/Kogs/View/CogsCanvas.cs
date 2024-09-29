using UnityEngine;

namespace Kukuru3.Kogs.View {
    class CogsCanvas : MonoBehaviour {
        [field: SerializeField] internal RectTransform CategoryRoot { get; private set; }
        [field: SerializeField] internal RectTransform ShortcutsPanelInsertionRoot { get; private set; }
    }
}