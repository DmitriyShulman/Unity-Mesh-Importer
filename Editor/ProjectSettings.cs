using UnityEditor;
using UnityEngine;

namespace MeshExtensions.Editor
{
    public static class ProjectSettings
    {
        private const string autoCollapsePref = "MeshImporter-AutoCollapse";
        private const string autoCombinePref = "MeshImporter-AutoCombine";
        private const string autoGenerateLightmapUV2 = "MeshImporter-AutoUV2";

        public static bool AutoCollapsePostProcessor
        {
            get => EditorPrefs.GetBool(autoCollapsePref, false);
            set => EditorPrefs.SetBool(autoCollapsePref, value);
        }

        public static bool AutoCombinePostProcessor
        {
            get => EditorPrefs.GetBool(autoCombinePref, false);
            set => EditorPrefs.SetBool(autoCombinePref, value);
        }

        public static bool AutoGenerateUV2
        {
            get => EditorPrefs.GetBool(autoGenerateLightmapUV2, false);
            set => EditorPrefs.SetBool(autoGenerateLightmapUV2, value);
        }
    }
}