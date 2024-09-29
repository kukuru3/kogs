using UnityEngine;

namespace Kukuru3.Kogs {
    [CreateAssetMenu(fileName = "cogs", menuName = "Embers/Cogs")]
    class CogsConfiguration : ScriptableObject {
        [field: SerializeField] public GameObject CanvasPrefab { get; private set; }
        [field: SerializeField] public GameObject CategoryPanelPrefab { get; private set; }
        [field: SerializeField] public GameObject LabelPrefab { get; private set; }
        [field: SerializeField] public GameObject BarPrefab { get; private set; }
        [field: SerializeField] public GameObject GraphPrefab { get; private set; }
        [field: SerializeField] public GameObject ShortcutItemPrefab { get; private set; }
        [field: SerializeField] public GameObject LampPrefab { get; private set; }

    }
}
