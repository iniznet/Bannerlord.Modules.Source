using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	// Token: 0x0200000B RID: 11
	public class DotNetObject
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000026 RID: 38 RVA: 0x000027E0 File Offset: 0x000009E0
		internal static int NumberOfAliveDotNetObjects
		{
			get
			{
				return DotNetObject._numberOfAliveDotNetObjects;
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000027E8 File Offset: 0x000009E8
		static DotNetObject()
		{
			for (int i = 0; i < 200; i++)
			{
				DotNetObject.DotnetObjectFirstReferences.Add(new Dictionary<int, DotNetObject>());
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002834 File Offset: 0x00000A34
		protected DotNetObject()
		{
			object locker = DotNetObject.Locker;
			lock (locker)
			{
				DotNetObject._totalCreatedObjectCount++;
				this._objectId = DotNetObject._totalCreatedObjectCount;
				DotNetObject.DotnetObjectFirstReferences[0].Add(this._objectId, this);
				DotNetObject._numberOfAliveDotNetObjects++;
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000028B0 File Offset: 0x00000AB0
		protected override void Finalize()
		{
			try
			{
				object locker = DotNetObject.Locker;
				lock (locker)
				{
					DotNetObject._numberOfAliveDotNetObjects--;
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002908 File Offset: 0x00000B08
		[LibraryCallback]
		internal static int GetAliveDotNetObjectCount()
		{
			return DotNetObject._numberOfAliveDotNetObjects;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002910 File Offset: 0x00000B10
		[LibraryCallback]
		internal static void IncreaseReferenceCount(int dotnetObjectId)
		{
			object locker = DotNetObject.Locker;
			lock (locker)
			{
				if (DotNetObject.DotnetObjectReferences.ContainsKey(dotnetObjectId))
				{
					DotNetObjectReferenceCounter dotNetObjectReferenceCounter = DotNetObject.DotnetObjectReferences[dotnetObjectId];
					dotNetObjectReferenceCounter.ReferenceCount++;
					DotNetObject.DotnetObjectReferences[dotnetObjectId] = dotNetObjectReferenceCounter;
				}
				else
				{
					DotNetObject dotNetObjectFromFirstReferences = DotNetObject.GetDotNetObjectFromFirstReferences(dotnetObjectId);
					DotNetObjectReferenceCounter dotNetObjectReferenceCounter2 = default(DotNetObjectReferenceCounter);
					dotNetObjectReferenceCounter2.ReferenceCount = 1;
					dotNetObjectReferenceCounter2.DotNetObject = dotNetObjectFromFirstReferences;
					DotNetObject.DotnetObjectReferences.Add(dotnetObjectId, dotNetObjectReferenceCounter2);
				}
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000029A8 File Offset: 0x00000BA8
		[LibraryCallback]
		internal static void DecreaseReferenceCount(int dotnetObjectId)
		{
			object locker = DotNetObject.Locker;
			lock (locker)
			{
				DotNetObjectReferenceCounter dotNetObjectReferenceCounter = DotNetObject.DotnetObjectReferences[dotnetObjectId];
				dotNetObjectReferenceCounter.ReferenceCount--;
				if (dotNetObjectReferenceCounter.ReferenceCount == 0)
				{
					DotNetObject.DotnetObjectReferences.Remove(dotnetObjectId);
				}
				else
				{
					DotNetObject.DotnetObjectReferences[dotnetObjectId] = dotNetObjectReferenceCounter;
				}
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002A1C File Offset: 0x00000C1C
		internal static DotNetObject GetManagedObjectWithId(int dotnetObjectId)
		{
			object locker = DotNetObject.Locker;
			DotNetObject dotNetObject;
			lock (locker)
			{
				DotNetObjectReferenceCounter dotNetObjectReferenceCounter;
				if (DotNetObject.DotnetObjectReferences.TryGetValue(dotnetObjectId, out dotNetObjectReferenceCounter))
				{
					dotNetObject = dotNetObjectReferenceCounter.DotNetObject;
				}
				else
				{
					dotNetObject = DotNetObject.GetDotNetObjectFromFirstReferences(dotnetObjectId);
				}
			}
			return dotNetObject;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002A78 File Offset: 0x00000C78
		private static DotNetObject GetDotNetObjectFromFirstReferences(int dotnetObjectId)
		{
			using (List<Dictionary<int, DotNetObject>>.Enumerator enumerator = DotNetObject.DotnetObjectFirstReferences.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DotNetObject dotNetObject;
					if (enumerator.Current.TryGetValue(dotnetObjectId, out dotNetObject))
					{
						return dotNetObject;
					}
				}
			}
			return null;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002AD4 File Offset: 0x00000CD4
		internal int GetManagedId()
		{
			return this._objectId;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002ADC File Offset: 0x00000CDC
		[LibraryCallback]
		internal static string GetAliveDotNetObjectNames()
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "GetAliveDotNetObjectNames");
			object locker = DotNetObject.Locker;
			lock (locker)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (DotNetObjectReferenceCounter dotNetObjectReferenceCounter in DotNetObject.DotnetObjectReferences.Values)
				{
					Type type = dotNetObjectReferenceCounter.DotNetObject.GetType();
					if (!dictionary.ContainsKey(type.Name))
					{
						dictionary.Add(type.Name, 1);
					}
					else
					{
						dictionary[type.Name] = dictionary[type.Name] + 1;
					}
				}
				foreach (string text in dictionary.Keys)
				{
					mbstringBuilder.Append<string>(string.Concat(new object[]
					{
						text,
						",",
						dictionary[text],
						"-"
					}));
				}
			}
			return mbstringBuilder.ToStringAndRelease();
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002C34 File Offset: 0x00000E34
		internal static void HandleDotNetObjects()
		{
			object locker = DotNetObject.Locker;
			lock (locker)
			{
				Dictionary<int, DotNetObject> dictionary = DotNetObject.DotnetObjectFirstReferences[199];
				for (int i = 199; i > 0; i--)
				{
					DotNetObject.DotnetObjectFirstReferences[i] = DotNetObject.DotnetObjectFirstReferences[i - 1];
				}
				dictionary.Clear();
				DotNetObject.DotnetObjectFirstReferences[0] = dictionary;
			}
		}

		// Token: 0x04000017 RID: 23
		private static readonly object Locker = new object();

		// Token: 0x04000018 RID: 24
		private const int DotnetObjectFirstReferencesTickCount = 200;

		// Token: 0x04000019 RID: 25
		private static readonly List<Dictionary<int, DotNetObject>> DotnetObjectFirstReferences = new List<Dictionary<int, DotNetObject>>();

		// Token: 0x0400001A RID: 26
		private static readonly Dictionary<int, DotNetObjectReferenceCounter> DotnetObjectReferences = new Dictionary<int, DotNetObjectReferenceCounter>();

		// Token: 0x0400001B RID: 27
		private static int _totalCreatedObjectCount;

		// Token: 0x0400001C RID: 28
		private readonly int _objectId;

		// Token: 0x0400001D RID: 29
		private static int _numberOfAliveDotNetObjects;
	}
}
