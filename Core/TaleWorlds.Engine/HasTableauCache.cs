using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.Engine
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class HasTableauCache : Attribute
	{
		public Type TableauCacheType { get; set; }

		public Type MaterialCacheIDGetType { get; set; }

		internal static Dictionary<Type, MaterialCacheIDGetMethodDelegate> TableauCacheTypes { get; private set; }

		public HasTableauCache(Type tableauCacheType, Type materialCacheIDGetType)
		{
			this.TableauCacheType = tableauCacheType;
			this.MaterialCacheIDGetType = materialCacheIDGetType;
		}

		public static void CollectTableauCacheTypes()
		{
			HasTableauCache.TableauCacheTypes = new Dictionary<Type, MaterialCacheIDGetMethodDelegate>();
			HasTableauCache.CollectTableauCacheTypesFrom(typeof(HasTableauCache).Assembly);
			Assembly[] viewAssemblies = HasTableauCache.GetViewAssemblies();
			for (int i = 0; i < viewAssemblies.Length; i++)
			{
				HasTableauCache.CollectTableauCacheTypesFrom(viewAssemblies[i]);
			}
		}

		private static void CollectTableauCacheTypesFrom(Assembly assembly)
		{
			object[] customAttributes = assembly.GetCustomAttributes(typeof(HasTableauCache), true);
			if (customAttributes.Length != 0)
			{
				foreach (HasTableauCache hasTableauCache in customAttributes)
				{
					MethodInfo method = hasTableauCache.MaterialCacheIDGetType.GetMethod("GetMaterialCacheID", BindingFlags.Static | BindingFlags.Public);
					MaterialCacheIDGetMethodDelegate materialCacheIDGetMethodDelegate = (MaterialCacheIDGetMethodDelegate)Delegate.CreateDelegate(typeof(MaterialCacheIDGetMethodDelegate), method);
					HasTableauCache.TableauCacheTypes.Add(hasTableauCache.TableauCacheType, materialCacheIDGetMethodDelegate);
				}
			}
		}

		private static Assembly[] GetViewAssemblies()
		{
			List<Assembly> list = new List<Assembly>();
			Assembly assembly = typeof(HasTableauCache).Assembly;
			foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssemblyName[] referencedAssemblies = assembly2.GetReferencedAssemblies();
				for (int j = 0; j < referencedAssemblies.Length; j++)
				{
					if (referencedAssemblies[j].ToString() == assembly.GetName().ToString())
					{
						list.Add(assembly2);
						break;
					}
				}
			}
			return list.ToArray();
		}
	}
}
