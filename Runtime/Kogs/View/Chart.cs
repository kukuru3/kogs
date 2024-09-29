
using System.Collections.Generic;

using UnityEngine;

namespace Kukuru3.Kogs.View {

    class Chart : CogsDiagnosticView<CaptionAndValue, ChartConfiguration> {
        [field: SerializeField] TMPro.TextMeshProUGUI Caption { get; set; }
        [field: SerializeField] TMPro.TextMeshProUGUI ValueLabel { get; set; }
        [field: SerializeField] LineRenderer LineRenderer { get; set; }

        float stateMinimumValue;
        float stateMaximumValue;
        float timeOfLastInsertion;
        readonly Queue<(float time, float value)> history = new();

        protected override void OnConfigured() {
            var c = Config;
            if (c.historyLength == 0) c.historyLength = 5f;
            if (c.frequency == 0) c.frequency = 0.05f;
            Config = c;

            stateMinimumValue = Config.minimumValue;
            stateMaximumValue = Config.maximumValue;
            if (!Config.lockBoundaries && stateMinimumValue == 0 && stateMaximumValue == 0) {
                stateMaximumValue = 1;
            }
        }

        private Vector3[] lrVertices;

        internal override void Maintain(ViewData data) {
            var timeSinceLast = Time.time - timeOfLastInsertion;
            if (timeSinceLast >= Config.frequency) {
                PruneHistory();
                AddToHistory(Value.value);
                UpdateMinMax(Value.value);
            }

            UpdateGraph();
            ValueLabel.text = $"{Value.value:F2}";
            Caption.text = Value.caption;
        }

        private void AddToHistory(float value) {
            timeOfLastInsertion = Time.time;
            var element = (timeOfLastInsertion, value);
            history.Enqueue(element);
            if (!Config.lockBoundaries) {
                if (value < stateMinimumValue) stateMinimumValue = value;
                if (value > stateMaximumValue) stateMaximumValue = value;
            }

        }

        private void UpdateMinMax(float val) {
            if (val < stateMinimumValue) stateMinimumValue = val;
            if (val > stateMaximumValue) stateMaximumValue = val;
        }

        private void PruneHistory() {
            while (history.Count > 0 && history.Peek().time < Time.time - Config.historyLength)
                history.Dequeue();
        }

        void UpdateGraph() {
            LineRenderer.positionCount = history.Count;
            if (lrVertices == null || lrVertices.Length != history.Count)
                lrVertices = new Vector3[history.Count];

            var index = 0;
            foreach (var item in history) {

                var timeFactor = Mathf.InverseLerp(Time.time - Config.historyLength, Time.time, item.time);
                var x = Mathf.Lerp(-100, 100, timeFactor);

                var posFactor = Mathf.InverseLerp(stateMinimumValue, stateMaximumValue, item.value);
                var y = Mathf.Lerp(-30, 30, posFactor);

                lrVertices[index] = new Vector3(x, y, 0);
                index++;
            }
            LineRenderer.SetPositions(lrVertices);
        }
    }
}