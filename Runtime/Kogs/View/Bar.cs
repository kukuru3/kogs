
using UnityEngine;

namespace Kukuru3.Kogs.View {
    class Bar : CogsDiagnosticView<CaptionAndValue, BarConfiguration> {

        [field: SerializeField] internal RectTransform BarTransform { get; private set; }
        [field: SerializeField] internal TMPro.TextMeshProUGUI Caption { get; private set; }
        [field: SerializeField] internal TMPro.TextMeshProUGUI ValueLabel { get; private set; }

        float stateMinimumValue;
        float stateMaximumValue;

        protected override void OnConfigured() {
            stateMinimumValue = Config.minimumValue;
            stateMaximumValue = Config.maximumValue;
            if (!Config.lockBoundaries && stateMinimumValue == 0 && stateMaximumValue == 0) {
                stateMaximumValue = 1;
            }
        }

        internal override void Maintain(ViewData data) {
            Caption.text = Value.caption;
            ValueLabel.text = Value.value.ToString($"F{Config.digits}");

            if (!Config.lockBoundaries) {
                if (Value.value < stateMinimumValue) stateMinimumValue = Value.value;
                if (Value.value > stateMaximumValue) stateMaximumValue = Value.value;
            }

            var t = Mathf.InverseLerp(stateMinimumValue, stateMaximumValue, Value.value);
            t = Mathf.Clamp01(t);

            BarTransform.sizeDelta = new Vector2(200 * t, BarTransform.sizeDelta.y);
        }

    }
}