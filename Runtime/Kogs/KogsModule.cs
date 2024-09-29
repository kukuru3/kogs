#if K3CORE_MODULES
using K3.Modules;

namespace Kukuru3.Kogs {

    public class KogsModule : BaseModule {
        protected override void Launch() => CreateComponent<CogsLoader>();
    }

    internal class KogsKomponent : Component<KogsModule> {

    }

}
#else
using System.Collections.Generic;
using K3.Modules;
using UnityEngine;

namespace Kukuru3.Kogs {

    [DefaultExecutionOrder(-19999)]
    public class KogsModule : MonoBehaviour {

        static bool alreadyLoaded; 

        readonly List<KogsKomponent> components = new();
        internal T CreateComponent<T>() where T : KogsKomponent, new() {
            var cmp = new T();
            AddComponent(cmp);
            return cmp;
        }

        internal void AddComponent<T>(T component) where T : KogsKomponent {
            components.Add(component);
            component.InjectModule(this);
        }        
        internal T GetModuleComponent<T>() {
            foreach (var c in components) if (c is T ct) return ct;
            return default;
        }

        void Awake() {
            if (alreadyLoaded) { Destroy(gameObject); return; }
            DontDestroyOnLoad(gameObject);
            alreadyLoaded = true;
            AddComponent(new CogsLoader());
        }

        void Update() {
            foreach (var cmp in components)
                if (cmp is IExecutesFrame framer) framer.Frame();
        }

        private void FixedUpdate() {
            foreach (var cmp in components)
                if (cmp is IExecutesTick ticker) ticker.Tick();
        }

        private void OnDestroy() {
            foreach (var cmp in components) cmp.Release();
        }
    }
}
#endif

