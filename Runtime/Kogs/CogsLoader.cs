using System;
using System.Reflection;
using Kukuru3.Kogs.View;

using UnityEngine;
using UnityEngine.Rendering;

namespace Kukuru3.Kogs {
    internal class CogsLoader : KogsKomponent {

        internal CogsConfiguration Config { get; private set; }

        protected override void Launch() {
            base.Launch();
            CreateContext();
            GenerateIntrinsicShortcuts();
        }

        private void GenerateIntrinsicShortcuts() {
            global::Kogs.CreateShortcut("SHIFT+F1", "Toggle this window", () => KogsDispatcher.ToggleShortcutsPanel());
            global::Kogs.CreateShortcut("SHIFT+F2", "Cycle panels", () => KogsDispatcher.CyclePanels());
        }

        protected override void Teardown() {
            var objects = Module.GetModuleComponent<CogsDiagnosticsDisplay>();
            if (objects != null) {
                GameObject.Destroy(objects.CanvasObject);
            }
            base.Teardown();
        }

        bool IsURP() => GraphicsSettings.currentRenderPipeline != null;

        void CreateContext() {
            Config = Resources.Load<CogsConfiguration>("cogs_configuration");
            var canvasObject = GameObject.Instantiate(Config.CanvasPrefab);
            canvasObject.name = "[KOGS]";
            
            DoURPFix(canvasObject);
           
            var canvas = canvasObject.GetComponentInChildren<CogsCanvas>();
            GameObject.DontDestroyOnLoad(canvasObject);

            Module.CreateComponent<CogsSourceInferrer>();
            Module.AddComponent(new CogsDiagnosticsDisplay() {
                CanvasObject = canvas,
            });
            Module.CreateComponent<CogsShortcuts>();
            KogsDispatcher.AssignContext(Module);
        }

        private void DoURPFix(GameObject canvasObject) {
            if(IsURP()) {
                var mainCam = Camera.main;
                var kogsCam = canvasObject.GetComponentInChildren<Camera>();

                var shim = new URPShim();
                shim.AddCameraToStack(mainCam, kogsCam);
            }
        }
    }
}
