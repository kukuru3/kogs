#if K3CORE
using K3;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Kukuru3.Kogs.View {
    class Signal : CogsDiagnosticView<CaptionAndBools, LampsConfiguration> {

        [field: SerializeField] TMPro.TextMeshProUGUI CaptionLabel { get; set; }
        [field: SerializeField] RectTransform LampContainer { get; set; }
        [field: SerializeField] GameObject LampPrefab { get; set; }

        struct PerLampState {
            internal float flash;
            internal float alpha;
            internal Image lamp;
            internal float timeSinceTriggered;
            internal bool triggered;
        }
        internal override bool RefreshObsoletion => maxTimeSinceTriggered < Config.linger;

        PerLampState[] perLampState;
        Color defaultColor;

        bool viewInitialized;
        private float maxTimeSinceTriggered;

        protected override void OnValueAssigned() {
            CaptionLabel.text = Value.caption;
            if (!viewInitialized) {
                viewInitialized = true;
                perLampState = new PerLampState[Value.values.Length];
                InitializeSubviews();
            }
            for (var i = 0; i < Value.values.Length; i++) {
                if (Value.values[i]) perLampState[i].triggered = true;
            }
        }

        private void InitializeSubviews() {
            for (var i = 0; i < Value.values.Length; i++) {
                var lamp = Instantiate(LampPrefab, LampContainer);
                var childImg = lamp.transform.GetChild(0).GetComponent<Image>();
                perLampState[i] = new PerLampState { flash = 0, alpha = 0, lamp = childImg };
                defaultColor = childImg.color;
            }
        }

        protected override void OnConfigured() {
            var c = Config;
            if (c.flashTime == 0) c.flashTime = 0.1f;
            if (c.fadeOutTime == 0) c.fadeOutTime = 0.05f;
            if (c.linger == 0) c.linger = 20f;
            Config = c;
        }

        internal override void Maintain(ViewData data) {
            for (var i = 0; i < perLampState.Length; i++) {
                var state = perLampState[i];
                if (state.triggered) {
                    state.triggered = false;
                    state.timeSinceTriggered = 0f;
                } else state.timeSinceTriggered += Time.unscaledDeltaTime;
                state.flash = state.timeSinceTriggered.Map(0f, Config.flashTime, 1f, 0f);
                state.alpha = state.timeSinceTriggered.Map(Config.flashTime, Config.flashTime + Config.fadeOutTime, 1f, 0f);
                UpdateLampGraphic(state);
                perLampState[i] = state;
            }

            maxTimeSinceTriggered = float.MinValue;
            foreach (var s in perLampState) if (s.timeSinceTriggered > maxTimeSinceTriggered) maxTimeSinceTriggered = s.timeSinceTriggered;

        }

        private void UpdateLampGraphic(PerLampState state) {
            var c = Color.Lerp(defaultColor, Color.white, state.flash);
            c.a = state.alpha;
            state.lamp.color = c;
        }
    }
}