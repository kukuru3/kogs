using System;

namespace Kukuru3.Kogs {
    public struct DispatchRequest {
        internal string category;
        internal object explicitSource;
        internal int callsiteHash;
        internal int callsiteCounter;
        internal Timings callsiteTiming;

        public DispatchRequest InGroup(string cat) {
            category = cat;
            return this;
        }

        public DispatchRequest From(object @this) {
            explicitSource = @this;
            return this;
        }

        public IConfigurable<BarConfiguration> Bar(string text, float value) =>
            //return KogsDispatcher.DispatchBar(this, new() { caption = text, value = value });
            KogsDispatcher.Dispatch<BarConfiguration, View.Bar, CaptionAndValue>(this, new() { caption = text, value = value });

        public IConfigurable<LabelConfiguration> Label(string text) =>
            // return KogsDispatcher.DispatchLabel(this, text, default);
            KogsDispatcher.Dispatch<LabelConfiguration, View.Label, string>(this, text);

        public IConfigurable<ChartConfiguration> Chart(string text, float value) => KogsDispatcher.Dispatch<ChartConfiguration, View.Chart, CaptionAndValue>(this, new() { caption = text, value = value });

        static readonly bool[] _defaultSignalValueArray = new[] { true };
        public IConfigurable<LampsConfiguration> Signal(string text, params bool[] values) {
            var finalValues = (values.Length == 0) ? _defaultSignalValueArray : values;
            return KogsDispatcher.Dispatch<LampsConfiguration, View.Signal, CaptionAndBools>(this, new() { caption = text, values = finalValues });
        }

        public override int GetHashCode() => HashCode.Combine(category, explicitSource, callsiteHash, callsiteCounter, callsiteTiming);
        public void Clear() => KogsDispatcher.Clear(category);
    }
}
