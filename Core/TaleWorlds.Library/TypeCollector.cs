using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.Library
{
	public class TypeCollector<T> where T : class
	{
		public Type BaseType { get; private set; }

		public TypeCollector()
		{
			this.BaseType = typeof(T);
			this._types = new Dictionary<string, Type>();
			this._currentAssembly = this.BaseType.Assembly;
		}

		public void Collect()
		{
			List<Type> list = this.CollectTypes();
			this._types.Clear();
			foreach (Type type in list)
			{
				this._types.Add(type.Name, type);
			}
		}

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

		public Type GetType(string typeName)
		{
			Type type;
			if (this._types.TryGetValue(typeName, out type))
			{
				return type;
			}
			return null;
		}

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

		private List<Type> CollectTypes()
		{
			List<Type> list = new List<Type>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (this.CheckAssemblyReferencesThis(assembly) || assembly == this._currentAssembly)
				{
					foreach (Type type in assembly.GetTypesSafe(null))
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

		private Dictionary<string, Type> _types;

		private Assembly _currentAssembly;
	}
}
