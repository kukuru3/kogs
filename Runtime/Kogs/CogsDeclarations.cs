using System;

namespace Kukuru3.Kogs {

    public enum Timings {
        Frame,
        Tick,
    }
    public interface IConfigurable<T> {
        void Configure(T config);
    }

    internal interface IReceivesValue<TValue> {
        void AssignValue(TValue value);
    }

    public struct BarConfiguration {
        public bool lockBoundaries;
        public float maximumValue;
        public float minimumValue;
        public int digits;
    }

    public struct ChartConfiguration {
        public bool lockBoundaries;
        public float maximumValue;
        public float minimumValue;
        public float gridValue;

        public float historyLength;
        public float frequency;
    }

    public struct LampsConfiguration {
        public float fadeOutTime;
        public float flashTime;
        public float linger;
    }

    public struct None { }

    public struct LabelConfiguration {
        public float linger;
    }

    internal struct CaptionAndValue {
        internal string caption;
        internal float value;
    }

    internal struct CaptionAndBools {
        internal string caption;
        internal bool[] values;
    }

    [System.Flags]
    public enum ModifierKeys {
        None = 0,
        Shift = 1,
        Ctrl = 2,
        Alt = 4,
    }

    public struct KeyCombo {
        public ModifierKeys modifiers;
        public UnityEngine.KeyCode key;

        public bool IsValid => key != UnityEngine.KeyCode.None;

        public override bool Equals(object obj) =>
            obj is KeyCombo combo &&
            modifiers == combo.modifiers &&
            key == combo.key;

        public override int GetHashCode() => HashCode.Combine(modifiers, key);
    }

    internal struct DebugShortcut {
        public KeyCombo trigger;
        public string description;
        public System.Action call;
    }
}
