using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.Engine
{
	// Token: 0x0200005A RID: 90
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class HasTableauCache : Attribute
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000782 RID: 1922 RVA: 0x00006E59 File Offset: 0x00005059
		// (set) Token: 0x06000783 RID: 1923 RVA: 0x00006E61 File Offset: 0x00005061
		public Type TableauCacheType { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000784 RID: 1924 RVA: 0x00006E6A File Offset: 0x0000506A
		// (set) Token: 0x06000785 RID: 1925 RVA: 0x00006E72 File Offset: 0x00005072
		public Type MaterialCacheIDGetType { get; set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000786 RID: 1926 RVA: 0x00006E7B File Offset: 0x0000507B
		// (set) Token: 0x06000787 RID: 1927 RVA: 0x00006E82 File Offset: 0x00005082
		internal static Dictionary<Type, MaterialCacheIDGetMethodDelegate> TableauCacheTypes { get; private set; }

		// Token: 0x06000788 RID: 1928 RVA: 0x00006E8A File Offset: 0x0000508A
		public HasTableauCache(Type tableauCacheType, Type materialCacheIDGetType)
		{
			this.TableauCacheType = tableauCacheType;
			this.MaterialCacheIDGetType = materialCacheIDGetType;
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x00006EA0 File Offset: 0x000050A0
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

		// Token: 0x0600078A RID: 1930 RVA: 0x00006EE8 File Offset: 0x000050E8
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

		// Token: 0x0600078B RID: 1931 RVA: 0x00006F64 File Offset: 0x00005164
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
