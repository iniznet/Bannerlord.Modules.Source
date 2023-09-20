using System;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001A9 RID: 425
	[EngineStruct("Deform_Key_Data")]
	public struct DeformKeyData
	{
		// Token: 0x04000749 RID: 1865
		public int GroupId;

		// Token: 0x0400074A RID: 1866
		public int KeyTimePoint;

		// Token: 0x0400074B RID: 1867
		public float Min;

		// Token: 0x0400074C RID: 1868
		public float Max;

		// Token: 0x0400074D RID: 1869
		public float Value;

		// Token: 0x0400074E RID: 1870
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string Id;
	}
}
