#if K3CORE
using K3.Modules;
using K3;
#endif

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Kukuru3.Kogs.View;
using System;

namespace Kukuru3.Kogs {

    internal class CogsDiagnosticsDisplay : KogsKomponent, IExecutesFrame, IExecutesTick {

        internal class ElementData {
            internal Timings loopTiming;
            internal int sourceHash;
            internal BaseCogsLiveView view;
            internal float obsoletionCounter;
            internal float life;
            internal bool forceRetire;
        }

        private const float FLASH_TIME = 0.4f;
        private const float OBSOLETION_TIME = 0.4f;
        private const float OBSOLETION_FADEOUT = 0.3f;

        internal List<ElementData> elements = new();
        readonly Dictionary<int, ElementData> elementLookup = new();
        readonly List<KogsCategoryPanelView> categoryPanelList = new();
        readonly Dictionary<string, KogsCategoryPanelView> categoryPanelLookup = new();

        internal CogsCanvas CanvasObject { get; set; }

        struct ViewInstantiationConfiguration {
            internal string category;
        }

        delegate BaseCogsLiveView ViewFactory(ViewInstantiationConfiguration config);

        ViewFactory GetFactory<T>() {
            var t = typeof(T);
            if (t == typeof(Label)) return InstantiateLabel;
            else if (t == typeof(Bar)) return InstantiateBar;
            else if (t == typeof(Chart)) return InstantiateGraph;
            else if (t == typeof(Signal)) return InstantiateLamp;
            return default;
        }

        internal ShortcutItem GenerateShortcutView(DebugShortcut shortcut) {
            var prefab = GetLoader.Config.ShortcutItemPrefab;
            var obj = GameObject.Instantiate(prefab, CanvasObject.ShortcutsPanelInsertionRoot);
            var shortcutView = obj.GetComponent<ShortcutItem>();
            shortcutView.Init(shortcut);
            return shortcutView;
        }

        internal TView AddElement<TView>(DispatchRequest source) where TView : BaseCogsLiveView {

            var instantiationConfig = new ViewInstantiationConfiguration {
                category = source.category,
            };

            var hash = source.GetHashCode();
            if (!elementLookup.TryGetValue(hash, out var data)) {
                data = new ElementData {
                    view = GetFactory<TView>()(instantiationConfig),
                    sourceHash = hash,
                    loopTiming = KogsUtility.InferLoopTiming(),
                };
                elementLookup.Add(hash, data);
                elements.Add(data);
                data.view.ConfigureDefault();
            }
            data.obsoletionCounter = 0;

            return (TView)data.view;
        }

        CogsLoader GetLoader => Module.GetModuleComponent<CogsLoader>();

        View.Label InstantiateLabel(ViewInstantiationConfiguration config) {
            var prefab = GetLoader.Config.LabelPrefab;
            var obj = GameObject.Instantiate(prefab, GetInsertionRoot(config.category));
            return obj.GetComponent<View.Label>();
        }

        View.Bar InstantiateBar(ViewInstantiationConfiguration config) {
            var prefab = GetLoader.Config.BarPrefab;
            var obj = GameObject.Instantiate(prefab, GetInsertionRoot(config.category));
            return obj.GetComponent<View.Bar>();
        }

        View.Chart InstantiateGraph(ViewInstantiationConfiguration config) {
            var prefab = GetLoader.Config.GraphPrefab;
            var obj = GameObject.Instantiate(prefab, GetInsertionRoot(config.category));
            return obj.GetComponent<View.Chart>();
        }

        View.Signal InstantiateLamp(ViewInstantiationConfiguration config) {
            var prefab = GetLoader.Config.LampPrefab;
            var obj = GameObject.Instantiate(prefab, GetInsertionRoot(config.category));
            return obj.GetComponent<View.Signal>();
        }

        public void Frame() => MaintainElements(Timings.Frame);

        public void Tick() => MaintainElements(Timings.Tick);

        Transform GetInsertionRoot(string categoryID) => GetOrCreateCategoryPanel(categoryID).InsertionRoot;

        internal KogsCategoryPanelView GetCategoryPanel(string categoryID) {
            categoryID ??= "";
            categoryPanelLookup.TryGetValue(categoryID, out var panel);
            return panel;
        }

        internal List<KogsCategoryPanelView> ListCategoryPanels() => categoryPanelList;

        internal void SetActivePanel(KogsCategoryPanelView panel) {
            foreach (var existingPanel in ListCategoryPanels())
                existingPanel.SetState(existingPanel == panel);
        }

        internal void SetActivePanel(string categoryID) => SetActivePanel(GetCategoryPanel(categoryID));

        private KogsCategoryPanelView GetOrCreateCategoryPanel(string categoryID) {
            categoryID ??= "";
            if (!categoryPanelLookup.TryGetValue(categoryID, out var panel)) {

                var gameObject = GameObject.Instantiate(GetLoader.Config.CategoryPanelPrefab, CanvasObject.CategoryRoot);
                panel = gameObject.GetComponent<KogsCategoryPanelView>();
                panel.Bind(categoryID);
                categoryPanelLookup[categoryID] = panel;
                categoryPanelList.Add(panel);
                panel.SetState(false); // every created panel is inactive by default. Use SetActivePanel.
            }
            return panel;
        }


        public void ForceCleanup() {
            foreach (var e in elements) {
               if (e.forceRetire) {
                    GameObject.Destroy(e.view.gameObject);
                    elementLookup.Remove(e.sourceHash);
               }
            }
            elements.RemoveAll(e => e.forceRetire);
        }

        private void MaintainElements(Timings timing) {
            var anyObsolete = false;

            bool ShouldDestroy(ElementData element) {
                return element.obsoletionCounter >= OBSOLETION_TIME || element.forceRetire;
            }

            foreach (var element in elements) {
                if (element.loopTiming != timing) continue;

                element.life += Time.unscaledDeltaTime;
                element.obsoletionCounter += Time.unscaledDeltaTime;
                if (element.view.RefreshObsoletion) element.obsoletionCounter = 0;
                if (ShouldDestroy(element)) anyObsolete = true;
            }

            if (anyObsolete) {
                var toRemove = elements.Where(ShouldDestroy);
                foreach (var l in toRemove) {
                    GameObject.Destroy(l.view.gameObject);
                    elementLookup.Remove(l.sourceHash);
                }
                elements.RemoveAll(ShouldDestroy);
            }

            if (timing != Timings.Frame) return;

            foreach (var element in elements) {
                var flash = element.life.Map(0f, FLASH_TIME, 1f, 0f);
                var alpha = element.obsoletionCounter.Map(OBSOLETION_TIME - OBSOLETION_FADEOUT, OBSOLETION_TIME, 1f, 0f);
                element.view.Maintain(new() { flash = flash, alpha = alpha, life = element.life });
            }
        }

        internal void ClearPanel(string category) { 
            var catPanel = GetCategoryPanel(category);
            if (catPanel == null) return;
            foreach (Transform t in catPanel.InsertionRoot) {
                var v = t.GetComponent<BaseCogsLiveView>();
                var data = elements.FirstOrDefault(e => e.view == v);
                data.forceRetire = true;
            }
            ForceCleanup();
            // if (catPanel != null) catPanel.InsertionRoot.transform.DestroyAllChildren();
        }
    }
}
