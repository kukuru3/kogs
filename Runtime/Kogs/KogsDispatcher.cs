using System;

namespace Kukuru3.Kogs {
    static class KogsDispatcher {
        private static CogsDiagnosticsDisplay canvas;
        private static CogsSourceInferrer inferrer;
        private static CogsShortcuts shortcuts;

        internal static void AssignContext(KogsModule module) {
            canvas = module.GetModuleComponent<CogsDiagnosticsDisplay>();
            inferrer = module.GetModuleComponent<CogsSourceInferrer>();
            shortcuts = module.GetModuleComponent<CogsShortcuts>();
        }

        internal static void CreateShortcut(DebugShortcut shortcut) {
            shortcuts.AddShortcut(shortcut);
            canvas.GenerateShortcutView(shortcut);
        }

        internal static IConfigurable<TConfig> Dispatch<TConfig, TElement, TValue>(DispatchRequest source, TValue value) where TElement : View.BaseCogsLiveView {
            if (inferrer == null) return default;
            inferrer.PopulateWithCallsiteInfo(ref source);

            var view = canvas.AddElement<TElement>(source);
            ((IReceivesValue<TValue>)view).AssignValue(value);

            return view as IConfigurable<TConfig>;
        }

        internal static void ToggleShortcutsPanel() {
            var go = canvas.CanvasObject.ShortcutsPanelInsertionRoot.gameObject;
            go.SetActive(!go.activeSelf);
        }
        internal static void ShowShortcutsPanel(bool doShow) {
            var go = canvas.CanvasObject.ShortcutsPanelInsertionRoot.gameObject;
            go.SetActive(doShow);
        }

        static int cycleIndex = -1;
        internal static bool AnyPanelShown() => cycleIndex >= 0;
        internal static void CyclePanels() {
            var l = canvas.ListCategoryPanels();
            if (++cycleIndex >= l.Count) cycleIndex = -1;
            canvas.SetActivePanel(cycleIndex == -1 ? null : l[cycleIndex]);
        }

        internal static void SetActiveCategory(string category) => canvas.SetActivePanel(category);
        internal static void Clear(string category) { canvas.ClearPanel(category); }
    }
}
