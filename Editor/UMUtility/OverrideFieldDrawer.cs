using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UM.Runtime.UMUtility.Optional;
using UnityEditor;
using UnityEngine;

namespace UM.Editor.UMUtility
{
    [CustomPropertyDrawer(typeof(OverrideField<>))]
    public class OverrideFieldDrawer<T> : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            if (label != null)
                rect = EditorGUI.PrefixLabel(rect, label);
            
            GUIHelper.PushLabelWidth(20);
            var hasOverride = property.FindPropertyRelative(nameof(OverrideField<T>.hasOverride));
            var value = property.FindPropertyRelative("_overrideValue");
            hasOverride.boolValue = EditorGUI.Toggle(rect.AlignLeft(20), value.boolValue, EditorStyles.toggle);
            EditorGUI.BeginDisabledGroup(!hasOverride.boolValue);
            if(hasOverride.boolValue)
                EditorGUI.PropertyField(rect.AlignRight(rect.width - 20), value, GUIContent.none);
            else
                EditorGUI.LabelField(rect.AlignRight(rect.width - 20), "Using default");
            EditorGUI.EndDisabledGroup();
            GUIHelper.PopLabelWidth();
        }
    }
    
}