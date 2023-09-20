using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TaleWorlds.Library
{
	internal static class GCHandleFactory
	{
		static GCHandleFactory()
		{
			for (int i = 0; i < 512; i++)
			{
				GCHandleFactory._handles.Add(GCHandle.Alloc(null, GCHandleType.Pinned));
			}
		}

		public static GCHandle GetHandle()
		{
			object locker = GCHandleFactory._locker;
			lock (locker)
			{
				if (GCHandleFactory._handles.Count > 0)
				{
					GCHandle gchandle = GCHandleFactory._handles[GCHandleFactory._handles.Count - 1];
					GCHandleFactory._handles.RemoveAt(GCHandleFactory._handles.Count - 1);
					return gchandle;
				}
			}
			return GCHandle.Alloc(null, GCHandleType.Pinned);
		}

		public static void ReturnHandle(GCHandle handle)
		{
			object locker = GCHandleFactory._locker;
			lock (locker)
			{
				GCHandleFactory._handles.Add(handle);
			}
		}

		private static List<GCHandle> _handles = new List<GCHandle>();

		private static object _locker = new object();
	}
}
