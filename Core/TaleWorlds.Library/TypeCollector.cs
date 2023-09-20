using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.Library
{
	// Token: 0x02000095 RID: 149
	public class TypeCollector<T> where T : class
	{
		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000504 RID: 1284 RVA: 0x0000FEBC File Offset: 0x0000E0BC
		// (set) Token: 0x06000505 RID: 1285 RVA: 0x0000FEC4 File Offset: 0x0000E0C4
		public Type BaseType { get; private set; }

		// Token: 0x06000506 RID: 1286 RVA: 0x0000FECD File Offset: 0x0000E0CD
		public TypeCollector()
		{
			this.BaseType = typeof(T);
			this._types = new Dictionary<string, Type>();
			this._currentAssembly = this.BaseType.Assembly;
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x0000FF04 File Offset: 0x0000E104
		public void Collect()
		{
			List<Type> list = this.CollectTypes();
			this._types.Clear();
			foreach (Type type in list)
			{
				this._types.Add(type.Name, type);
			}
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x0000FF70 File Offset: 0x0000E170
		public T Instantiate(string typeName, params object[] parameters)
		{
			T t = default(T);
			Type type;
			if (this._types.TryGetValue(typeName, out type))
			{
				t = (T)((object)type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, new Type[0], null).Invoke(parameters));
			}
			return t;
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x0000FFB8 File Offset: 0x0000E1B8
		public Type GetType(string typeName)
		{
			Type type;
			if (this._types.TryGetValue(typeName, out type))
			{
				return type;
			}
			return null;
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x0000FFD8 File Offset: 0x0000E1D8
		private bool CheckAssemblyReferencesThis(Assembly assembly)
		{
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			for (int i = 0; i < referencedAssemblies.Length; i++)
			{
				if (referencedAssemblies[i].Name == this._currentAssembly.GetName().Name)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x0001001C File Offset: 0x0000E21C
		private List<Type> CollectTypes()
		{
			List<Type> list = new List<Type>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (this.CheckAssemblyReferencesThis(assembly) || assembly == this._currentAssembly)
				{
					foreach (Type type in assembly.GetTypes())
					{
						if (this.BaseType.IsAssignableFrom(type))
						{
							list.Add(type);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x0400017F RID: 383
		private Dictionary<string, Type> _types;

		// Token: 0x04000180 RID: 384
		private Assembly _currentAssembly;
	}
}
