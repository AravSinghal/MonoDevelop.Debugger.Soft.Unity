// 
// UnitySoftDebuggerEngine.cs
//   based on MoonlightSoftDebuggerEngine.cs
//  
// Author:
//       Michael Hutchinson <mhutchinson@novell.com>
//       Lucas Meijer <lucas@unity3d.com>
// 
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


using System;
using System.Linq;
using Mono.Debugging.Client;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Debugger;
using MonoDevelop.Ide;

namespace MonoDevelop.Debugger.Soft.Unity
{
	public class UnitySoftDebuggerEngine: DebuggerEngineBackend
	{
		UnitySoftDebuggerSession session;

		class DebuggerLogger : Log.ILogger
		{
			public void Info(string message)
			{
				DebuggerLoggingService.LogMessage (message);
			}

			public void Warning (string message, Exception e)
			{
				DebuggerLoggingService.LogMessage ("Warning: " + message + "\n" + e);
			}

			public void Error (string message, Exception e)
			{
				DebuggerLoggingService.LogError (message, e);
			}
		}

		class MDLogger : ICustomLogger
		{
			public string GetNewDebuggerLogFilename ()
			{
				string filename;
				var logWriter = LoggingService.CreateLogFile ("Unity-Debugger", out filename);
				logWriter.Dispose ();
				return filename;
			}

			public void LogError (string message, Exception ex)
			{
				LoggingService.LogError (message, ex);
			}

			public void LogAndShowException (string message, Exception ex)
			{
				MessageService.ShowError (message, ex);
			}

			public void LogMessage (string messageFormat, params object[] args)
			{
				LoggingService.LogInfo (messageFormat, args);
			}
		}

		public UnitySoftDebuggerEngine()
		{
			Log.AddLogger (new DebuggerLogger ());
			DebuggerLoggingService.CustomLogger = new MDLogger ();
			UnityProcessDiscovery.Init ();
		}

		public override bool CanDebugCommand (ExecutionCommand cmd)
		{
			return cmd is UnityExecutionCommand;
		}

		public override bool IsDefaultDebugger (ExecutionCommand cmd)
		{
			return cmd is UnityExecutionCommand;
		}

		public override DebuggerStartInfo CreateDebuggerStartInfo (ExecutionCommand command)
		{
			return null;
		}
			
		public override DebuggerSession CreateSession ()
		{
			session = new UnitySoftDebuggerSession ();
			session.TargetExited += delegate{ session = null; };
			return session;
		}
		
		public override ProcessInfo[] GetAttachableProcesses ()
		{
			return UnityProcessDiscovery.GetAttachableProcessesAsync ().Select (p => new ProcessInfo (p.Id, p.Name)).ToArray();
		}

		public string Name {
			get {
				return "Mono Soft Debugger for Unity";
			}
		}
	}

	class UnityExecutionCommand : ExecutionCommand
	{
	};
}
