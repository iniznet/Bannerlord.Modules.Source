using System;

namespace TaleWorlds.DotNet
{
	public sealed class WeakNativeObjectReference<T> where T : NativeObject
	{
		public WeakNativeObjectReference(T nativeObject)
		{
			if (nativeObject != null)
			{
				this._pointer = nativeObject.Pointer;
				this._weakReferenceCache = new WeakReference<T>(nativeObject);
			}
		}

		public void ManualInvalidate()
		{
			T t;
			if (this._weakReferenceCache.TryGetTarget(out t) && t != null)
			{
				t.ManualInvalidate();
			}
		}

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

		private readonly UIntPtr _pointer;

		private WeakReference<T> _weakReferenceCache;
	}
}
