using UnityEngine;

namespace Kukuru3.Kogs.View {

    internal struct ViewData {
        internal float flash;
        internal float alpha;
        internal float life;
    }

    abstract class BaseCogsLiveView : MonoBehaviour {
        internal virtual bool RefreshObsoletion => false;

        internal abstract void Maintain(ViewData data);

        internal abstract void ConfigureDefault();
    }

    abstract class CogsDiagnosticView<TValues, TConfiguration> : BaseCogsLiveView, IConfigurable<TConfiguration>, IReceivesValue<TValues> {

        bool alreadyConfigured = false;

        public void Configure(TConfiguration config) {
            if (alreadyConfigured) return;
            Config = config;
            OnConfigured();
            alreadyConfigured = true;
        }

        internal sealed override void ConfigureDefault() {
            Config = default;
            OnConfigured();
        }

        protected TConfiguration Config { get; set; }

        protected TValues Value { get; private set; }

        public void AssignValue(TValues value) {
            Value = value;
            OnValueAssigned();
        }

        protected virtual void OnConfigured() { }
        protected virtual void OnValueAssigned() { }

    }
}