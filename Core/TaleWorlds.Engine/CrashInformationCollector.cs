using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200000F RID: 15
	public static class CrashInformationCollector
	{
		// Token: 0x06000067 RID: 103 RVA: 0x00002BC4 File Offset: 0x00000DC4
		[EngineCallback]
		public static string CollectInformation()
		{
			List<CrashInformationCollector.CrashInformation> list = new List<CrashInformationCollector.CrashInformation>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					Type[] types = assembly.GetTypes();
					for (int j = 0; j < types.Length; j++)
					{
						foreach (MethodInfo methodInfo in types[j].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
						{
							object[] customAttributes = methodInfo.GetCustomAttributes(typeof(CrashInformationCollector.CrashInformationProvider), false);
							if (customAttributes != null && customAttributes.Length != 0 && customAttributes[0] is CrashInformationCollector.CrashInformationProvider)
							{
								CrashInformationCollector.CrashInformation crashInformation = methodInfo.Invoke(null, new object[0]) as CrashInformationCollector.CrashInformation;
								if (crashInformation != null)
								{
									list.Add(crashInformation);
								}
							}
						}
					}
				}
				catch (ReflectionTypeLoadException ex)
				{
					foreach (Exception ex2 in ex.LoaderExceptions)
					{
						MBDebug.Print("Unable to load types from assembly: " + ex2.Message, 0, Debug.DebugColor.White, 17592186044416UL);
					}
				}
				catch (Exception ex3)
				{
					MBDebug.Print("Exception while collecting crash information : " + ex3.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				}
			}
			string text = "";
			foreach (CrashInformationCollector.CrashInformation crashInformation2 in list)
			{
				foreach (ValueTuple<string, string> valueTuple in crashInformation2.Lines)
				{
					text = string.Concat(new string[] { text, "[", crashInformation2.Id, "][", valueTuple.Item1, "][", valueTuple.Item2, "]\n" });
				}
			}
			return text;
		}

		// Token: 0x020000A7 RID: 167
		public class CrashInformation
		{
			// Token: 0x06000C1B RID: 3099 RVA: 0x0000E9D7 File Offset: 0x0000CBD7
			public CrashInformation(string id, MBReadOnlyList<ValueTuple<string, string>> lines)
			{
				this.Id = id;
				this.Lines = lines;
			}

			// Token: 0x0400032B RID: 811
			public readonly string Id;

			// Token: 0x0400032C RID: 812
			public readonly MBReadOnlyList<ValueTuple<string, string>> Lines;
		}

		// Token: 0x020000A8 RID: 168
		[AttributeUsage(AttributeTargets.Method)]
		public class CrashInformationProvider : Attribute
		{
		}
	}
}
