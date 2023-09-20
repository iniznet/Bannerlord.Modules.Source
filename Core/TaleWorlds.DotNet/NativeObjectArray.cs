using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000027 RID: 39
	[EngineClass("ftdnNative_object_array")]
	public sealed class NativeObjectArray : NativeObject, IEnumerable<NativeObject>, IEnumerable
	{
		// Token: 0x060000FA RID: 250 RVA: 0x000050A0 File Offset: 0x000032A0
		internal NativeObjectArray(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x060000FB RID: 251 RVA: 0x000050AF File Offset: 0x000032AF
		public static NativeObjectArray Create()
		{
			return LibraryApplicationInterface.INativeObjectArray.Create();
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000FC RID: 252 RVA: 0x000050BB File Offset: 0x000032BB
		public int Count
		{
			get
			{
				return LibraryApplicationInterface.INativeObjectArray.GetCount(base.Pointer);
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x000050CD File Offset: 0x000032CD
		public NativeObject GetElementAt(int index)
		{
			return LibraryApplicationInterface.INativeObjectArray.GetElementAtIndex(base.Pointer, index);
		}

		// Token: 0x060000FE RID: 254 RVA: 0x000050E0 File Offset: 0x000032E0
		public void AddElement(NativeObject nativeObject)
		{
			LibraryApplicationInterface.INativeObjectArray.AddElement(base.Pointer, (nativeObject != null) ? nativeObject.Pointer : UIntPtr.Zero);
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00005108 File Offset: 0x00003308
		public void Clear()
		{
			LibraryApplicationInterface.INativeObjectArray.Clear(base.Pointer);
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0000511A File Offset: 0x0000331A
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

		// Token: 0x06000101 RID: 257 RVA: 0x00005129 File Offset: 0x00003329
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
