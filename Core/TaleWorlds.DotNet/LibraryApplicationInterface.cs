using System;
using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000014 RID: 20
	internal class LibraryApplicationInterface
	{
		// Token: 0x06000047 RID: 71 RVA: 0x00002D50 File Offset: 0x00000F50
		private static T GetObject<T>() where T : class
		{
			object obj;
			if (LibraryApplicationInterface._objects.TryGetValue(typeof(T).FullName, out obj))
			{
				return obj as T;
			}
			return default(T);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002D90 File Offset: 0x00000F90
		internal static void SetObjects(Dictionary<string, object> objects)
		{
			LibraryApplicationInterface._objects = objects;
			LibraryApplicationInterface.IManaged = LibraryApplicationInterface.GetObject<IManaged>();
			LibraryApplicationInterface.ITelemetry = LibraryApplicationInterface.GetObject<ITelemetry>();
			LibraryApplicationInterface.ILibrarySizeChecker = LibraryApplicationInterface.GetObject<ILibrarySizeChecker>();
			LibraryApplicationInterface.INativeArray = LibraryApplicationInterface.GetObject<INativeArray>();
			LibraryApplicationInterface.INativeObjectArray = LibraryApplicationInterface.GetObject<INativeObjectArray>();
			LibraryApplicationInterface.INativeStringHelper = LibraryApplicationInterface.GetObject<INativeStringHelper>();
			LibraryApplicationInterface.INativeString = LibraryApplicationInterface.GetObject<INativeString>();
		}

		// Token: 0x04000026 RID: 38
		internal static IManaged IManaged;

		// Token: 0x04000027 RID: 39
		internal static ITelemetry ITelemetry;

		// Token: 0x04000028 RID: 40
		internal static ILibrarySizeChecker ILibrarySizeChecker;

		// Token: 0x04000029 RID: 41
		internal static INativeArray INativeArray;

		// Token: 0x0400002A RID: 42
		internal static INativeObjectArray INativeObjectArray;

		// Token: 0x0400002B RID: 43
		internal static INativeStringHelper INativeStringHelper;

		// Token: 0x0400002C RID: 44
		internal static INativeString INativeString;

		// Token: 0x0400002D RID: 45
		private static Dictionary<string, object> _objects;
	}
}
