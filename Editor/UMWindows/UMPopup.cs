using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace UM.Editor.UMWindows
{
    public class UMPopup : EditorWindow
    {
        internal PopupWindowBuilder builder;
        
        public static PopupWindowBuilder CreatePopup(string title)
        {
            return new PopupWindowBuilder(title);
        }
        
        protected void OnGUI()
        {
            if (builder != null)
            {
                foreach (var action in builder.drawActions)
                {
                    action();
                }
            }
        }
    }
}