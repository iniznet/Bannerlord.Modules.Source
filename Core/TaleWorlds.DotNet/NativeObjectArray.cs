using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
	[EngineClass("ftdnNative_object_array")]
	public sealed class NativeObjectArray : NativeObject, IEnumerable<NativeObject>, IEnumerable
	{
		internal NativeObjectArray(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		public static NativeObjectArray Create()
		{
			return LibraryApplicationInterface.INativeObjectArray.Create();
		}

		public int Count
		{
			get
			{
				return LibraryApplicationInterface.INativeObjectArray.GetCount(base.Pointer);
			}
		}

		public NativeObject GetElementAt(int index)
		{
			return LibraryApplicationInterface.INativeObjectArray.GetElementAtIndex(base.Pointer, index);
		}

		public void AddElement(NativeObject nativeObject)
		{
			LibraryApplicationInterface.INativeObjectArray.AddElement(base.Pointer, (nativeObject != null) ? nativeObject.Pointer : UIntPtr.Zero);
		}

		public void Clear()
		{
			LibraryApplicationInterface.INativeObjectArray.Clear(base.Pointer);
		}

		IEnumerator<NativeObject> IEnumerable<NativeObject>.GetEnumerator()
		{
			int count = this.Count;
			int num;
			for (int i = 0; i < count; i = num + 1)
			{
				NativeObject elementAt = this.GetElementAt(i);
				yield return elementAt;
				num = i;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			int count = this.Count;
			int num;
			for (int i = 0; i < count; i = num + 1)
			{
				NativeObject elementAt = this.GetElementAt(i);
				yield return elementAt;
				num = i;
			}
			yield break;
		}
	}
}
