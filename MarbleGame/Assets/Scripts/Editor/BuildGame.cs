#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MarbleGame.Editor
{
    public static class BuildGame
    {
        [MenuItem("MarbleGame/Build Windows Player")]
        public static void BuildWindowsPlayer()
        {
            string output = "Builds/MarbleGame/MarbleGame.exe";
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/GlassMachine_Prototype.unity" },
                locationPathName = output,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            });
            Debug.Log($"MarbleGame build result: {report.summary.result}, errors={report.summary.totalErrors}, warnings={report.summary.totalWarnings}");
            if (report.summary.result != BuildResult.Succeeded) throw new BuildFailedException("Windows player build failed.");
        }
    }
}
#endif
