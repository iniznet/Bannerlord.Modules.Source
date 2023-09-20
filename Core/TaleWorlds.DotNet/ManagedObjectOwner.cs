using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000022 RID: 34
	internal class ManagedObjectOwner
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00004030 File Offset: 0x00002230
		internal static int NumberOfAliveManagedObjects
		{
			get
			{
				return ManagedObjectOwner._numberOfAliveManagedObjects;
			}
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00004038 File Offset: 0x00002238
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

		// Token: 0x060000C2 RID: 194 RVA: 0x000040D0 File Offset: 0x000022D0
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

		// Token: 0x060000C3 RID: 195 RVA: 0x00004228 File Offset: 0x00002428
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

		// Token: 0x060000C4 RID: 196 RVA: 0x0000433C File Offset: 0x0000253C
		internal static void PreFinalizeManagedObjects()
		{
			ManagedObjectOwner.GarbageCollect();
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00004344 File Offset: 0x00002544
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

		// Token: 0x060000C6 RID: 198 RVA: 0x000043B0 File Offset: 0x000025B0
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

		// Token: 0x060000C7 RID: 199 RVA: 0x00004418 File Offset: 0x00002618
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

		// Token: 0x060000C8 RID: 200 RVA: 0x000044B8 File Offset: 0x000026B8
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

		// Token: 0x060000C9 RID: 201 RVA: 0x0000462C File Offset: 0x0000282C
		internal static string GetAliveManagedObjectCreationCallstacks(string name)
		{
			return "";
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000CA RID: 202 RVA: 0x00004633 File Offset: 0x00002833
		internal int NativeId
		{
			get
			{
				return this._nativeId;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000CB RID: 203 RVA: 0x0000463B File Offset: 0x0000283B
		// (set) Token: 0x060000CC RID: 204 RVA: 0x00004643 File Offset: 0x00002843
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

		// Token: 0x060000CD RID: 205 RVA: 0x00004664 File Offset: 0x00002864
		private ManagedObjectOwner()
		{
			this._ptr = UIntPtr.Zero;
			this._managedObject = new WeakReference(null, false);
			this._managedObjectLongReference = new WeakReference(null, true);
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00004694 File Offset: 0x00002894
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

		// Token: 0x060000CF RID: 207 RVA: 0x0000476C File Offset: 0x0000296C
		private void Destruct()
		{
			this._managedObject.Target = null;
			this._managedObjectLongReference.Target = null;
			this._typeInfo = null;
			this._ptr = UIntPtr.Zero;
			this._nativeId = 0;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x000047A0 File Offset: 0x000029A0
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

		// Token: 0x060000D1 RID: 209 RVA: 0x000047EC File Offset: 0x000029EC
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

		// Token: 0x04000042 RID: 66
		private const int PooledManagedObjectOwnerCount = 8192;

		// Token: 0x04000043 RID: 67
		private static readonly List<ManagedObjectOwner> _pool = new List<ManagedObjectOwner>(8192);

		// Token: 0x04000044 RID: 68
		private static readonly List<WeakReference> _managedObjectOwnerWeakReferences = new List<WeakReference>(8192);

		// Token: 0x04000045 RID: 69
		private static readonly Dictionary<int, WeakReference> _managedObjectOwners = new Dictionary<int, WeakReference>();

		// Token: 0x04000046 RID: 70
		private static readonly HashSet<ManagedObjectOwner> _managedObjectOwnerReferences = new HashSet<ManagedObjectOwner>();

		// Token: 0x04000047 RID: 71
		private static int _lastId = 10;

		// Token: 0x04000048 RID: 72
		private static readonly List<ManagedObjectOwner> _lastframedeletedManagedObjects = new List<ManagedObjectOwner>();

		// Token: 0x04000049 RID: 73
		private static int _numberOfAliveManagedObjects = 0;

		// Token: 0x0400004A RID: 74
		private static readonly List<ManagedObjectOwner> _lastframedeletedManagedObjectBuffer = new List<ManagedObjectOwner>(1024);

		// Token: 0x0400004B RID: 75
		private Type _typeInfo;

		// Token: 0x0400004C RID: 76
		private int _nativeId;

		// Token: 0x0400004D RID: 77
		private UIntPtr _ptr;

		// Token: 0x0400004E RID: 78
		private readonly WeakReference _managedObject;

		// Token: 0x0400004F RID: 79
		private readonly WeakReference _managedObjectLongReference;
	}
}
