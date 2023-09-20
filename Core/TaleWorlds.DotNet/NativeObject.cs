using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.DotNet
{
	public abstract class NativeObject
	{
		public UIntPtr Pointer { get; private set; }

		internal void Construct(UIntPtr pointer)
		{
			this.Pointer = pointer;
			LibraryApplicationInterface.IManaged.IncreaseReferenceCount(this.Pointer);
			List<List<NativeObject>> nativeObjectFirstReferences = NativeObject._nativeObjectFirstReferences;
			lock (nativeObjectFirstReferences)
			{
				NativeObject._nativeObjectFirstReferences[0].Add(this);
			}
		}

		~NativeObject()
		{
			if (!this._manualInvalidated)
			{
				LibraryApplicationInterface.IManaged.DecreaseReferenceCount(this.Pointer);
			}
		}

		public void ManualInvalidate()
		{
			if (!this._manualInvalidated)
			{
				LibraryApplicationInterface.IManaged.DecreaseReferenceCount(this.Pointer);
				this._manualInvalidated = true;
			}
		}

		static NativeObject()
		{
			int classTypeDefinitionCount = LibraryApplicationInterface.IManaged.GetClassTypeDefinitionCount();
			NativeObject._typeDefinitions = new List<EngineClassTypeDefinition>();
			NativeObject._constructors = new List<ConstructorInfo>();
			for (int i = 0; i < classTypeDefinitionCount; i++)
			{
				EngineClassTypeDefinition engineClassTypeDefinition = default(EngineClassTypeDefinition);
				LibraryApplicationInterface.IManaged.GetClassTypeDefinition(i, ref engineClassTypeDefinition);
				NativeObject._typeDefinitions.Add(engineClassTypeDefinition);
				NativeObject._constructors.Add(null);
			}
			List<Type> list = new List<Type>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					if (NativeObject.DoesNativeObjectDefinedAssembly(assembly))
					{
						foreach (Type type in assembly.GetTypes())
						{
							if (type.GetCustomAttributes(typeof(EngineClass), false).Length == 1)
							{
								list.Add(type);
							}
						}
					}
				}
				catch (Exception)
				{
				}
			}
			foreach (Type type2 in list)
			{
				EngineClass engineClass = (EngineClass)type2.GetCustomAttributes(typeof(EngineClass), false)[0];
				if (!type2.IsAbstract)
				{
					ConstructorInfo constructor = type2.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(UIntPtr) }, null);
					int typeDefinitionId = NativeObject.GetTypeDefinitionId(engineClass.EngineType);
					if (typeDefinitionId != -1)
					{
						NativeObject._constructors[typeDefinitionId] = constructor;
					}
				}
			}
			NativeObject._nativeObjectFirstReferences = new List<List<NativeObject>>();
			for (int l = 0; l < 10; l++)
			{
				NativeObject._nativeObjectFirstReferences.Add(new List<NativeObject>());
			}
		}

		internal static void HandleNativeObjects()
		{
			List<List<NativeObject>> nativeObjectFirstReferences = NativeObject._nativeObjectFirstReferences;
			lock (nativeObjectFirstReferences)
			{
				List<NativeObject> list = NativeObject._nativeObjectFirstReferences[9];
				for (int i = 9; i > 0; i--)
				{
					NativeObject._nativeObjectFirstReferences[i] = NativeObject._nativeObjectFirstReferences[i - 1];
				}
				list.Clear();
				NativeObject._nativeObjectFirstReferences[0] = list;
			}
		}

		[LibraryCallback]
		internal static int GetAliveNativeObjectCount()
		{
			return NativeObject._numberOfAliveNativeObjects;
		}

		[LibraryCallback]
		internal static string GetAliveNativeObjectNames()
		{
			return "";
		}

		private static int GetTypeDefinitionId(string typeName)
		{
			foreach (EngineClassTypeDefinition engineClassTypeDefinition in NativeObject._typeDefinitions)
			{
				if (engineClassTypeDefinition.TypeName == typeName)
				{
					return engineClassTypeDefinition.TypeId;
				}
			}
			return -1;
		}

		private static bool DoesNativeObjectDefinedAssembly(Assembly assembly)
		{
			if (typeof(NativeObject).Assembly.FullName == assembly.FullName)
			{
				return true;
			}
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			string fullName = typeof(NativeObject).Assembly.FullName;
			AssemblyName[] array = referencedAssemblies;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].FullName == fullName)
				{
					return true;
				}
			}
			return false;
		}

		[Obsolete]
		protected void AddUnmanagedMemoryPressure(int size)
		{
		}

		internal static NativeObject CreateNativeObjectWrapper(NativeObjectPointer nativeObjectPointer)
		{
			if (nativeObjectPointer.Pointer != UIntPtr.Zero)
			{
				ConstructorInfo constructorInfo = NativeObject._constructors[nativeObjectPointer.TypeId];
				if (constructorInfo != null)
				{
					return (NativeObject)constructorInfo.Invoke(new object[] { nativeObjectPointer.Pointer });
				}
			}
			return null;
		}

		internal static T CreateNativeObjectWrapper<T>(NativeObjectPointer nativeObjectPointer) where T : NativeObject
		{
			return (T)((object)NativeObject.CreateNativeObjectWrapper(nativeObjectPointer));
		}

		public override int GetHashCode()
		{
			return this.Pointer.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && ((NativeObject)obj).Pointer == this.Pointer;
		}

		public static bool operator ==(NativeObject a, NativeObject b)
		{
			return a == b || (a != null && b != null && a.Equals(b));
		}

		public static bool operator !=(NativeObject a, NativeObject b)
		{
			return !(a == b);
		}

		private static List<EngineClassTypeDefinition> _typeDefinitions;

		private static List<ConstructorInfo> _constructors;

		private const int NativeObjectFirstReferencesTickCount = 10;

		private static List<List<NativeObject>> _nativeObjectFirstReferences;

		private static volatile int _numberOfAliveNativeObjects;

		private bool _manualInvalidated;
	}
}
