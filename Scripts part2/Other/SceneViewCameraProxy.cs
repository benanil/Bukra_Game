using UnityEngine;
using System.Collections.Generic;
using HighlightingSystem;

#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using Demonixis.Toolbox;
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SceneViewCameraProxy : MonoBehaviour
{
#if UNITY_EDITOR
    
    private SceneView SceneView;
    private Camera SceneCamera;

    private Camera ReferenceCamera;
    private bool UseReferenceCamera;

    private readonly static bool ReflectionCheckForIE = false;
    private bool CheckForStandardIE = true;

    private bool UpdateOnChange = true;
    private bool ResetIEOnDisable = true;
    private bool DebugImageEffects = false;

    // Used only for Update
    private int lastComponentCount;
    private List<Component> cachedComponents;

    private GameObject IEsourceGO { get { return UseReferenceCamera ? ReferenceCamera.gameObject : gameObject; } }

    public void OnEnable()
    {
        UpdateImageEffects();
    }

    public void OnValidate()
    { // Update when a variable on this script was changed
        if (!UpdateOnChange)
            OnEnable();
    }

    public void OnDisable()
    { // Reset image effects on disabling this component if desired
        if (ResetIEOnDisable)
            ResetImageEffects();
    }

    public void Update()
    {
        if (UpdateOnChange && Selection.activeGameObject == IEsourceGO)
        { // Update scene camera with changed image effects using cached components, as long as none are added or removed
            if (DebugImageEffects)
                Debug.Log("Updating reference camera due to changed components!");
            int componentCount = IEsourceGO.GetComponents<Component>().Length;
            if (lastComponentCount != componentCount)
            { // Image Effects might have been added or removed, so refetch them
                lastComponentCount = componentCount;
                cachedComponents = GetImageEffectComponents(IEsourceGO);
            }
            UpdateSceneCamera();
            if (SceneCamera != null)
                InternalCopyComponents(cachedComponents, SceneCamera.gameObject);
        }
    }

    private void UpdateSceneCamera()
    {
        if (UnityEditor.SceneView.lastActiveSceneView != null)
            SceneView = UnityEditor.SceneView.lastActiveSceneView;
        SceneCamera = SceneView == null ? null : SceneView.camera;
    }

    /// <summary>
    /// Returns all components filtered for image effects
    /// </summary>
    private List<Component> GetImageEffectComponents(GameObject GO)
    {
        List<Component> components = GO.GetComponents<Component>().ToList();
        if (components != null && components.Count > 0)
        { // Exclude Transform and Camera components
            if (ReflectionCheckForIE)
            { // Check if component implements OnRenderImage used for image postprocessing -> Perfect check!
                components = components.Where((Component c) => c.GetType().GetMethod("OnRenderImage", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) != null).ToList();
            }
            else if (CheckForStandardIE)
            { // Check if it is an standard image effects; unfortunately does not always work on 3rd party components!
                components = components.Where((Component c) => c.GetType().IsSubclassOf(typeof(FastPostProcessing))).ToList();
                components.AddRange(components.Where((Component c) => c.GetType().IsSubclassOf(typeof(HighlightingRenderer))));
            }
            else
            { // Check for all Components possibly being image effects, but may include normal scripts!
                components = components.Where((Component c) => {
                    Type cT = c.GetType();
                    return c != this && cT != typeof(Transform) /*&& cT != typeof(GUILayer)*/ && cT != typeof(Camera);
                }).ToList();
            }
        }
        return components;
    }

    /// <summary>
    /// Updates the image effects found on the proxy object to the scene camera
    /// </summary>
    private void UpdateImageEffects()
    {
        UpdateSceneCamera();
        if (SceneCamera == null)
            return;

        if (UseReferenceCamera && ReferenceCamera == null)
            throw new Exception("UseReferenceCamera enabled, but none chosen.");

        if (DebugImageEffects)
            Debug.Log("Applying image effects to '" + SceneCamera.gameObject + "':");

        lastComponentCount = IEsourceGO.GetComponents<Component>().Length;
        cachedComponents = GetImageEffectComponents(IEsourceGO);
        InternalCopyComponents(cachedComponents, SceneCamera.gameObject);
    }

    /// <summary>
    /// Resets all image effects found on the scene camera
    /// </summary>
    private void ResetImageEffects()
    {
        UpdateSceneCamera();
        if (SceneCamera == null)
            return;

        if (DebugImageEffects)
            Debug.Log("Resetting image effects of '" + SceneCamera.gameObject + "':");

        List<Component> components = GetImageEffectComponents(SceneCamera.gameObject);
        for (int i = 0; i < components.Count; i++)
        {
            Component comp = components[i];
            if (DebugImageEffects)
                Debug.Log(comp.GetType().Name);
           // DestroyImmediate(comp);
        }
    }

    private void InternalCopyComponents(List<Component> components, GameObject target)
    {
        if (components != null && components.Count > 0)
        {
            for (int i = 0; i < components.Count; i++)
            {
                Component comp = components[i];
                Type cType = comp.GetType();
                if (DebugImageEffects)
                    Debug.Log(cType.Name);
                // Copy component values
                Component existingComp = target.GetComponent(cType) ?? target.AddComponent(cType);
                EditorUtility.CopySerialized(comp, existingComp);
            }
        }
    }

#endif
}