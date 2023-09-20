using System;

namespace TaleWorlds.DotNet
{
	// Token: 0x0200002D RID: 45
	public sealed class WeakNativeObjectReference<T> where T : NativeObject
	{
		// Token: 0x06000118 RID: 280 RVA: 0x0000530E File Offset: 0x0000350E
		public WeakNativeObjectReference(T nativeObject)
		{
			if (nativeObject != null)
			{
				this._pointer = nativeObject.Pointer;
				this._weakReferenceCache = new WeakReference<T>(nativeObject);
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00005344 File Offset: 0x00003544
		public void ManualInvalidate()
		{
			T t;
			if (this._weakReferenceCache.TryGetTarget(out t) && t != null)
			{
				t.ManualInvalidate();
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000537C File Offset: 0x0000357C
		public NativeObject GetNativeObject()
		{
			if (!(this._pointer != UIntPtr.Zero))
			{
				return null;
			}
			T t;
			if (this._weakReferenceCache.TryGetTarget(out t) && t != null)
			{
				return t;
			}
			T t2 = (T)((object)Activator.CreateInstance(typeof(T), new object[] { this._pointer }));
			this._weakReferenceCache.SetTarget(t2);
			return t2;
		}

		// Token: 0x04000065 RID: 101
		private readonly UIntPtr _pointer;

		// Token: 0x04000066 RID: 102
		private WeakReference<T> _weakReferenceCache;
	}
}
