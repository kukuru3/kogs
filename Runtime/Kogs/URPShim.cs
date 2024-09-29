using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

class URPShim {
    bool IsURPActive()
    {
        var pipelineAsset = GraphicsSettings.currentRenderPipeline;
        return pipelineAsset != null && pipelineAsset.GetType().ToString().Contains("UniversalRenderPipelineAsset");
    }

    internal void AddCameraToStack(Camera baseCamera, Camera overlayCamera)
    {
        if (!IsURPActive()) return;

        // Use reflection to check for the extension method's static class and method
        Type urpCameraDataType = Type.GetType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime");
        if (urpCameraDataType == null) {
            Debug.LogWarning("URP is not installed or UniversalAdditionalCameraData class not found.");
            return;
        }

        // Find the static class that contains the extension method
        Type extensionType = Type.GetType("UnityEngine.Rendering.Universal.CameraExtensions, Unity.RenderPipelines.Universal.Runtime");
        if (extensionType == null) {
            Debug.LogWarning("Cannot find CameraExtensions class.");
            return;
        }

        // Get the extension method (static method) for 'GetUniversalAdditionalCameraData'
        MethodInfo getAdditionalDataMethod = extensionType.GetMethod("GetUniversalAdditionalCameraData", BindingFlags.Static | BindingFlags.Public);
        if (getAdditionalDataMethod == null) {
            Debug.LogWarning("Cannot find GetUniversalAdditionalCameraData extension method.");
            return;
        }

        // Invoke the extension method on the base camera
        object baseCameraData = getAdditionalDataMethod.Invoke(null, new object[] { baseCamera });
        if (baseCameraData == null) {
            Debug.LogWarning("Base camera does not have UniversalAdditionalCameraData.");
            return;
        }

        object attachedCameraData = getAdditionalDataMethod.Invoke(null, new object[] { overlayCamera });

        
        PropertyInfo renderTypeProperty = urpCameraDataType.GetProperty("renderType", BindingFlags.Public | BindingFlags.Instance);
        if (renderTypeProperty == null) {
            Debug.LogWarning("Cannot find renderType property.");
            return;
        }

        // Enum for CameraRenderType.Overlay = 1
        renderTypeProperty.SetValue(attachedCameraData, 1); // 1 corresponds to CameraRenderType.Overlay
        

        // Access the camera stack property via reflection
        var cameraStackProperty = urpCameraDataType.GetProperty("cameraStack", BindingFlags.Public | BindingFlags.Instance);
        if (cameraStackProperty == null) {
            Debug.LogWarning("Cannot find cameraStack property.");
            return;
        }

        if (cameraStackProperty.GetValue(baseCameraData) is List<Camera> cameraStack && !cameraStack.Contains(overlayCamera)) {
            cameraStack.Add(overlayCamera);
            Debug.Log("Overlay camera added to the stack.");
        }
    }
}