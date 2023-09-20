using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.Library
{
	public static class CommandLineFunctionality
	{
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

		public static List<string> CollectCommandLineFunctions()
		{
			List<string> list = new List<string>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (CommandLineFunctionality.CheckAssemblyReferencesThis(assembly))
				{
					foreach (Type type in assembly.GetTypesSafe(null))
					{
						foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
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

		public static bool HasFunctionForCommand(string command)
		{
			CommandLineFunctionality.CommandLineFunction commandLineFunction;
			return CommandLineFunctionality.AllFunctions.TryGetValue(command, out commandLineFunction);
		}

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

		public static string CallFunction(string concatName, List<string> argList, out bool found)
		{
			CommandLineFunctionality.CommandLineFunction commandLineFunction;
			if (CommandLineFunctionality.AllFunctions.TryGetValue(concatName, out commandLineFunction))
			{
				found = true;
				return commandLineFunction.Call(argList);
			}
			found = false;
			return "Could not find the command " + concatName;
		}

		private static Dictionary<string, CommandLineFunctionality.CommandLineFunction> AllFunctions = new Dictionary<string, CommandLineFunctionality.CommandLineFunction>();

		private class CommandLineFunction
		{
			public CommandLineFunction(Func<List<string>, string> commandlinefunc)
			{
				this.CommandLineFunc = commandlinefunc;
				this.Children = new List<CommandLineFunctionality.CommandLineFunction>();
			}

			public string Call(List<string> objects)
			{
				return this.CommandLineFunc(objects);
			}

			public Func<List<string>, string> CommandLineFunc;

			public List<CommandLineFunctionality.CommandLineFunction> Children;
		}

		public class CommandLineArgumentFunction : Attribute
		{
			public CommandLineArgumentFunction(string name, string groupname)
			{
				this.Name = name;
				this.GroupName = groupname;
			}

			public string Name;

			public string GroupName;
		}
	}
}
