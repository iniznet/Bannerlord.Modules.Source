using System;

namespace TaleWorlds.DotNet
{
	public sealed class WeakNativeObjectReference
	{
		public WeakNativeObjectReference(NativeObject nativeObject)
		{
			if (nativeObject != null)
			{
				this._pointer = nativeObject.Pointer;
				this._constructor = (Func<NativeObject>)Managed.GetConstructorDelegateOfWeakReferenceClass(nativeObject.GetType());
				this._weakReferenceCache = new WeakReference(nativeObject);
			}
		}

		public void ManualInvalidate()
		{
			NativeObject nativeObject = (NativeObject)this._weakReferenceCache.Target;
			if (nativeObject != null)
			{
				nativeObject.ManualInvalidate();
			}
		}

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

		private readonly UIntPtr _pointer;

		private readonly Func<NativeObject> _constructor;

		private readonly WeakReference _weakReferenceCache;
	}
}
