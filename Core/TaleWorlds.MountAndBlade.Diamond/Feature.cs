using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200010B RID: 267
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class Feature : Attribute
	{
		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x060004F6 RID: 1270 RVA: 0x000079A0 File Offset: 0x00005BA0
		// (set) Token: 0x060004F7 RID: 1271 RVA: 0x000079A8 File Offset: 0x00005BA8
		public Features FeatureFlag { get; private set; }

		// Token: 0x060004F8 RID: 1272 RVA: 0x000079B1 File Offset: 0x00005BB1
		public Feature(Features flag)
		{
			this.FeatureFlag = flag;
		}
	}
}
