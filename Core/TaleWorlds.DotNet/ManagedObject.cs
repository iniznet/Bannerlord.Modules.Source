using System;
using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000021 RID: 33
	public abstract class ManagedObject
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000AD RID: 173 RVA: 0x00003D8E File Offset: 0x00001F8E
		internal ManagedObjectOwner ManagedObjectOwner
		{
			get
			{
				return this._managedObjectOwner;
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00003D98 File Offset: 0x00001F98
		internal static void FinalizeManagedObjects()
		{
			List<List<ManagedObject>> managedObjectFirstReferences = ManagedObject._managedObjectFirstReferences;
			lock (managedObjectFirstReferences)
			{
				ManagedObject._managedObjectFirstReferences.Clear();
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00003DDC File Offset: 0x00001FDC
		protected void AddUnmanagedMemoryPressure(int size)
		{
			GC.AddMemoryPressure((long)size);
			this.forcedMemory = size;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00003DEC File Offset: 0x00001FEC
		static ManagedObject()
		{
			for (int i = 0; i < 200; i++)
			{
				ManagedObject._managedObjectFirstReferences.Add(new List<ManagedObject>());
			}
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00003E22 File Offset: 0x00002022
		protected ManagedObject(UIntPtr ptr, bool createManagedObjectOwner)
		{
			if (createManagedObjectOwner)
			{
				this.SetOwnerManagedObject(ManagedObjectOwner.CreateManagedObjectOwner(ptr, this));
			}
			this.Initialize();
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00003E40 File Offset: 0x00002040
		internal void SetOwnerManagedObject(ManagedObjectOwner owner)
		{
			this._managedObjectOwner = owner;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00003E49 File Offset: 0x00002049
		private void Initialize()
		{
			ManagedObject.ManagedObjectFetched(this);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00003E54 File Offset: 0x00002054
		~ManagedObject()
		{
			if (this.forcedMemory > 0)
			{
				GC.RemoveMemoryPressure((long)this.forcedMemory);
			}
			ManagedObjectOwner.ManagedObjectGarbageCollected(this._managedObjectOwner, this);
			this._managedObjectOwner = null;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00003EA4 File Offset: 0x000020A4
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

		// Token: 0x060000B6 RID: 182 RVA: 0x00003F28 File Offset: 0x00002128
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

		// Token: 0x060000B7 RID: 183 RVA: 0x00003F7C File Offset: 0x0000217C
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

		// Token: 0x060000B8 RID: 184 RVA: 0x00003FD8 File Offset: 0x000021D8
		[LibraryCallback]
		internal static int GetAliveManagedObjectCount()
		{
			return ManagedObjectOwner.NumberOfAliveManagedObjects;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00003FDF File Offset: 0x000021DF
		[LibraryCallback]
		internal static string GetAliveManagedObjectNames()
		{
			return ManagedObjectOwner.GetAliveManagedObjectNames();
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00003FE6 File Offset: 0x000021E6
		[LibraryCallback]
		internal static string GetCreationCallstack(string name)
		{
			return ManagedObjectOwner.GetAliveManagedObjectCreationCallstacks(name);
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00003FEE File Offset: 0x000021EE
		// (set) Token: 0x060000BC RID: 188 RVA: 0x00003FFB File Offset: 0x000021FB
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

		// Token: 0x060000BD RID: 189 RVA: 0x00004009 File Offset: 0x00002209
		public int GetManagedId()
		{
			return this._managedObjectOwner.NativeId;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00004016 File Offset: 0x00002216
		[LibraryCallback]
		internal string GetClassOfObject()
		{
			return base.GetType().Name;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00004023 File Offset: 0x00002223
		public override int GetHashCode()
		{
			return this._managedObjectOwner.NativeId;
		}

		// Token: 0x0400003E RID: 62
		private const int ManagedObjectFirstReferencesTickCount = 200;

		// Token: 0x0400003F RID: 63
		private static List<List<ManagedObject>> _managedObjectFirstReferences = new List<List<ManagedObject>>();

		// Token: 0x04000040 RID: 64
		private ManagedObjectOwner _managedObjectOwner;

		// Token: 0x04000041 RID: 65
		private int forcedMemory;
	}
}
