using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks
{
	internal static class PerkAssemblyCollection
	{
		public static List<Type> GetPerkAssemblyTypes()
		{
			List<Type> list = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			List<Assembly> list2 = new List<Assembly>();
			foreach (Assembly assembly in assemblies)
			{
				try
				{
					if (PerkAssemblyCollection.CheckAssemblyForPerks(assembly))
					{
						list2.Add(assembly);
					}
				}
				catch
				{
				}
			}
			foreach (Assembly assembly2 in list2)
			{
				try
				{
					List<Type> typesSafe = assembly2.GetTypesSafe(null);
					list.AddRange(typesSafe);
				}
				catch
				{
				}
			}
			return list;
		}

		private static bool CheckAssemblyForPerks(Assembly assembly)
		{
			Assembly assembly2 = Assembly.GetAssembly(typeof(MPPerkObject));
			if (assembly == assembly2)
			{
				return true;
			}
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			for (int i = 0; i < referencedAssemblies.Length; i++)
			{
				if (referencedAssemblies[i].FullName == assembly2.FullName)
				{
					return true;
				}
			}
			return false;
		}
	}
}
