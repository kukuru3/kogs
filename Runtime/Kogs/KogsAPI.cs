using Kukuru3.Kogs;

public class Kogs {
    public static DispatchRequest Display => new();
    public static DispatchRequest InGroup(string cat) => Display.InGroup(cat);
    public static DispatchRequest From(object @this) => Display.From(@this);

    public static IConfigurable<LabelConfiguration> Label(string text) => Display.Label(text);

    public static IConfigurable<LampsConfiguration> Signal(string text, params bool[] values) => Display.Signal(text, values);

    public static IConfigurable<LampsConfiguration> Signal(string text) => Display.Signal(text, new bool[] { true });

    public static IConfigurable<BarConfiguration> Bar(string text, float value) => Display.Bar(text, value);

    public static IConfigurable<ChartConfiguration> Chart(string text, float value) => Display.Chart(text, value);

    public static void CreateShortcut(KeyCombo combo, string description, System.Action call) {
        if (!combo.IsValid) {
            UnityEngine.Debug.LogError($"Supplied combo {combo} invalid; will not create shortcut `{description}`");
            return;
        }
        var shortcut = new DebugShortcut { trigger = combo, call = call, description = description };
        KogsDispatcher.CreateShortcut(shortcut);
    }

    public static void SetActiveCategory(string category) => KogsDispatcher.SetActiveCategory(category);

    public static void CreateShortcut(string combo, string description, System.Action call) {
        var cmb = KogsUtility.ReverseEngineerCombo(combo);
        if (!cmb.IsValid) {
            UnityEngine.Debug.LogError($"String combo invalid : `{combo}`; will not create shortcut `{description}`");
            return;
        }
        CreateShortcut(cmb, description, call);
    }
}