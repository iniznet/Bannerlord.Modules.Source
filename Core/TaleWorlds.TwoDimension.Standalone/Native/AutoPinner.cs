using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.TwoDimension.Standalone.Native
{
	// Token: 0x02000011 RID: 17
	internal class AutoPinner : IDisposable
	{
		// Token: 0x060000E2 RID: 226 RVA: 0x00004D8F File Offset: 0x00002F8F
		public AutoPinner(object obj)
		{
			if (obj != null)
			{
				this._pinnedObject = GCHandle.Alloc(obj, GCHandleType.Pinned);
			}
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00004DA7 File Offset: 0x00002FA7
		public static implicit operator IntPtr(AutoPinner autoPinner)
		{
			if (autoPinner._pinnedObject.IsAllocated)
			{
				return autoPinner._pinnedObject.AddrOfPinnedObject();
			}
			return IntPtr.Zero;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00004DC7 File Offset: 0x00002FC7
		public void Dispose()
		{
			if (this._pinnedObject.IsAllocated)
			{
				this._pinnedObject.Free();
			}
		}

		// Token: 0x04000059 RID: 89
		private GCHandle _pinnedObject;
	}
}
