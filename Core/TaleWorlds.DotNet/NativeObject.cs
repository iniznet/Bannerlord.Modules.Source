using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000026 RID: 38
	public abstract class NativeObject
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x00004BC7 File Offset: 0x00002DC7
		// (set) Token: 0x060000E8 RID: 232 RVA: 0x00004BCF File Offset: 0x00002DCF
		public UIntPtr Pointer { get; private set; }

		// Token: 0x060000EA RID: 234 RVA: 0x00004BE0 File Offset: 0x00002DE0
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

		// Token: 0x060000EB RID: 235 RVA: 0x00004C44 File Offset: 0x00002E44
		~NativeObject()
		{
			if (!this._manualInvalidated)
			{
				LibraryApplicationInterface.IManaged.DecreaseReferenceCount(this.Pointer);
			}
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00004C84 File Offset: 0x00002E84
		public void ManualInvalidate()
		{
			if (!this._manualInvalidated)
			{
				LibraryApplicationInterface.IManaged.DecreaseReferenceCount(this.Pointer);
				this._manualInvalidated = true;
			}
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00004CA8 File Offset: 0x00002EA8
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

		// Token: 0x060000EE RID: 238 RVA: 0x00004E60 File Offset: 0x00003060
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

		// Token: 0x060000EF RID: 239 RVA: 0x00004EE0 File Offset: 0x000030E0
		[LibraryCallback]
		internal static int GetAliveNativeObjectCount()
		{
			return NativeObject._numberOfAliveNativeObjects;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00004EE9 File Offset: 0x000030E9
		[LibraryCallback]
		internal static string GetAliveNativeObjectNames()
		{
			return "";
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00004EF0 File Offset: 0x000030F0
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

		// Token: 0x060000F2 RID: 242 RVA: 0x00004F58 File Offset: 0x00003158
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

		// Token: 0x060000F3 RID: 243 RVA: 0x00004FC5 File Offset: 0x000031C5
		[Obsolete]
		protected void AddUnmanagedMemoryPressure(int size)
		{
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00004FC8 File Offset: 0x000031C8
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

		// Token: 0x060000F5 RID: 245 RVA: 0x00005022 File Offset: 0x00003222
		internal static T CreateNativeObjectWrapper<T>(NativeObjectPointer nativeObjectPointer) where T : NativeObject
		{
			return (T)((object)NativeObject.CreateNativeObjectWrapper(nativeObjectPointer));
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00005030 File Offset: 0x00003230
		public override int GetHashCode()
		{
			return this.Pointer.GetHashCode();
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000504B File Offset: 0x0000324B
		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && ((NativeObject)obj).Pointer == this.Pointer;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0000507D File Offset: 0x0000327D
		public static bool operator ==(NativeObject a, NativeObject b)
		{
			return a == b || (a != null && b != null && a.Equals(b));
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00005094 File Offset: 0x00003294
		public static bool operator !=(NativeObject a, NativeObject b)
		{
			return !(a == b);
		}

		// Token: 0x04000058 RID: 88
		private static List<EngineClassTypeDefinition> _typeDefinitions;

		// Token: 0x04000059 RID: 89
		private static List<ConstructorInfo> _constructors;

		// Token: 0x0400005A RID: 90
		private const int NativeObjectFirstReferencesTickCount = 10;

		// Token: 0x0400005B RID: 91
		private static List<List<NativeObject>> _nativeObjectFirstReferences;

		// Token: 0x0400005C RID: 92
		private static volatile int _numberOfAliveNativeObjects;

		// Token: 0x0400005D RID: 93
		private bool _manualInvalidated;
	}
}
