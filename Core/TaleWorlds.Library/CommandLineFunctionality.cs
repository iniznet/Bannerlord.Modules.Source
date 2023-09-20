using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.Library
{
	// Token: 0x02000020 RID: 32
	public static class CommandLineFunctionality
	{
		// Token: 0x060000B9 RID: 185 RVA: 0x000043E8 File Offset: 0x000025E8
		private static bool CheckAssemblyReferencesThis(Assembly assembly)
		{
			Assembly assembly2 = typeof(CommandLineFunctionality).Assembly;
			if (assembly2.GetName().Name == assembly.GetName().Name)
			{
				return true;
			}
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			for (int i = 0; i < referencedAssemblies.Length; i++)
			{
				if (referencedAssemblies[i].Name == assembly2.GetName().Name)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00004458 File Offset: 0x00002658
		public static List<string> CollectCommandLineFunctions()
		{
			List<string> list = new List<string>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (CommandLineFunctionality.CheckAssemblyReferencesThis(assembly))
				{
					Type[] types = assembly.GetTypes();
					for (int j = 0; j < types.Length; j++)
					{
						foreach (MethodInfo methodInfo in types[j].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
						{
							object[] customAttributes = methodInfo.GetCustomAttributes(typeof(CommandLineFunctionality.CommandLineArgumentFunction), false);
							if (customAttributes != null && customAttributes.Length != 0)
							{
								CommandLineFunctionality.CommandLineArgumentFunction commandLineArgumentFunction = customAttributes[0] as CommandLineFunctionality.CommandLineArgumentFunction;
								if (commandLineArgumentFunction != null && !(methodInfo.ReturnType != typeof(string)))
								{
									string name = commandLineArgumentFunction.Name;
									string text = commandLineArgumentFunction.GroupName + "." + name;
									list.Add(text);
									CommandLineFunctionality.CommandLineFunction commandLineFunction = new CommandLineFunctionality.CommandLineFunction((Func<List<string>, string>)Delegate.CreateDelegate(typeof(Func<List<string>, string>), methodInfo));
									CommandLineFunctionality.AllFunctions.Add(text, commandLineFunction);
								}
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00004580 File Offset: 0x00002780
		public static bool HasFunctionForCommand(string command)
		{
			CommandLineFunctionality.CommandLineFunction commandLineFunction;
			return CommandLineFunctionality.AllFunctions.TryGetValue(command, out commandLineFunction);
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000459C File Offset: 0x0000279C
		public static string CallFunction(string concatName, string concatArguments, out bool found)
		{
			CommandLineFunctionality.CommandLineFunction commandLineFunction;
			if (CommandLineFunctionality.AllFunctions.TryGetValue(concatName, out commandLineFunction))
			{
				List<string> list;
				if (concatArguments != string.Empty)
				{
					list = new List<string>(concatArguments.Split(new char[] { ' ' }));
				}
				else
				{
					list = new List<string>();
				}
				found = true;
				return commandLineFunction.Call(list);
			}
			found = false;
			return "Could not find the command " + concatName;
		}

		// Token: 0x04000067 RID: 103
		private static Dictionary<string, CommandLineFunctionality.CommandLineFunction> AllFunctions = new Dictionary<string, CommandLineFunctionality.CommandLineFunction>();

		// Token: 0x020000BF RID: 191
		private class CommandLineFunction
		{
			// Token: 0x060006B7 RID: 1719 RVA: 0x0001485C File Offset: 0x00012A5C
			public CommandLineFunction(Func<List<string>, string> commandlinefunc)
			{
				this.CommandLineFunc = commandlinefunc;
				this.Children = new List<CommandLineFunctionality.CommandLineFunction>();
			}

			// Token: 0x060006B8 RID: 1720 RVA: 0x00014876 File Offset: 0x00012A76
			public string Call(List<string> objects)
			{
				return this.CommandLineFunc(objects);
			}

			// Token: 0x0400021F RID: 543
			public Func<List<string>, string> CommandLineFunc;

			// Token: 0x04000220 RID: 544
			public List<CommandLineFunctionality.CommandLineFunction> Children;
		}

		// Token: 0x020000C0 RID: 192
		public class CommandLineArgumentFunction : Attribute
		{
			// Token: 0x060006B9 RID: 1721 RVA: 0x00014884 File Offset: 0x00012A84
			public CommandLineArgumentFunction(string name, string groupname)
			{
				this.Name = name;
				this.GroupName = groupname;
			}

			// Token: 0x04000221 RID: 545
			public string Name;

			// Token: 0x04000222 RID: 546
			public string GroupName;
		}
	}
}
