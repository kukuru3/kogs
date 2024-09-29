#if !K3CORE
using System;

using UnityEngine;

namespace Kukuru3.Kogs {
    
    interface IExecutesFrame {
        void Frame();
    }

    interface IExecutesTick {
        void Tick();
    }

    internal abstract class KogsKomponent {
        protected KogsModule Module { get; private set; }
        internal void InjectModule(KogsModule kogsModule) {
            this.Module = kogsModule;
            Launch();
        }

        protected virtual void Launch() { }
        protected virtual void Teardown() { }
        
        protected T GetModuleComponent<T>() => Module.GetModuleComponent<T>();
        internal void Release() => Teardown();
    }
}
#endif