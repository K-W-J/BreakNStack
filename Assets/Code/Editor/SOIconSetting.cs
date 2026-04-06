using UnityEditor;
using UnityEngine;

namespace Code.Editor
{
    [CustomEditor(typeof(ScriptableObject), true)]
    public class SOIconSetting : UnityEditor.Editor
    {
        private bool _showIconSettings;

        public override void OnInspectorGUI()
        {
            ScriptableObject data = (ScriptableObject)target;
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(data));

            EditorGUILayout.Space(10);
            _showIconSettings = EditorGUILayout.Foldout(_showIconSettings, "아이콘 설정", true, EditorStyles.foldoutHeader);

            if (_showIconSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();

                Rect buttonRect = GUILayoutUtility.GetRect(64, 64, GUILayout.Width(64), GUILayout.Height(64));
                if (GUI.Button(buttonRect, new GUIContent("Set Icon")))
                {
                    int controlID = GUIUtility.GetControlID(FocusType.Passive);
                    EditorGUIUtility.ShowObjectPicker<Sprite>(null, false, "", controlID);
                }

                if (GUILayout.Button("아이콘 초기화\n(Reset)", GUILayout.Width(64), GUILayout.Height(64)))
                {
                    EditorGUIUtility.SetIconForObject(data, null);
                    EditorUtility.SetDirty(data);
                    AssetDatabase.SaveAssets();

                    SoBadgeRegistry.Unregister(guid);
                    EditorApplication.RepaintProjectWindow();
                }

                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                Sprite selectedSprite = EditorGUIUtility.GetObjectPickerObject() as Sprite;

                if (selectedSprite != null)
                {
                    Texture2D spriteTexture = ExtractSpriteTexture(selectedSprite);

                    EditorGUIUtility.SetIconForObject(data, spriteTexture);
                    SoBadgeRegistry.Register(guid);
                }
                else
                {
                    EditorGUIUtility.SetIconForObject(data, null);
                    SoBadgeRegistry.Unregister(guid);
                }

                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssets();
                EditorApplication.RepaintProjectWindow();
            }

            EditorGUILayout.Space(10);
            DrawDefaultInspector();
        }

        // 스프라이트 시트에서 해당 스프라이트의 rect 영역만 잘라 새 Texture2D로 반환합니다.
        // Read/Write 설정 없이도 동작하도록 RenderTexture를 경유합니다.
        private static readonly int MinIconSize = 64;

        private static Texture2D ExtractSpriteTexture(Sprite sprite)
        {
            Rect rect = sprite.rect;

            RenderTexture rt = RenderTexture.GetTemporary(
                sprite.texture.width,
                sprite.texture.height,
                0,
                RenderTextureFormat.ARGB32
            );

            Graphics.Blit(sprite.texture, rt);
            RenderTexture.active = rt;

            var extracted = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
            extracted.ReadPixels(new Rect(rect.x, rect.y, rect.width, rect.height), 0, 0);
            extracted.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            // ✅ 최소 크기보다 작으면 업스케일
            int srcW = extracted.width;
            int srcH = extracted.height;

            if (srcW >= MinIconSize && srcH >= MinIconSize) return extracted;

            // 비율 유지하며 MinIconSize에 맞춤
            float scale = (float)MinIconSize / Mathf.Max(srcW, srcH);
            int dstW = Mathf.RoundToInt(srcW * scale);
            int dstH = Mathf.RoundToInt(srcH * scale);

            RenderTexture scaledRt = RenderTexture.GetTemporary(dstW, dstH, 0, RenderTextureFormat.ARGB32);

            // 픽셀 아트 계열은 Point, 일반 이미지는 Bilinear
            bool isPixelArt = srcW <= 32 || srcH <= 32;
            scaledRt.filterMode = isPixelArt ? FilterMode.Point : FilterMode.Bilinear;

            Graphics.Blit(extracted, scaledRt);
            RenderTexture.active = scaledRt;

            var upscaled = new Texture2D(dstW, dstH, TextureFormat.RGBA32, false);
            upscaled.ReadPixels(new Rect(0, 0, dstW, dstH), 0, 0);
            upscaled.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(scaledRt);

            return upscaled;
        }
    }
}