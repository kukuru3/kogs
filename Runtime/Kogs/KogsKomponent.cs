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

namespace Kukuru3.Kogs {
    public abstract class KogsKomponent {

        protected virtual void Launch() { }
        protected virtual void Teardown() { }

        internal void Release() { 
            Teardown();
        }

        protected KogsModule Module { get; private set; }
        internal void InjectModule(KogsModule module) {
            this.Module = module;
            Launch();
        }
    }
}
#endif

