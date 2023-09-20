using System;
using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
	public abstract class ManagedObject
	{
		internal ManagedObjectOwner ManagedObjectOwner
		{
			get
			{
				return this._managedObjectOwner;
			}
		}

		internal static void FinalizeManagedObjects()
		{
			List<List<ManagedObject>> managedObjectFirstReferences = ManagedObject._managedObjectFirstReferences;
			lock (managedObjectFirstReferences)
			{
				ManagedObject._managedObjectFirstReferences.Clear();
			}
		}

		protected void AddUnmanagedMemoryPressure(int size)
		{
			GC.AddMemoryPressure((long)size);
			this.forcedMemory = size;
		}

		static ManagedObject()
		{
			for (int i = 0; i < 200; i++)
			{
				ManagedObject._managedObjectFirstReferences.Add(new List<ManagedObject>());
			}
		}

		protected ManagedObject(UIntPtr ptr, bool createManagedObjectOwner)
		{
			if (createManagedObjectOwner)
			{
				this.SetOwnerManagedObject(ManagedObjectOwner.CreateManagedObjectOwner(ptr, this));
			}
			this.Initialize();
		}

		internal void SetOwnerManagedObject(ManagedObjectOwner owner)
		{
			this._managedObjectOwner = owner;
		}

		private void Initialize()
		{
			ManagedObject.ManagedObjectFetched(this);
		}

		~ManagedObject()
		{
			if (this.forcedMemory > 0)
			{
				GC.RemoveMemoryPressure((long)this.forcedMemory);
			}
			ManagedObjectOwner.ManagedObjectGarbageCollected(this._managedObjectOwner, this);
			this._managedObjectOwner = null;
		}

		internal static void HandleManagedObjects()
		{
			List<List<ManagedObject>> managedObjectFirstReferences = ManagedObject._managedObjectFirstReferences;
			lock (managedObjectFirstReferences)
			{
				List<ManagedObject> list = ManagedObject._managedObjectFirstReferences[199];
				for (int i = 199; i > 0; i--)
				{
					ManagedObject._managedObjectFirstReferences[i] = ManagedObject._managedObjectFirstReferences[i - 1];
				}
				list.Clear();
				ManagedObject._managedObjectFirstReferences[0] = list;
			}
		}

		internal static void ManagedObjectFetched(ManagedObject managedObject)
		{
			List<List<ManagedObject>> managedObjectFirstReferences = ManagedObject._managedObjectFirstReferences;
			lock (managedObjectFirstReferences)
			{
				if (!Managed.Closing)
				{
					ManagedObject._managedObjectFirstReferences[0].Add(managedObject);
				}
			}
		}

		internal static void FlushManagedObjects()
		{
			List<List<ManagedObject>> managedObjectFirstReferences = ManagedObject._managedObjectFirstReferences;
			lock (managedObjectFirstReferences)
			{
				for (int i = 0; i < 200; i++)
				{
					ManagedObject._managedObjectFirstReferences[i].Clear();
				}
			}
		}

		[LibraryCallback]
		internal static int GetAliveManagedObjectCount()
		{
			return ManagedObjectOwner.NumberOfAliveManagedObjects;
		}

		[LibraryCallback]
		internal static string GetAliveManagedObjectNames()
		{
			return ManagedObjectOwner.GetAliveManagedObjectNames();
		}

		[LibraryCallback]
		internal static string GetCreationCallstack(string name)
		{
			return ManagedObjectOwner.GetAliveManagedObjectCreationCallstacks(name);
		}

		internal UIntPtr Pointer
		{
			get
			{
				return this._managedObjectOwner.Pointer;
			}
			set
			{
				this._managedObjectOwner.Pointer = value;
			}
		}

		public int GetManagedId()
		{
			return this._managedObjectOwner.NativeId;
		}

		[LibraryCallback]
		internal string GetClassOfObject()
		{
			return base.GetType().Name;
		}

		public override int GetHashCode()
		{
			return this._managedObjectOwner.NativeId;
		}

		private const int ManagedObjectFirstReferencesTickCount = 200;

		private static List<List<ManagedObject>> _managedObjectFirstReferences = new List<List<ManagedObject>>();

		private ManagedObjectOwner _managedObjectOwner;

		private int forcedMemory;
	}
}
