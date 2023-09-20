using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	public class DotNetObject
	{
		internal static int NumberOfAliveDotNetObjects
		{
			get
			{
				return DotNetObject._numberOfAliveDotNetObjects;
			}
		}

		static DotNetObject()
		{
			for (int i = 0; i < 200; i++)
			{
				DotNetObject.DotnetObjectFirstReferences.Add(new Dictionary<int, DotNetObject>());
			}
		}

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

		[LibraryCallback]
		internal static int GetAliveDotNetObjectCount()
		{
			return DotNetObject._numberOfAliveDotNetObjects;
		}

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

		internal int GetManagedId()
		{
			return this._objectId;
		}

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

		private static readonly object Locker = new object();

		private const int DotnetObjectFirstReferencesTickCount = 200;

		private static readonly List<Dictionary<int, DotNetObject>> DotnetObjectFirstReferences = new List<Dictionary<int, DotNetObject>>();

		private static readonly Dictionary<int, DotNetObjectReferenceCounter> DotnetObjectReferences = new Dictionary<int, DotNetObjectReferenceCounter>();

		private static int _totalCreatedObjectCount;

		private readonly int _objectId;

		private static int _numberOfAliveDotNetObjects;
	}
}
