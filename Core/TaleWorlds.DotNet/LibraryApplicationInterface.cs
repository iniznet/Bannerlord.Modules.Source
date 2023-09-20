using System;
using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
	internal class LibraryApplicationInterface
	{
		private static T GetObject<T>() where T : class
		{
			object obj;
			if (LibraryApplicationInterface._objects.TryGetValue(typeof(T).FullName, out obj))
			{
				return obj as T;
			}
			return default(T);
		}

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

		internal static IManaged IManaged;

		internal static ITelemetry ITelemetry;

		internal static ILibrarySizeChecker ILibrarySizeChecker;

		internal static INativeArray INativeArray;

		internal static INativeObjectArray INativeObjectArray;

		internal static INativeStringHelper INativeStringHelper;

		internal static INativeString INativeString;

		private static Dictionary<string, object> _objects;
	}
}
