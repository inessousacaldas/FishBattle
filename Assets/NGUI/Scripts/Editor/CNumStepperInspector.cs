using Assets.Standard_Assets.NGUI.CustomExtension.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace Assets.NGUI.Scripts.Editor{
    [CustomEditor(typeof (CNumStepper))] ///
    /// 
    internal class CNumStepperInspector : UIWidgetInspector{
        protected override void DrawCustomProperties(){
            NGUIEditorTools.DrawProperty("Input", serializedObject, "Input");
            NGUIEditorTools.DrawProperty("RightBtn", serializedObject,"RightBtn");
            NGUIEditorTools.DrawProperty("LeftBtn", serializedObject,"LeftBtn");
            NGUIEditorTools.DrawProperty("MaxBtn", serializedObject, "btnMax");
            NGUIEditorTools.DrawProperty("MinBtn", serializedObject, "btnMin");
            NGUIEditorTools.DrawProperty("lblMax", serializedObject, "lblMax");
            NGUIEditorTools.DrawProperty("Min", serializedObject, "Min");
            NGUIEditorTools.DrawProperty("Max", serializedObject, "Max");
            NGUIEditorTools.DrawProperty("Step", serializedObject, "Step");
            NGUIEditorTools.DrawProperty("DefaultValue", serializedObject, "DefaultValue");
            
            base.DrawCustomProperties();
        }
    }
}