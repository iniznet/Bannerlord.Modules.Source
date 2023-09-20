using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.Library
{
	public struct PinnedArrayData<T>
	{
		public bool Pinned { get; private set; }

		public IntPtr Pointer { get; private set; }

		public T[] Array { get; private set; }

		public T[,] Array2D { get; private set; }

		public GCHandle Handle
		{
			get
			{
				return this._handle;
			}
		}

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
						this._handle = GCHandleFactory.GetHandle();
						this._handle.Target = array;
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
						this._handle = GCHandleFactory.GetHandle();
						this._handle.Target = array;
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

		public void Dispose()
		{
			if (this.Pinned)
			{
				if (this.Array != null)
				{
					this._handle.Target = null;
					GCHandleFactory.ReturnHandle(this._handle);
					this.Array = null;
					this.Pointer = IntPtr.Zero;
					return;
				}
				if (this.Array2D != null)
				{
					this._handle.Target = null;
					GCHandleFactory.ReturnHandle(this._handle);
					this.Array2D = null;
					this.Pointer = IntPtr.Zero;
				}
			}
		}

		private static IntPtr _unmanagedCache = Marshal.AllocHGlobal(16384);

		private GCHandle _handle;
	}
}
