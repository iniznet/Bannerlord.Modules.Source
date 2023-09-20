using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.Library
{
	public static class AssemblyLoader
	{
		static AssemblyLoader()
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssemblyLoader._loadedAssemblies.Add(assembly);
			}
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyLoader.OnAssemblyResolve;
		}

		public static void Initialize()
		{
		}

		public static Assembly LoadFrom(string assemblyFile, bool show_error = true)
		{
			Assembly assembly = null;
			Debug.Print("Loading assembly: " + assemblyFile + "\n", 0, Debug.DebugColor.White, 17592186044416UL);
			try
			{
				if (ApplicationPlatform.CurrentRuntimeLibrary == Runtime.DotNetCore)
				{
					try
					{
						assembly = Assembly.LoadFrom(assemblyFile);
					}
					catch (Exception)
					{
						assembly = null;
					}
					if (assembly != null && !AssemblyLoader._loadedAssemblies.Contains(assembly))
					{
						AssemblyLoader._loadedAssemblies.Add(assembly);
						AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
						for (int i = 0; i < referencedAssemblies.Length; i++)
						{
							string text = referencedAssemblies[i].Name + ".dll";
							if (!text.StartsWith("System") && !text.StartsWith("mscorlib") && !text.StartsWith("netstandard"))
							{
								AssemblyLoader.LoadFrom(text, true);
							}
						}
					}
				}
				else
				{
					assembly = Assembly.LoadFrom(assemblyFile);
				}
			}
			catch
			{
				if (show_error)
				{
					string text2 = "Cannot load: " + assemblyFile;
					string text3 = "ERROR";
					Debug.ShowMessageBox(text2, text3, 4U);
				}
			}
			Debug.Print("Assembly load result: " + ((assembly == null) ? "NULL" : "SUCCESS"), 0, Debug.DebugColor.White, 17592186044416UL);
			return assembly;
		}

		private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.FullName == args.Name)
				{
					return assembly;
				}
			}
			if (ApplicationPlatform.CurrentRuntimeLibrary == Runtime.Mono && ApplicationPlatform.IsPlatformWindows())
			{
				return AssemblyLoader.LoadFrom(args.Name.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0] + ".dll", false);
			}
			return null;
		}

		private static List<Assembly> _loadedAssemblies = new List<Assembly>();
	}
}
