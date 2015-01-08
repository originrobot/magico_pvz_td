/*
* Energy Bar Toolkit by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnergyBarToolkit;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

[ExecuteInEditMode]
public class EnergyBarToolkitNGUIAnchor : UIWidget {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    [SerializeField]
    EnergyBar3DBase _energyBar;
    public EnergyBar3DBase energyBar {
        get {
            return _energyBar;
        }
        
        set {
            if (value == null) {
                _energyBar = null;
                return;
            }
            
            if (_energyBar != value) {
                _energyBar = value;
                var size = energyBar.DrawAreaRect;
                
                width = (int) size.width;
                height = (int) size.height;
                
                energyBarBaseWidth = size.width;
                energyBarBaseHeight = size.height;
            }
        }
    }
    
    [SerializeField]
    float energyBarBaseWidth;
    [SerializeField]
    float energyBarBaseHeight;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    // ===========================================================
    // Methods
    // ===========================================================
    
    new protected void OnEnable() {
        base.OnEnable();
        DuplicateFix();
    }
    
    #if UNITY_EDITOR
    void OnDestroy() {
        if (energyBar != null) {
            MadGameObject.SafeDestroy(energyBar.gameObject);
        }
        
        // hack to access NGUI OnDestroy() method
        MethodInfo dynMethod = typeof(UIWidget).GetMethod("OnDestroy", BindingFlags.NonPublic | BindingFlags.Instance);
        dynMethod.Invoke((UIWidget) this, new object[] {});
    }
    #endif

    protected override void OnUpdate() {
        base.OnUpdate();
        
        if (energyBar != null) {
            var size = energyBar.DrawAreaRect;
            energyBarBaseWidth = size.width;
            energyBarBaseHeight = size.height;
            
            energyBar.editorSelectable = false;
        }
    }
    
    public virtual void LateUpdate() {
        
        if (energyBar != null) {
            var center = transform.position;
            energyBar.transform.position = center;
            energyBar.transform.rotation = transform.rotation;
            energyBar.pivot = Translate(pivot);

            var barScale = energyBar.transform.parent.lossyScale;

            float width = mWidth * transform.lossyScale.x;
            float height = mHeight * transform.lossyScale.y;
            
            energyBar.transform.localScale =
                new Vector3(
                    width / barScale.x / energyBarBaseWidth,
                    height / barScale.y / energyBarBaseHeight,
                    1
                    );
            
            energyBar.opacity = finalAlpha;
            energyBar.tint = color;
        }
    }
    
    EnergyBar3DBase.Pivot Translate(UIWidget.Pivot pivot) {
        switch (pivot) {
            case Pivot.Left:
                return EnergyBar3DBase.Pivot.Left;
            case Pivot.TopLeft:
                return EnergyBar3DBase.Pivot.TopLeft;
            case Pivot.Top:
                return EnergyBar3DBase.Pivot.Top;
            case Pivot.TopRight:
                return EnergyBar3DBase.Pivot.TopRight;
            case Pivot.Right:
                return EnergyBar3DBase.Pivot.Right;
            case Pivot.BottomRight:
                return EnergyBar3DBase.Pivot.BottomRight;
            case Pivot.Bottom:
                return EnergyBar3DBase.Pivot.Bottom;
            case Pivot.BottomLeft:
                return EnergyBar3DBase.Pivot.BottomLeft;
            case Pivot.Center:
                return EnergyBar3DBase.Pivot.Center;
            default:
                Debug.Log("Unknown pivot: " + pivot);
                return EnergyBar3DBase.Pivot.Center;
        }
    }
    
    /// <summary>
    /// Resets NGUI bar dimensions to match its exact size for ebt bar.
    /// Normally resizing bar won't resize NGUI widget. Use this function to resize the widget afterwards.
    /// </summary>
    public void ResetDimensions() {
        if (energyBar != null) {
            var size = energyBar.DrawAreaRect;
            
            width = (int) size.width;
            height = (int) size.height;
            
            energyBarBaseWidth = size.width;
            energyBarBaseHeight = size.height;
        }
    }
    
    // looking if there is other anchor that references the same bar. If there is, then creates a copy of this bar
    void DuplicateFix() {
        if (energyBar == null) {
            return;
        }
    
        if (HasDuplicate()) {
            CopyEnergyBar();
        }
    }
    
    bool HasDuplicate() {
        var anchors = FindObjectsOfType(typeof(EnergyBarToolkitNGUIAnchor)) as EnergyBarToolkitNGUIAnchor[];
        foreach (var anchor in anchors) {
            if (anchor == this) {
                continue;
            }
            
            if (anchor.energyBar == energyBar) {
                return true;
            }
        }
        
        return false;
    }
    
    void CopyEnergyBar() {
        var nEnergyBar = Instantiate(energyBar) as EnergyBar3DBase;
        nEnergyBar.transform.parent = energyBar.transform.parent;
        
        int w = width;
        int h = height;
        
        energyBar = nEnergyBar;
        
        // preserve NGUI width/height
        width = w;
        height = h;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    #if UNITY_EDITOR
    [MenuItem("Tools/Energy Bar Toolkit/Create NGUI Bar", false, 300)]
    [MenuItem("GameObject/Create Other/Energy Bar Toolkit/NGUI Bar", false, 2100)]
    public static void CreateNGUIBar() {
        var panel = FindObjectOfType(typeof(UIPanel)) as UIPanel;
        if (panel != null) {
            var child = MadTransform.CreateChild<EnergyBarToolkitNGUIAnchor>(panel.transform, "Energy Bar Anchor");
            Selection.activeObject = child;
        } else {
            EditorUtility.DisplayDialog("No UIPanel Found", "There's no UIPanel on the scene. "
                + "Have you initialized NGUI?", "Ouch...");
        }
    }
    #endif
    

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}
