using System;
using System.IO;
using UnityEngine;

namespace HoldTheHill.Core.Diagnostics
{
    /// <summary>
    /// Writes each session's warnings, errors and exceptions (with stack traces) to
    /// <c>persistentDataPath/logs/session-&lt;time&gt;.log</c>, under a header that says which build
    /// and machine it came from, so a play-tester can send one small file after a problem. Keeps
    /// the newest <see cref="KeepFiles"/> files. Runs in every build; also keeps the last lines in
    /// <see cref="Recent"/> for the dev console. Hooks itself up when the game starts.
    /// </summary>
    public static class LogFile
    {
        public const string FolderName = "logs";
        public const int KeepFiles = 10;

        private static readonly object s_lock = new object();
        private static StreamWriter s_writer;

        /// <summary>The last 40 log lines of any kind, for the dev console.</summary>
        public static LogRing Recent { get; private set; } = new LogRing(40);

        /// <summary>The file this session writes, or null if it could not be opened.</summary>
        public static string CurrentPath { get; private set; }

        /// <summary>The folder that holds the session logs.</summary>
        public static string Folder => Path.Combine(Application.persistentDataPath, FolderName);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Close();
            Recent = new LogRing(40);
            CurrentPath = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            Application.logMessageReceivedThreaded -= HandleLog;
            Application.logMessageReceivedThreaded += HandleLog;
            Application.quitting -= Close;
            Application.quitting += Close;
            Open();
        }

        /// <summary>
        /// Deletes the oldest <c>session-*.log</c> files in <paramref name="folder"/> until at most
        /// <paramref name="keep"/> are left. The names carry the time, so name order is age order.
        /// </summary>
        public static void DeleteOldest(string folder, int keep)
        {
            if (!Directory.Exists(folder))
            {
                return;
            }

            string[] files = Directory.GetFiles(folder, "session-*.log");
            Array.Sort(files, StringComparer.Ordinal);
            for (int i = 0; i < files.Length - keep; i++)
            {
                try
                {
                    File.Delete(files[i]);
                }
                catch (IOException)
                {
                    // Open in another copy of the game: leave it for next time.
                }
            }
        }

        private static void Open()
        {
            try
            {
                Directory.CreateDirectory(Folder);
                // One fewer than kept, to make room for this session's file.
                DeleteOldest(Folder, KeepFiles - 1);

                string path = Path.Combine(Folder, $"session-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log");
                lock (s_lock)
                {
                    s_writer = new StreamWriter(path, append: true) { AutoFlush = true };
                    CurrentPath = path;
                    s_writer.WriteLine("Hold the Hill session log");
                    s_writer.WriteLine($"Started:  {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    s_writer.WriteLine($"Build:    {Application.version} ({(Debug.isDebugBuild ? "development" : "release")}{(Application.isEditor ? ", Editor" : "")})");
                    s_writer.WriteLine($"Unity:    {Application.unityVersion}");
                    s_writer.WriteLine($"Platform: {Application.platform}, {SystemInfo.operatingSystem}");
                    s_writer.WriteLine($"Device:   {SystemInfo.processorType}, {SystemInfo.systemMemorySize} MB, {SystemInfo.graphicsDeviceName}");
                    s_writer.WriteLine($"Screen:   {Screen.width}x{Screen.height}");
                    s_writer.WriteLine("Warnings, errors and exceptions follow.");
                    s_writer.WriteLine();
                }
            }
            catch (Exception e)
            {
                // Logging must never break the game. Say so once, in Unity's own log.
                CurrentPath = null;
                Debug.LogWarning($"Session log could not be opened: {e.Message}");
            }
        }

        private static void Close()
        {
            lock (s_lock)
            {
                if (s_writer != null)
                {
                    s_writer.Dispose();
                    s_writer = null;
                }
            }
        }

        // Called from any thread, so no Unity API that needs the main thread.
        private static void HandleLog(string message, string stackTrace, LogType type)
        {
            Recent.Add(new LogLine(type, message));
            if (type == LogType.Log)
            {
                return;
            }

            lock (s_lock)
            {
                if (s_writer == null)
                {
                    return;
                }

                try
                {
                    s_writer.WriteLine($"[{DateTime.Now:HH:mm:ss}] {type}: {message}");
                    if (type != LogType.Warning && !string.IsNullOrEmpty(stackTrace))
                    {
                        s_writer.WriteLine(stackTrace.TrimEnd());
                    }
                }
                catch (Exception)
                {
                    // Disk full or file gone: stop writing rather than log about logging.
                    s_writer = null;
                }
            }
        }
    }
}
