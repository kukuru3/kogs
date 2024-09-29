using UnityEngine;

namespace Kukuru3.Kogs.Compatibility {
    internal class KogsCompatibilityLauncher : MonoBehaviour {
#if K3CORE
        // this method is unnecessary if we have the more modern Core approach.
#else
        private void Start() {
            var module = gameObject.AddComponent<KogsModule>();
            module.CreateComponent<CogsLoader>();
        }
#endif
    }
}
