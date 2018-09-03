// MIT License
// 
// Copyright(c) 2018 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Glaives.GameFramework;

namespace Glaives.Diagnostics
{
    public class Debugger
    {
        private readonly GameInstance _gameInstance;
        private readonly List<string> _issueLogs = new List<string>();

        internal Debugger(GameInstance gameInstance)
        {
            _gameInstance = gameInstance;
            AppDomain.CurrentDomain.ProcessExit += SerializeLogs;
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
        }

        /// <summary>
        /// <para>Log an information message (something non-problematic)</para>
        /// <para>Will not be included in the error log file</para>
        /// </summary>
        /// <param name="infoMessage"></param>
        public void Info(object infoMessage)
        {
            WriteLog(LogType.Info, infoMessage);
        }

        /// <summary>
        /// <para>Log a warning message (something potentially problematic)</para>
        /// <para>Will be included in the error log file</para>
        /// </summary>
        /// <param name="warningMessage"></param>
        public void Warning(object warningMessage)
        {
            WriteLog(LogType.Warning, warningMessage);
        }

        /// <summary>
        /// <para>Log an error message (something critically problematic)</para>
        /// <para>Will be included in the error log file</para>
        /// </summary>
        /// <param name="errorMessage"></param>
        public void Error(object errorMessage)
        {
            WriteLog(LogType.Error, errorMessage);
        }

        private void OnFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            WriteLog(LogType.Error, $"{e.Exception}: '{e.Exception.Message}'");
            SerializeLogs(sender, e);
        }

        private void SerializeLogs(object sender, EventArgs e)
        {
            if (_issueLogs.Count == 0)
            {
                return;
            }

            if (!Directory.Exists("ErrorLogs"))
            {
                Directory.CreateDirectory("ErrorLogs");
            }

            DateTime now = DateTime.Now;
            string logFile = $"ErrorLogs/{now.Year}-{now.Month}-{now.Day}_{now.Hour}-{now.Minute}-{now.Second}.txt";
            File.Create(logFile).Close();
            File.WriteAllLines(logFile, _issueLogs);
        }

        private void WriteLog(LogType logType, object logMsg)
        {
            StackTrace stack = new StackTrace(true);
            StackFrame lastFrame = stack.GetFrame(2);
            MethodBase method = lastFrame.GetMethod();
            string className = method.DeclaringType?.ToString();
            string methodName = method.Name;
            int line = lastFrame.GetFileLineNumber();
            _gameInstance.LogInternal(logType, logMsg.ToString(), className, methodName, line);

            if (logType == LogType.Error || logType == LogType.Warning)
            {
                _issueLogs.Add($"[{logType}] {logMsg} <{className}.{methodName} ({line})>");
            }
        }

        
    }
}
