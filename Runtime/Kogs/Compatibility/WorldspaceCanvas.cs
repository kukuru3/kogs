using System;
using UnityEngine;

namespace Kukuru3.Kogs  {
    public class WorldspaceCanvas : MonoBehaviour {
        bool isInjected;
        [SerializeField] Vector2 dimensions;
        [SerializeField] float   pixelsPerUnit = 100;

        void Update() {
            if (!isInjected) {
                TryInject();
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(dimensions.x, dimensions.y, 0));
        }

        private void TryInject() {
            var canvas = KogsDispatcher.GetDisplay().CanvasObject;
            if (canvas == null) return;
            isInjected = true;
            canvas.transform.SetParent(this.transform, false);
            canvas.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            var unityCanvas = canvas.GetComponentInChildren<Canvas>();
            unityCanvas.renderMode = RenderMode.WorldSpace;

            var rectT = unityCanvas.GetComponent<RectTransform>();
            
            // var currentX = rectT.sizeDelta.x * rectT.lossyScale.x;
            // var currentY = rectT.sizeDelta.y * rectT.lossyScale.y;

            // sizeDelta.y * lossyScale.y should equal dimensions.y

            rectT.localScale = Vector3.one * (1f / pixelsPerUnit);

            var s = rectT.sizeDelta;
            s.x = dimensions.x / rectT.lossyScale.x;
            s.y = dimensions.y / rectT.lossyScale.y;
            rectT.sizeDelta = s;

            var desiredLossyScaleY = dimensions.y / rectT.sizeDelta.y;
            unityCanvas.transform.localScale *= desiredLossyScaleY / unityCanvas.transform.lossyScale.y;

            //var s = rectT.sizeDelta;
            //s.x = dimensions.x / rectT.lossyScale.x;
            //s.y = dimensions.y / rectT.lossyScale.y;
            //rectT.sizeDelta = s;




            // Debug.Log($"{x:f4}x{y:f4}");
            // unityCanvas.transform.localScale = Vector3.one * 0.01f;

        }
    }
}