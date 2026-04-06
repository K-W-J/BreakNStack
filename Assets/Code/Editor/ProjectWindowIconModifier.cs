using UnityEditor;
using UnityEngine;

namespace Code.Editor
{
    [InitializeOnLoad]
    public class ProjectWindowIconModifier
    {
        private static readonly Texture2D DefaultSoIcon;

        static ProjectWindowIconModifier()
        {
            DefaultSoIcon = EditorGUIUtility.IconContent("ScriptableObject Icon").image as Texture2D;
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            if (!SoBadgeRegistry.Contains(guid)) return;

            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path) || !path.EndsWith(".asset")) return;

            System.Type type = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (type == null || !typeof(ScriptableObject).IsAssignableFrom(type)) return;

            Rect badgeRect;

            if (selectionRect.height > 20)
            {
                float iconAreaSize = selectionRect.height - 16f;
                float badgeSize    = Mathf.Max(iconAreaSize * 0.3f, 12f);

                badgeRect = new Rect(
                    selectionRect.x + iconAreaSize - badgeSize,
                    selectionRect.y + iconAreaSize - badgeSize,
                    badgeSize, badgeSize
                );
            }
            else
            {
                badgeRect = new Rect(selectionRect.x + 8, selectionRect.y + 8, 8, 8);
            }

            GUI.DrawTexture(badgeRect, DefaultSoIcon);
        }
    }
}