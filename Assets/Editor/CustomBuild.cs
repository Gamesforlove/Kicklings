using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Editor
{
    public static class CustomBuild
    {
        public static void Build()
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
            {
                Debug.Log("Setting WebGL compression to Disabled...");
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
                PlayerSettings.WebGL.decompressionFallback = false;
            }

            // Automatically get all enabled scenes
            string[] scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            BuildPlayerOptions buildPlayerOptions = new()
            {
                scenes = scenes,
                locationPathName = GetOutputPath(),
                target = EditorUserBuildSettings.activeBuildTarget,
                options = BuildOptions.None
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        static string GetOutputPath() => EditorUserBuildSettings.activeBuildTarget switch
        {
            BuildTarget.StandaloneWindows64 => $"build/StandaloneWindows64/{Application.productName}.exe",
            BuildTarget.WebGL => "build/WebGL"
        };
    }
}