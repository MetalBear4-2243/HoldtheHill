using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Core.Diagnostics
{
    /// <summary>One line from the Unity log.</summary>
    public readonly struct LogLine
    {
        public LogLine(LogType type, string message)
        {
            Type = type;
            Message = message;
        }

        public LogType Type { get; }
        public string Message { get; }

        /// <summary>Error, assert or exception.</summary>
        public bool IsError => Type == LogType.Error || Type == LogType.Assert || Type == LogType.Exception;
    }

    /// <summary>
    /// The last few log lines, oldest dropped first, for the dev console. Safe to add to from
    /// any thread (Unity reports logs from worker threads too).
    /// </summary>
    public class LogRing
    {
        private readonly LogLine[] _lines;
        private readonly object _lock = new object();
        private int _next;
        private int _count;

        public LogRing(int capacity)
        {
            _lines = new LogLine[Mathf.Max(1, capacity)];
        }

        /// <summary>How many lines it keeps.</summary>
        public int Capacity => _lines.Length;

        /// <summary>Errors, asserts and exceptions seen since it was made, including dropped ones.</summary>
        public int ErrorCount { get; private set; }

        /// <summary>Goes up on every add, so a reader can tell when to copy again.</summary>
        public int Version { get; private set; }

        public void Add(LogLine line)
        {
            lock (_lock)
            {
                _lines[_next] = line;
                _next = (_next + 1) % _lines.Length;
                _count = Mathf.Min(_count + 1, _lines.Length);
                if (line.IsError)
                {
                    ErrorCount++;
                }

                Version++;
            }
        }

        /// <summary>Fills <paramref name="into"/> with the kept lines, oldest first.</summary>
        public void CopyTo(List<LogLine> into)
        {
            into.Clear();
            lock (_lock)
            {
                int start = (_next - _count + _lines.Length) % _lines.Length;
                for (int i = 0; i < _count; i++)
                {
                    into.Add(_lines[(start + i) % _lines.Length]);
                }
            }
        }
    }
}
