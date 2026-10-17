using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Builds a Windows Development player of whatever scenes are enabled in Build Settings
    /// (UNITY_RULES rule 48: a player is built and run before each milestone).
    /// </summary>
    /// <remarks>
    /// Changes no project setting. The exe lands in
    /// <c>B:\Projects\GameDev\Builds\hold-the-hill\windows\alpha-&lt;date&gt;\HoldTheHill.exe</c>
    /// unless <c>-buildOutput &lt;dir&gt;</c> is on the command line; build output is never committed.
    /// </remarks>
    internal static class AlphaPlayerBuild
    {
        private const string DefaultBuildRoot = @"B:\Projects\GameDev\Builds\hold-the-hill\windows";
        private const string ExeName = "HoldTheHill.exe";
        private const string OutputArgument = "-buildOutput";

        [MenuItem("Tools/Hold the Hill/Alpha/Build Development Player (Windows)")]
        private static void BuildFromMenu()
        {
            BuildReport report = Build(DefaultOutputDirectory());
            if (report != null && report.summary.result == BuildResult.Succeeded)
            {
                EditorUtility.RevealInFinder(report.summary.outputPath);
            }
        }

        /// <summary>
        /// Entry point for a batch run:
        /// <c>-batchmode -quit -buildTarget Win64 -executeMethod HoldTheHill.EditorTools.AlphaPlayerBuild.BuildFromCommandLine [-buildOutput &lt;dir&gt;]</c>.
        /// Exits with code 1 when the build does not succeed, so the calling script sees it.
        /// </summary>
        public static void BuildFromCommandLine()
        {
            BuildReport report = Build(OutputDirectoryFromArguments());
            bool succeeded = report != null && report.summary.result == BuildResult.Succeeded;
            if (!succeeded && Application.isBatchMode)
            {
                EditorApplication.Exit(1);
            }
        }

        private static BuildReport Build(string outputDirectory)
        {
            string[] scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            if (!SettingsAllowBuild(scenes))
            {
                return null;
            }

            Directory.CreateDirectory(outputDirectory);
            string exePath = Path.Combine(outputDirectory, ExeName);

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = exePath,
                target = BuildTarget.StandaloneWindows64,
                targetGroup = BuildTargetGroup.Standalone,
                options = BuildOptions.Development,
            };

            Debug.Log($"Alpha player build: {scenes.Length} scenes, first scene {scenes[0]}, to {exePath}");
            BuildReport report = BuildPipeline.BuildPlayer(options);
            LogSummary(report);
            return report;
        }

        /// <summary>
        /// Reports, and does not fix, anything in Player Settings that would block or mislabel the build.
        /// Settings changes are a team decision (no ProjectSettings edits from a build script).
        /// </summary>
        private static bool SettingsAllowBuild(string[] scenes)
        {
            bool ok = true;
            if (scenes.Length == 0)
            {
                Debug.LogError("Alpha player build: no scenes are enabled in Build Settings.");
                ok = false;
            }

            if (string.IsNullOrWhiteSpace(PlayerSettings.companyName))
            {
                Debug.LogError("Alpha player build: Player Settings has no company name. Ask before setting one.");
                ok = false;
            }

            if (string.IsNullOrWhiteSpace(PlayerSettings.productName))
            {
                Debug.LogError("Alpha player build: Player Settings has no product name. Ask before setting one.");
                ok = false;
            }

            ScriptingImplementation backend = PlayerSettings.GetScriptingBackend(NamedBuildTarget.Standalone);
            Debug.Log($"Alpha player build: scripting backend {backend}, company '{PlayerSettings.companyName}', product '{PlayerSettings.productName}', version {PlayerSettings.bundleVersion}");
            return ok;
        }

        private static void LogSummary(BuildReport report)
        {
            BuildSummary summary = report.summary;
            string line = $"Alpha player build {summary.result}: {summary.totalSize / (1024.0 * 1024.0):F1} MB in {summary.totalTime.TotalSeconds:F0} s, " +
                          $"{summary.totalErrors} errors, {summary.totalWarnings} warnings, output {summary.outputPath}";
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log(line);
            }
            else
            {
                Debug.LogError(line);
            }
        }

        private static string DefaultOutputDirectory()
        {
            return Path.Combine(DefaultBuildRoot, $"alpha-{DateTime.Now:yyyy-MM-dd}");
        }

        private static string OutputDirectoryFromArguments()
        {
            string[] args = Environment.GetCommandLineArgs();
            int index = Array.IndexOf(args, OutputArgument);
            if (index >= 0 && index + 1 < args.Length && !string.IsNullOrWhiteSpace(args[index + 1]))
            {
                return Path.GetFullPath(args[index + 1]);
            }

            return DefaultOutputDirectory();
        }
    }
}
