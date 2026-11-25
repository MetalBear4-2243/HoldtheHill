using System.Collections.Generic;
using System.IO;
using HoldTheHill.Core.Diagnostics;
using NUnit.Framework;
using UnityEngine;

namespace HoldTheHill.Tests
{
    public class LogFileTests
    {
        [Test]
        public void LogRing_KeepsTheNewestLines_OldestFirst()
        {
            var ring = new LogRing(3);
            for (int i = 1; i <= 5; i++)
            {
                ring.Add(new LogLine(LogType.Log, "line " + i));
            }

            var lines = new List<LogLine>();
            ring.CopyTo(lines);

            Assert.AreEqual(3, lines.Count);
            Assert.AreEqual("line 3", lines[0].Message);
            Assert.AreEqual("line 5", lines[2].Message);
            Assert.AreEqual(5, ring.Version);
        }

        [Test]
        public void LogRing_CountsErrors_EvenAfterTheyAreDropped()
        {
            var ring = new LogRing(2);
            ring.Add(new LogLine(LogType.Exception, "boom"));
            ring.Add(new LogLine(LogType.Error, "bad"));
            ring.Add(new LogLine(LogType.Warning, "hmm"));
            ring.Add(new LogLine(LogType.Log, "fine"));

            Assert.AreEqual(2, ring.ErrorCount);
        }

        [Test]
        public void DeleteOldest_KeepsTheNewestFiles()
        {
            string folder = Path.Combine(Path.GetTempPath(), "hth-log-tests-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            try
            {
                for (int day = 10; day <= 14; day++)
                {
                    File.WriteAllText(Path.Combine(folder, $"session-2026-11-{day}_12-00-00.log"), "x");
                }

                File.WriteAllText(Path.Combine(folder, "notes.txt"), "not a session log");

                LogFile.DeleteOldest(folder, keep: 2);

                string[] left = Directory.GetFiles(folder, "session-*.log");
                System.Array.Sort(left);
                Assert.AreEqual(2, left.Length);
                StringAssert.EndsWith("2026-11-13_12-00-00.log", left[0]);
                StringAssert.EndsWith("2026-11-14_12-00-00.log", left[1]);
                Assert.IsTrue(File.Exists(Path.Combine(folder, "notes.txt")));
            }
            finally
            {
                Directory.Delete(folder, recursive: true);
            }
        }
    }
}
