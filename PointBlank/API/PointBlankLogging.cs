﻿using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PointBlank.API
{
    /// <summary>
    /// Logging methods for PointBlank
    /// </summary>
    public static class PointBlankLogging
    {
        #region Info
        public static readonly string LogPath = Directory.GetCurrentDirectory() + "//PointBlank.log";
        public static readonly string LogPathPrev = Directory.GetCurrentDirectory() + "//PointBlankOld.log";
        #endregion

        static PointBlankLogging()
        {
            if (File.Exists(LogPathPrev))
                File.Delete(LogPathPrev);
            if (File.Exists(LogPath))
                File.Move(LogPath, LogPathPrev);
        }

        /// <summary>
        /// Logs text into the logs file and console
        /// </summary>
        /// <param name="log">Object/Text to log</param>
        /// <param name="inConsole">Should the text be printed into the console</param>
        public static void Log(object log, bool inConsole = true)
        {
            StackTrace stack = new StackTrace(false);
            string asm = "";

            asm = stack.FrameCount > 0 ? stack.GetFrame(1).GetMethod().DeclaringType.Assembly.GetName().Name : "Not Found";

            if (stack.FrameCount > 1 && (asm == "PointBlank" || asm == "Assembly-CSharp" || asm == "UnityEngine"))
                asm = stack.GetFrame(2).GetMethod().DeclaringType.Assembly.GetName().Name;
            if (asm == "Assembly-CSharp" || asm == "UnityEngine")
                asm = "Game";

            log = "[LOG] " + asm + " >> " + log;
            File.AppendAllText(LogPath, log.ToString() + Environment.NewLine);
            if (inConsole)
                PointBlankConsole.WriteLine(log);
        }

        /// <summary>
        /// Logs an error into the logs file and console
        /// </summary>
        /// <param name="log">Object/Text to log</param>
        /// <param name="ex">Exception to log</param>
        /// <param name="exInConsole">Should the exception show in the console</param>
        /// <param name="inConsole">Should the text be printed into the console</param>
        public static void LogError(object log, Exception ex, bool exInConsole = false, bool inConsole = true)
        {
            StackTrace stack = new StackTrace(false);
            string asm = "";

            asm = stack.FrameCount > 0 ? stack.GetFrame(1).GetMethod().DeclaringType.Assembly.GetName().Name : "Not Found";

            if (stack.FrameCount > 1 && (asm == "PointBlank" || asm == "Assembly-CSharp" || asm == "UnityEngine"))
                asm = stack.GetFrame(2).GetMethod().DeclaringType.Assembly.GetName().Name;
            if (asm == "Assembly-CSharp" || asm == "UnityEngine")
                asm = "Game";

            log = "[ERROR] " + asm + " >> " + log;
            File.AppendAllText(LogPath, log.ToString() + Environment.NewLine);
            File.AppendAllText(LogPath, ex.ToString() + Environment.NewLine);
            if (inConsole)
                PointBlankConsole.WriteLine(log, ConsoleColor.Red);
            if (exInConsole)
                PointBlankConsole.WriteLine(ex, ConsoleColor.Red);
        }

        /// <summary>
        /// Logs a warning into the logs file and console
        /// </summary>
        /// <param name="log">Object/Text to log</param>
        /// <param name="inConsole">Should the text be printed into the console</param>
        public static void LogWarning(object log, bool inConsole = true)
        {
            StackTrace stack = new StackTrace(false);
            string asm = "";

            asm = stack.FrameCount > 0 ? stack.GetFrame(1).GetMethod().DeclaringType.Assembly.GetName().Name : "Not Found";

            if (stack.FrameCount > 1 && (asm == "PointBlank" || asm == "Assembly-CSharp" || asm == "UnityEngine"))
                asm = stack.GetFrame(2).GetMethod().DeclaringType.Assembly.GetName().Name;
            if (asm == "Assembly-CSharp" || asm == "UnityEngine")
                asm = "Game";

            log = "[WARNING] " + asm + " >> " + log;
            File.AppendAllText(LogPath, log.ToString() + Environment.NewLine);
            if (inConsole)
                PointBlankConsole.WriteLine(log, ConsoleColor.Yellow);
        }

        /// <summary>
        /// Logs important text into the logs file and console
        /// </summary>
        /// <param name="log">Object/Text to log</param>
        /// <param name="inConsole">Should the text be printed into the console</param>
        public static void LogImportant(object log, bool inConsole = true)
        {
            StackTrace stack = new StackTrace(false);
            string asm = "";

            asm = stack.FrameCount > 0 ? stack.GetFrame(1).GetMethod().DeclaringType.Assembly.GetName().Name : "Not Found";

            if (stack.FrameCount > 1 && (asm == "PointBlank" || asm == "Assembly-CSharp" || asm == "UnityEngine"))
                asm = stack.GetFrame(2).GetMethod().DeclaringType.Assembly.GetName().Name;
            if (asm == "Assembly-CSharp" || asm == "UnityEngine")
                asm = "Game";

            log = "[IMPORTANT] " + asm + " >> " + log;
            File.AppendAllText(LogPath, log.ToString() + Environment.NewLine);
            if (inConsole)
                PointBlankConsole.WriteLine(log, ConsoleColor.Cyan);
        }
    }
}
