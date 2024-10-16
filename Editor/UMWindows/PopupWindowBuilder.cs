using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace UM.Editor.UMWindows
{
    public class PopupWindowBuilder
    {
        private readonly string _title;
        internal readonly List<Action> drawActions = new List<Action>();

        public PopupWindowBuilder(string title)
        {
            _title = title;
            drawActions.Add(() => UnityEngine.GUILayout.BeginVertical());
        }
        public PopupWindowBuilder AppendText(string text)
        {
            drawActions.Add(() => { UnityEngine.GUILayout.Label(text, SirenixGUIStyles.MultiLineCenteredLabel); });

            return this;
        }
        
        public PopupWindowBuilder AppendButton(string text, Action onClick)
        {
            drawActions.Add(() =>
            {
                if (UnityEngine.GUILayout.Button(text))
                {
                    onClick?.Invoke();
                }
            });

            return this;
        }
        
        public PopupWindowBuilder AppendFlexibleSpace()
        {
            drawActions.Add(() => UnityEngine.GUILayout.FlexibleSpace());

            return this;
        }
        public PopupWindowBuilder StartHorizontal(params GUILayoutOption[] options)
        {
            drawActions.Add(() => UnityEngine.GUILayout.BeginHorizontal(options));

            return this;
        }
        
        public PopupWindowBuilder EndHorizontal()
        {
            drawActions.Add(UnityEngine.GUILayout.EndHorizontal);

            return this;
        }
        
        public PopupWindowBuilder AppendStringField(string defaultValue, Action<string> onValueChanged = null, Func<string,bool> validateInput = null)
        {
            var val = defaultValue;
            var cachedColor = GUI.backgroundColor;
            var isValid = true;
            drawActions.Add(() =>
            {
                GUI.backgroundColor = isValid ? cachedColor*0.5f : Color.red;
                var newVal = UnityEngine.GUILayout.TextField(val, SirenixGUIStyles.CenteredTextField);
                GUI.backgroundColor = cachedColor;

                if (val == newVal)
                    return;

                if (validateInput != null && !validateInput(newVal))
                    isValid = false;
                else
                    isValid = true;
                onValueChanged?.Invoke(newVal);
                val = newVal;
            });

            return this;
        }
        
        public void CompleteWithResolve(string okButton, string cancelButton, Action onOk, Action onCancel = null)
        {
            UMPopup window = null;
            onOk += () => window.Close();
            if(onCancel != null)
                onCancel += () => window.Close();
            else
                onCancel = () => window.Close();
            StartHorizontal();
            AppendFlexibleSpace();
            AppendButton(okButton, onOk);
            AppendButton(cancelButton, onCancel);
            EndHorizontal();
            window = Complete();
        }
        
        public UMPopup Complete()
        {
            drawActions.Add(UnityEngine.GUILayout.EndVertical);
            var window = EditorWindow.GetWindow<UMPopup>();
            window.builder = this;
            window.titleContent = new GUIContent(_title);
            window.ShowPopup();
            

            return window;
        }
    }
}