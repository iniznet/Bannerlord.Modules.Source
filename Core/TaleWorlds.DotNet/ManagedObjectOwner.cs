using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	internal class ManagedObjectOwner
	{
		internal static int NumberOfAliveManagedObjects
		{
			get
			{
				return ManagedObjectOwner._numberOfAliveManagedObjects;
			}
		}

		static ManagedObjectOwner()
		{
			for (int i = 0; i < 8192; i++)
			{
				ManagedObjectOwner managedObjectOwner = new ManagedObjectOwner();
				ManagedObjectOwner._pool.Add(managedObjectOwner);
				WeakReference weakReference = new WeakReference(null);
				ManagedObjectOwner._managedObjectOwnerWeakReferences.Add(weakReference);
			}
		}

		internal static void GarbageCollect()
		{
			HashSet<ManagedObjectOwner> managedObjectOwnerReferences = ManagedObjectOwner._managedObjectOwnerReferences;
			lock (managedObjectOwnerReferences)
			{
				ManagedObjectOwner._lastframedeletedManagedObjectBuffer.AddRange(ManagedObjectOwner._lastframedeletedManagedObjects);
				ManagedObjectOwner._lastframedeletedManagedObjects.Clear();
				foreach (ManagedObjectOwner managedObjectOwner in ManagedObjectOwner._lastframedeletedManagedObjectBuffer)
				{
					if (managedObjectOwner._ptr != UIntPtr.Zero)
					{
						LibraryApplicationInterface.IManaged.ReleaseManagedObject(managedObjectOwner._ptr);
						managedObjectOwner._ptr = UIntPtr.Zero;
					}
					ManagedObjectOwner._numberOfAliveManagedObjects--;
					WeakReference weakReference = ManagedObjectOwner._managedObjectOwners[managedObjectOwner.NativeId];
					ManagedObjectOwner._managedObjectOwners.Remove(managedObjectOwner.NativeId);
					weakReference.Target = null;
					ManagedObjectOwner._managedObjectOwnerWeakReferences.Add(weakReference);
				}
			}
			foreach (ManagedObjectOwner managedObjectOwner2 in ManagedObjectOwner._lastframedeletedManagedObjectBuffer)
			{
				managedObjectOwner2.Destruct();
				ManagedObjectOwner._pool.Add(managedObjectOwner2);
			}
			ManagedObjectOwner._lastframedeletedManagedObjectBuffer.Clear();
		}

		internal static void LogFinalize()
		{
			Debug.Print("Checking if any managed object still lives...", 0, Debug.DebugColor.White, 17592186044416UL);
			int num = 0;
			HashSet<ManagedObjectOwner> managedObjectOwnerReferences = ManagedObjectOwner._managedObjectOwnerReferences;
			lock (managedObjectOwnerReferences)
			{
				foreach (KeyValuePair<int, WeakReference> keyValuePair in ManagedObjectOwner._managedObjectOwners)
				{
					if (keyValuePair.Value.Target != null)
					{
						Debug.Print("An object with type of " + keyValuePair.Value.Target.GetType().Name + " still lives", 0, Debug.DebugColor.White, 17592186044416UL);
						num++;
					}
				}
			}
			if (num == 0)
			{
				Debug.Print("There are no living managed objects.", 0, Debug.DebugColor.White, 17592186044416UL);
				return;
			}
			Debug.Print("There are " + num + " living managed objects.", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		internal static void PreFinalizeManagedObjects()
		{
			ManagedObjectOwner.GarbageCollect();
		}

		internal static ManagedObject GetManagedObjectWithId(int id)
		{
			if (id == 0)
			{
				return null;
			}
			HashSet<ManagedObjectOwner> managedObjectOwnerReferences = ManagedObjectOwner._managedObjectOwnerReferences;
			lock (managedObjectOwnerReferences)
			{
				ManagedObjectOwner managedObjectOwner = ManagedObjectOwner._managedObjectOwners[id].Target as ManagedObjectOwner;
				if (managedObjectOwner != null)
				{
					ManagedObject managedObject = managedObjectOwner.TryGetManagedObject();
					ManagedObject.ManagedObjectFetched(managedObject);
					return managedObject;
				}
			}
			return null;
		}

		internal static void ManagedObjectGarbageCollected(ManagedObjectOwner owner, ManagedObject managedObject)
		{
			HashSet<ManagedObjectOwner> managedObjectOwnerReferences = ManagedObjectOwner._managedObjectOwnerReferences;
			lock (managedObjectOwnerReferences)
			{
				if (owner != null && owner._managedObjectLongReference.Target as ManagedObject == managedObject)
				{
					ManagedObjectOwner._lastframedeletedManagedObjects.Add(owner);
					ManagedObjectOwner._managedObjectOwnerReferences.Remove(owner);
				}
			}
		}

		internal static ManagedObjectOwner CreateManagedObjectOwner(UIntPtr ptr, ManagedObject managedObject)
		{
			ManagedObjectOwner managedObjectOwner = null;
			if (ManagedObjectOwner._pool.Count > 0)
			{
				managedObjectOwner = ManagedObjectOwner._pool[ManagedObjectOwner._pool.Count - 1];
				ManagedObjectOwner._pool.RemoveAt(ManagedObjectOwner._pool.Count - 1);
			}
			else
			{
				managedObjectOwner = new ManagedObjectOwner();
			}
			managedObjectOwner.Construct(ptr, managedObject);
			HashSet<ManagedObjectOwner> managedObjectOwnerReferences = ManagedObjectOwner._managedObjectOwnerReferences;
			lock (managedObjectOwnerReferences)
			{
				ManagedObjectOwner._numberOfAliveManagedObjects++;
				ManagedObjectOwner._managedObjectOwnerReferences.Add(managedObjectOwner);
			}
			return managedObjectOwner;
		}

		internal static string GetAliveManagedObjectNames()
		{
			string text = "";
			HashSet<ManagedObjectOwner> managedObjectOwnerReferences = ManagedObjectOwner._managedObjectOwnerReferences;
			lock (managedObjectOwnerReferences)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (WeakReference weakReference in ManagedObjectOwner._managedObjectOwners.Values)
				{
					ManagedObjectOwner managedObjectOwner = weakReference.Target as ManagedObjectOwner;
					if (!dictionary.ContainsKey(managedObjectOwner._typeInfo.Name))
					{
						dictionary.Add(managedObjectOwner._typeInfo.Name, 1);
					}
					else
					{
						dictionary[managedObjectOwner._typeInfo.Name] = dictionary[managedObjectOwner._typeInfo.Name] + 1;
					}
				}
				foreach (string text2 in dictionary.Keys)
				{
					text = string.Concat(new object[]
					{
						text,
						text2,
						",",
						dictionary[text2],
						"-"
					});
				}
			}
			return text;
		}

		internal static string GetAliveManagedObjectCreationCallstacks(string name)
		{
			return "";
		}

		internal int NativeId
		{
			get
			{
				return this._nativeId;
			}
		}

		internal UIntPtr Pointer
		{
			get
			{
				return this._ptr;
			}
			set
			{
				if (value != UIntPtr.Zero)
				{
					LibraryApplicationInterface.IManaged.IncreaseReferenceCount(value);
				}
				this._ptr = value;
			}
		}

		private ManagedObjectOwner()
		{
			this._ptr = UIntPtr.Zero;
			this._managedObject = new WeakReference(null, false);
			this._managedObjectLongReference = new WeakReference(null, true);
		}

		private void Construct(UIntPtr ptr, ManagedObject managedObject)
		{
			this._typeInfo = managedObject.GetType();
			this._managedObject.Target = managedObject;
			this._managedObjectLongReference.Target = managedObject;
			this.Pointer = ptr;
			HashSet<ManagedObjectOwner> managedObjectOwnerReferences = ManagedObjectOwner._managedObjectOwnerReferences;
			lock (managedObjectOwnerReferences)
			{
				this._nativeId = ManagedObjectOwner._lastId;
				ManagedObjectOwner._lastId++;
				WeakReference weakReference;
				if (ManagedObjectOwner._managedObjectOwnerWeakReferences.Count > 0)
				{
					weakReference = ManagedObjectOwner._managedObjectOwnerWeakReferences[ManagedObjectOwner._managedObjectOwnerWeakReferences.Count - 1];
					ManagedObjectOwner._managedObjectOwnerWeakReferences.RemoveAt(ManagedObjectOwner._managedObjectOwnerWeakReferences.Count - 1);
					weakReference.Target = this;
				}
				else
				{
					weakReference = new WeakReference(this);
				}
				ManagedObjectOwner._managedObjectOwners.Add(this.NativeId, weakReference);
			}
		}

		private void Destruct()
		{
			this._managedObject.Target = null;
			this._managedObjectLongReference.Target = null;
			this._typeInfo = null;
			this._ptr = UIntPtr.Zero;
			this._nativeId = 0;
		}

		protected override void Finalize()
		{
			try
			{
				HashSet<ManagedObjectOwner> managedObjectOwnerReferences = ManagedObjectOwner._managedObjectOwnerReferences;
				lock (managedObjectOwnerReferences)
				{
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		private ManagedObject TryGetManagedObject()
		{
			ManagedObject managedObject = null;
			HashSet<ManagedObjectOwner> managedObjectOwnerReferences = ManagedObjectOwner._managedObjectOwnerReferences;
			lock (managedObjectOwnerReferences)
			{
				managedObject = this._managedObject.Target as ManagedObject;
				if (managedObject == null)
				{
					managedObject = (ManagedObject)this._typeInfo.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(UIntPtr),
						typeof(bool)
					}, null).Invoke(new object[] { this._ptr, false });
					managedObject.SetOwnerManagedObject(this);
					this._managedObject.Target = managedObject;
					this._managedObjectLongReference.Target = managedObject;
					if (!ManagedObjectOwner._managedObjectOwnerReferences.Contains(this))
					{
						ManagedObjectOwner._managedObjectOwnerReferences.Add(this);
					}
					ManagedObjectOwner._lastframedeletedManagedObjects.Remove(this);
				}
			}
			return managedObject;
		}

		private const int PooledManagedObjectOwnerCount = 8192;

		private static readonly List<ManagedObjectOwner> _pool = new List<ManagedObjectOwner>(8192);

		private static readonly List<WeakReference> _managedObjectOwnerWeakReferences = new List<WeakReference>(8192);

		private static readonly Dictionary<int, WeakReference> _managedObjectOwners = new Dictionary<int, WeakReference>();

		private static readonly HashSet<ManagedObjectOwner> _managedObjectOwnerReferences = new HashSet<ManagedObjectOwner>();

		private static int _lastId = 10;

		private static readonly List<ManagedObjectOwner> _lastframedeletedManagedObjects = new List<ManagedObjectOwner>();

		private static int _numberOfAliveManagedObjects = 0;

		private static readonly List<ManagedObjectOwner> _lastframedeletedManagedObjectBuffer = new List<ManagedObjectOwner>(1024);

		private Type _typeInfo;

		private int _nativeId;

		private UIntPtr _ptr;

		private readonly WeakReference _managedObject;

		private readonly WeakReference _managedObjectLongReference;
	}
}
