using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public static class CrashInformationCollector
	{
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

		public class CrashInformation
		{
			public CrashInformation(string id, MBReadOnlyList<ValueTuple<string, string>> lines)
			{
				this.Id = id;
				this.Lines = lines;
			}

			public readonly string Id;

			public readonly MBReadOnlyList<ValueTuple<string, string>> Lines;
		}

		[AttributeUsage(AttributeTargets.Method)]
		public class CrashInformationProvider : Attribute
		{
		}
	}
}
