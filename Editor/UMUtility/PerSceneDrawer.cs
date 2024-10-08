using UM.Runtime.UMUtility.PerScene;
using UnityEditor;
using UnityEngine;

namespace UM.Editor.UMUtility
{

    [CustomPropertyDrawer(typeof(PerScene<>))]
    public class PerSceneDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property.FindPropertyRelative("_instance"), label);
            EditorGUI.EndDisabledGroup();
        }
    }
}