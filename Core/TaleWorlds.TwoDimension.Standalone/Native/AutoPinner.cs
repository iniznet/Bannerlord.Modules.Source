using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.TwoDimension.Standalone.Native
{
	internal class AutoPinner : IDisposable
	{
		public AutoPinner(object obj)
		{
			if (obj != null)
			{
				this._pinnedObject = GCHandle.Alloc(obj, GCHandleType.Pinned);
			}
		}

		public static implicit operator IntPtr(AutoPinner autoPinner)
		{
			if (autoPinner._pinnedObject.IsAllocated)
			{
				return autoPinner._pinnedObject.AddrOfPinnedObject();
			}
			return IntPtr.Zero;
		}

		public void Dispose()
		{
			if (this._pinnedObject.IsAllocated)
			{
				this._pinnedObject.Free();
			}
		}

		private GCHandle _pinnedObject;
	}
}
