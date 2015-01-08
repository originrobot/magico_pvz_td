/*
* Energy Bar Toolkit by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using EnergyBarToolkit;

[CustomEditor(typeof(EnergyBarToolkitNGUIAnchor))]
public class EnergyBarToolkitNGUIAnchorInspector : UIWidgetInspector {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    EnergyBarToolkitNGUIAnchor component;
    EnergyBar3DBase.BarType barType;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    // ===========================================================
    // Methods
    // ===========================================================

    protected override void OnEnable() {
        base.OnEnable();
        component = target as EnergyBarToolkitNGUIAnchor;
    }

    public override void OnInspectorGUI() {
        if (component.energyBar != null) {
            var panel = MadTransform.FindParent<MadPanel>(component.energyBar.transform);
            
            if (panel != null && panel.gameObject.layer != component.gameObject.layer) {
                if (MadGUI.ErrorFix("Your panel exists on different layer than NGUI.", "Change It")) {
                    panel.gameObject.layer = component.gameObject.layer;
                }
                EditorGUILayout.Space();
            }
        }
    
        EditorGUI.BeginChangeCheck();
        
        if (component.energyBar == null) {
            MadGUI.Message("To use anchor object you have to assing existing energy bar to this field.",
                MessageType.Info);
        }
        
        component.energyBar = (EnergyBar3DBase) EditorGUILayout.ObjectField(
            "Energy Bar", component.energyBar, typeof(EnergyBar3DBase), true);
        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(component);
        }
    
        if (component.energyBar != null) {
            var eb = component.energyBar.GetComponent<EnergyBar>();
            if (eb != null) {
                EditorGUI.BeginChangeCheck();
                eb.valueCurrent = EditorGUILayout.IntSlider("Value", eb.valueCurrent, eb.valueMin, eb.valueMax);
                eb.valueMin = EditorGUILayout.IntField("Value Min", eb.valueMin);
                eb.valueMax = EditorGUILayout.IntField("Value Max", eb.valueMax);
                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(eb);
                }
            }
        
            base.OnInspectorGUI();
            NGUIEditorTools.SetLabelWidth(0); // cancel ngui label width
            GUI.color = Color.yellow;
            if (GUILayout.Button("Reset Dimensions")) {
                component.ResetDimensions();
            }
            GUI.color = Color.white;
        
            var energyBarEditor = Editor.CreateEditor(component.energyBar) as EnergyBarInspectorBase;
            energyBarEditor.showPositionAndSize = false;
            energyBarEditor.OnEnable();
            energyBarEditor.OnInspectorGUI();
            DestroyImmediate(energyBarEditor);
        } else {
            MadGUI.Box("Create Energy Bar", () => {
                MadGUI.Message("Or you can create a new one using this wizard.", MessageType.Info);
            
                barType = (EnergyBar3DBase.BarType) EditorGUILayout.EnumPopup("Bar Type", barType);
                
                GUI.color = Color.green;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Create")) {
                    component.energyBar = EnergyBarUtils.Create3DBar(barType);
                    EditorUtility.SetDirty(component);
                    
                    // selection is lost when new energy bar is created, reclaiming it
                    if (component.energyBar != null) {
                        Selection.activeGameObject = component.gameObject;
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = Color.white;
            });
        }
    }

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}
