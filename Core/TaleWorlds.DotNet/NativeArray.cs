using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000025 RID: 37
	[EngineClass("ftdnNative_array")]
	public sealed class NativeArray : NativeObject
	{
		// Token: 0x060000DB RID: 219 RVA: 0x00004A3B File Offset: 0x00002C3B
		internal NativeArray(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00004A4A File Offset: 0x00002C4A
		public static NativeArray Create()
		{
			return LibraryApplicationInterface.INativeArray.Create();
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000DD RID: 221 RVA: 0x00004A56 File Offset: 0x00002C56
		public int DataSize
		{
			get
			{
				return LibraryApplicationInterface.INativeArray.GetDataSize(base.Pointer);
			}
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00004A68 File Offset: 0x00002C68
		private UIntPtr GetDataPointer()
		{
			return LibraryApplicationInterface.INativeArray.GetDataPointer(base.Pointer);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00004A7C File Offset: 0x00002C7C
		public int GetLength<T>() where T : struct
		{
			int dataSize = this.DataSize;
			int num = Marshal.SizeOf<T>();
			return dataSize / num;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00004A98 File Offset: 0x00002C98
		public T GetElementAt<T>(int index) where T : struct
		{
			IntPtr intPtr = Marshal.PtrToStructure<IntPtr>(new IntPtr((long)(base.Pointer.ToUInt64() + (ulong)((long)NativeArray.DataPointerOffset))));
			int num = Marshal.SizeOf<T>();
			return Marshal.PtrToStructure<T>(new IntPtr(intPtr.ToInt64() + (long)(index * num)));
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00004AE1 File Offset: 0x00002CE1
		public IEnumerable<T> GetEnumerator<T>() where T : struct
		{
			int length = this.GetLength<T>();
			IntPtr intPtr = new IntPtr((long)(base.Pointer.ToUInt64() + (ulong)((long)NativeArray.DataPointerOffset)));
			IntPtr dataPointer = Marshal.PtrToStructure<IntPtr>(intPtr);
			int elementSize = Marshal.SizeOf<T>();
			int num;
			for (int i = 0; i < length; i = num + 1)
			{
				T t = Marshal.PtrToStructure<T>(new IntPtr(dataPointer.ToInt64() + (long)(i * elementSize)));
				yield return t;
				num = i;
			}
			yield break;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00004AF4 File Offset: 0x00002CF4
		public T[] ToArray<T>() where T : struct
		{
			T[] array = new T[this.GetLength<T>()];
			IEnumerable<T> enumerator = this.GetEnumerator<T>();
			int num = 0;
			foreach (T t in enumerator)
			{
				array[num] = t;
				num++;
			}
			return array;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00004B58 File Offset: 0x00002D58
		public void AddElement(int value)
		{
			LibraryApplicationInterface.INativeArray.AddIntegerElement(base.Pointer, value);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00004B6B File Offset: 0x00002D6B
		public void AddElement(float value)
		{
			LibraryApplicationInterface.INativeArray.AddFloatElement(base.Pointer, value);
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00004B80 File Offset: 0x00002D80
		public void AddElement<T>(T value) where T : struct
		{
			int num = Marshal.SizeOf<T>();
			Marshal.StructureToPtr<T>(value, NativeArray._temporaryData, false);
			LibraryApplicationInterface.INativeArray.AddElement(base.Pointer, NativeArray._temporaryData, num);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00004BB5 File Offset: 0x00002DB5
		public void Clear()
		{
			LibraryApplicationInterface.INativeArray.Clear(base.Pointer);
		}

		// Token: 0x04000055 RID: 85
		private static readonly IntPtr _temporaryData = Marshal.AllocHGlobal(16384);

		// Token: 0x04000056 RID: 86
		private const int TemporaryDataSize = 16384;

		// Token: 0x04000057 RID: 87
		private static readonly int DataPointerOffset = LibraryApplicationInterface.INativeArray.GetDataPointerOffset();
	}
}
