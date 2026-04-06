using System.Collections.Generic;
using UnityEditor;

namespace Code.Editor
{
    // 커스텀 아이콘이 명시적으로 세팅된 SO의 GUID를 관리하는 레지스트리
    // SOIconSetting ↔ ProjectWindowIconModifier 간 공유
    public static class SoBadgeRegistry
    {
        private const string PrefsKey = "CustomIconSoBadgeGUIDs";

        private static HashSet<string> _guids;
        private static HashSet<string> Guids => _guids ??= Load();

        // ── 외부 API ──────────────────────────────────

        public static void Register(string guid)
        {
            if (Guids.Add(guid)) Save();
        }

        public static void Unregister(string guid)
        {
            if (Guids.Remove(guid)) Save();
        }

        public static bool Contains(string guid) => Guids.Contains(guid);

        // ── 저장 / 불러오기 ───────────────────────────

        private static void Save()
        {
            EditorPrefs.SetString(PrefsKey, string.Join("\n", Guids));
        }

        private static HashSet<string> Load()
        {
            var set = new HashSet<string>();
            string raw = EditorPrefs.GetString(PrefsKey, "");

            if (!string.IsNullOrEmpty(raw))
                foreach (string guid in raw.Split('\n'))
                    if (!string.IsNullOrWhiteSpace(guid))
                        set.Add(guid);

            return set;
        }
    }
}