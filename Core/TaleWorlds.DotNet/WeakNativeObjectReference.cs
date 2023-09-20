using System;

namespace TaleWorlds.DotNet
{
	// Token: 0x0200002C RID: 44
	public sealed class WeakNativeObjectReference
	{
		// Token: 0x06000115 RID: 277 RVA: 0x0000523A File Offset: 0x0000343A
		public WeakNativeObjectReference(NativeObject nativeObject)
		{
			if (nativeObject != null)
			{
				this._pointer = nativeObject.Pointer;
				this._constructor = (Func<NativeObject>)Managed.GetConstructorDelegateOfWeakReferenceClass(nativeObject.GetType());
				this._weakReferenceCache = new WeakReference(nativeObject);
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000527C File Offset: 0x0000347C
		public void ManualInvalidate()
		{
			NativeObject nativeObject = (NativeObject)this._weakReferenceCache.Target;
			if (nativeObject != null)
			{
				nativeObject.ManualInvalidate();
			}
		}

		// Token: 0x06000117 RID: 279 RVA: 0x000052AC File Offset: 0x000034AC
		public NativeObject GetNativeObject()
		{
			if (!(this._pointer != UIntPtr.Zero))
			{
				return null;
			}
			NativeObject nativeObject = (NativeObject)this._weakReferenceCache.Target;
			if (nativeObject != null)
			{
				return nativeObject;
			}
			NativeObject nativeObject2 = this._constructor();
			nativeObject2.Construct(this._pointer);
			this._weakReferenceCache.Target = nativeObject2;
			return nativeObject2;
		}

		// Token: 0x04000062 RID: 98
		private readonly UIntPtr _pointer;

		// Token: 0x04000063 RID: 99
		private readonly Func<NativeObject> _constructor;

		// Token: 0x04000064 RID: 100
		private readonly WeakReference _weakReferenceCache;
	}
}
