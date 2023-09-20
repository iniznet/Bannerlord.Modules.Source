using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TaleWorlds.DotNet
{
	[EngineClass("ftdnNative_array")]
	public sealed class NativeArray : NativeObject
	{
		internal NativeArray(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		public static NativeArray Create()
		{
			return LibraryApplicationInterface.INativeArray.Create();
		}

		public int DataSize
		{
			get
			{
				return LibraryApplicationInterface.INativeArray.GetDataSize(base.Pointer);
			}
		}

		private UIntPtr GetDataPointer()
		{
			return LibraryApplicationInterface.INativeArray.GetDataPointer(base.Pointer);
		}

		public int GetLength<T>() where T : struct
		{
			int dataSize = this.DataSize;
			int num = Marshal.SizeOf<T>();
			return dataSize / num;
		}

		public T GetElementAt<T>(int index) where T : struct
		{
			IntPtr intPtr = Marshal.PtrToStructure<IntPtr>(new IntPtr((long)(base.Pointer.ToUInt64() + (ulong)((long)NativeArray.DataPointerOffset))));
			int num = Marshal.SizeOf<T>();
			return Marshal.PtrToStructure<T>(new IntPtr(intPtr.ToInt64() + (long)(index * num)));
		}

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

		public void AddElement(int value)
		{
			LibraryApplicationInterface.INativeArray.AddIntegerElement(base.Pointer, value);
		}

		public void AddElement(float value)
		{
			LibraryApplicationInterface.INativeArray.AddFloatElement(base.Pointer, value);
		}

		public void AddElement<T>(T value) where T : struct
		{
			int num = Marshal.SizeOf<T>();
			Marshal.StructureToPtr<T>(value, NativeArray._temporaryData, false);
			LibraryApplicationInterface.INativeArray.AddElement(base.Pointer, NativeArray._temporaryData, num);
		}

		public void Clear()
		{
			LibraryApplicationInterface.INativeArray.Clear(base.Pointer);
		}

		private static readonly IntPtr _temporaryData = Marshal.AllocHGlobal(16384);

		private const int TemporaryDataSize = 16384;

		private static readonly int DataPointerOffset = LibraryApplicationInterface.INativeArray.GetDataPointerOffset();
	}
}
