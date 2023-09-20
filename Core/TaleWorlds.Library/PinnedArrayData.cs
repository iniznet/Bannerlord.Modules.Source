using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.Library
{
	// Token: 0x02000072 RID: 114
	public struct PinnedArrayData<T>
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060003D4 RID: 980 RVA: 0x0000C138 File Offset: 0x0000A338
		// (set) Token: 0x060003D5 RID: 981 RVA: 0x0000C140 File Offset: 0x0000A340
		public bool Pinned { get; private set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060003D6 RID: 982 RVA: 0x0000C149 File Offset: 0x0000A349
		// (set) Token: 0x060003D7 RID: 983 RVA: 0x0000C151 File Offset: 0x0000A351
		public IntPtr Pointer { get; private set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060003D8 RID: 984 RVA: 0x0000C15A File Offset: 0x0000A35A
		// (set) Token: 0x060003D9 RID: 985 RVA: 0x0000C162 File Offset: 0x0000A362
		public T[] Array { get; private set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060003DA RID: 986 RVA: 0x0000C16B File Offset: 0x0000A36B
		// (set) Token: 0x060003DB RID: 987 RVA: 0x0000C173 File Offset: 0x0000A373
		public T[,] Array2D { get; private set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060003DC RID: 988 RVA: 0x0000C17C File Offset: 0x0000A37C
		// (set) Token: 0x060003DD RID: 989 RVA: 0x0000C184 File Offset: 0x0000A384
		public GCHandle Handle { get; private set; }

		// Token: 0x060003DE RID: 990 RVA: 0x0000C190 File Offset: 0x0000A390
		public PinnedArrayData(T[] array, bool manualPinning = false)
		{
			this.Array = array;
			this.Array2D = null;
			this.Pinned = false;
			this.Pointer = IntPtr.Zero;
			if (array != null)
			{
				if (!manualPinning)
				{
					try
					{
						this.Handle = GCHandle.Alloc(array, GCHandleType.Pinned);
						this.Pointer = this.Handle.AddrOfPinnedObject();
						this.Pinned = true;
					}
					catch (ArgumentException)
					{
						manualPinning = true;
					}
				}
				if (manualPinning)
				{
					this.Pinned = false;
					int num = Marshal.SizeOf<T>();
					for (int i = 0; i < array.Length; i++)
					{
						Marshal.StructureToPtr<T>(array[i], PinnedArrayData<T>._unmanagedCache + num * i, false);
					}
					this.Pointer = PinnedArrayData<T>._unmanagedCache;
				}
			}
		}

		// Token: 0x060003DF RID: 991 RVA: 0x0000C248 File Offset: 0x0000A448
		public PinnedArrayData(T[,] array, bool manualPinning = false)
		{
			this.Array = null;
			this.Array2D = array;
			this.Pinned = false;
			this.Pointer = IntPtr.Zero;
			if (array != null)
			{
				if (!manualPinning)
				{
					try
					{
						this.Handle = GCHandle.Alloc(array, GCHandleType.Pinned);
						this.Pointer = this.Handle.AddrOfPinnedObject();
						this.Pinned = true;
					}
					catch (ArgumentException)
					{
						manualPinning = true;
					}
				}
				if (manualPinning)
				{
					this.Pinned = false;
					int num = Marshal.SizeOf<T>();
					for (int i = 0; i < array.GetLength(0); i++)
					{
						for (int j = 0; j < array.GetLength(1); j++)
						{
							Marshal.StructureToPtr<T>(array[i, j], PinnedArrayData<T>._unmanagedCache + num * (i * array.GetLength(1) + j), false);
						}
					}
					this.Pointer = PinnedArrayData<T>._unmanagedCache;
				}
			}
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x0000C324 File Offset: 0x0000A524
		public static bool CheckIfTypeRequiresManualPinning(Type type)
		{
			bool flag = false;
			Array array = System.Array.CreateInstance(type, 10);
			GCHandle gchandle;
			try
			{
				gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
				gchandle.AddrOfPinnedObject();
			}
			catch (ArgumentException)
			{
				flag = true;
			}
			if (gchandle.IsAllocated)
			{
				gchandle.Free();
			}
			return flag;
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x0000C374 File Offset: 0x0000A574
		public void Dispose()
		{
			if (this.Pinned)
			{
				if (this.Array != null)
				{
					this.Handle.Free();
					this.Array = null;
					this.Pointer = IntPtr.Zero;
					return;
				}
				if (this.Array2D != null)
				{
					this.Handle.Free();
					this.Array2D = null;
					this.Pointer = IntPtr.Zero;
				}
			}
		}

		// Token: 0x04000128 RID: 296
		private static IntPtr _unmanagedCache = Marshal.AllocHGlobal(16384);
	}
}
