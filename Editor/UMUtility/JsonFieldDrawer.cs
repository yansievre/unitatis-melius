using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UM.Runtime.UMUtility.JsonField;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace UM.Editor.UMUtility
{
    public class JsonFieldDrawer<T> : OdinAttributeDrawer<JsonFieldAttribute, T>
    {
        private const float K_ButtonSize = 20;
        private const string K_PrefKey = "_currentJsonTarget";
        private static string _currentJsonValue;
 

        private Vector2 _scrollPosition;
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var path = ValueEntry.Property.PrefabModificationPath;
            var isCurrentJsonTarget = path == EditorPrefs.GetString(K_PrefKey, "");
            // Get a rect to draw the health-bar on.
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            if (isCurrentJsonTarget)
            {
                if(label !=null)
                    SirenixEditorGUI.Title(label.text, null, TextAlignment.Left, true, true);
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false, GUILayout.Height(80), GUILayout.MaxHeight(400));
                
                _currentJsonValue = EditorGUILayout.TextArea(_currentJsonValue, GUILayout.ExpandHeight(true));
                
                EditorGUILayout.EndScrollView();
            }
            else
            {
                this.CallNextDrawer(label);
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(K_ButtonSize), GUILayout.MinWidth(K_ButtonSize));
            var buttonRect = EditorGUILayout.GetControlRect(false, K_ButtonSize, GUILayout.Width(K_ButtonSize));
            if(SirenixEditorGUI.SDFIconButton(buttonRect, SdfIconType.CodeSlash, null))
            {
                if (isCurrentJsonTarget)
                {
                    EditorPrefs.SetString(K_PrefKey, "");
                    ValueEntry.SmartValue = JsonConvert.DeserializeObject<T>(_currentJsonValue);
                }
                else
                {
                    EditorPrefs.SetString(K_PrefKey, path);
                    _currentJsonValue = JsonConvert.SerializeObject(ValueEntry.SmartValue, Formatting.Indented); //System.Text.Encoding.Default.GetString(Sirenix.Serialization.SerializationUtility.SerializeValue(ValueEntry.SmartValue, DataFormat.JSON));
                }
            }
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
        }
    }
}