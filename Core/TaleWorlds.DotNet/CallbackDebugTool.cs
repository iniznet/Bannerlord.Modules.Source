using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TaleWorlds.DotNet
{
	public static class CallbackDebugTool
	{
		[Conditional("DEBUG")]
		public static void AddLog([CallerMemberName] string memberName = "")
		{
			Dictionary<string, CallbackDebugTool.CallbackLog> logs = CallbackDebugTool.Logs;
			lock (logs)
			{
				CallbackDebugTool.CallbackLog callbackLog;
				if (CallbackDebugTool.Logs.TryGetValue(memberName, out callbackLog))
				{
					callbackLog.CallCount += 1L;
				}
				else
				{
					CallbackDebugTool.Logs.Add(memberName, new CallbackDebugTool.CallbackLog
					{
						CallCount = 1L,
						FuncName = memberName
					});
				}
			}
		}

		[Conditional("DEBUG")]
		public static void FrameEnd()
		{
			CallbackDebugTool.FrameCount += 1UL;
		}

		[Conditional("DEBUG")]
		public static void Reset()
		{
			CallbackDebugTool.Logs = new Dictionary<string, CallbackDebugTool.CallbackLog>();
			CallbackDebugTool.FrameCount = 0UL;
		}

		public static string ShowResults()
		{
			List<CallbackDebugTool.CallbackLog> list = CallbackDebugTool.Logs.Values.ToList<CallbackDebugTool.CallbackLog>();
			list.Sort((CallbackDebugTool.CallbackLog x, CallbackDebugTool.CallbackLog y) => (int)(y.CallCount - x.CallCount));
			string text = "";
			double num = 1.0 / CallbackDebugTool.FrameCount;
			foreach (CallbackDebugTool.CallbackLog callbackLog in list)
			{
				double num2 = (double)callbackLog.CallCount * num;
				text = string.Concat(new object[]
				{
					text,
					callbackLog.FuncName,
					": ",
					callbackLog.CallCount,
					", ",
					num2,
					Environment.NewLine
				});
			}
			return text;
		}

		private static Dictionary<string, CallbackDebugTool.CallbackLog> Logs = new Dictionary<string, CallbackDebugTool.CallbackLog>();

		private static ulong FrameCount = 0UL;

		private class CallbackLog
		{
			public long CallCount;

			public string FuncName;
		}
	}
}
