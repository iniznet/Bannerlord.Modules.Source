using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000152 RID: 338
	[Serializable]
	public class SupportedFeatures
	{
		// Token: 0x06000870 RID: 2160 RVA: 0x0000E41C File Offset: 0x0000C61C
		public SupportedFeatures()
		{
			this._supportedFeatures = -1;
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x0000E42B File Offset: 0x0000C62B
		public SupportedFeatures(int features)
		{
			this._supportedFeatures = features;
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x0000E43C File Offset: 0x0000C63C
		public bool SupportsFeatures(Features feature)
		{
			return (this._supportedFeatures & (int)feature) == (int)feature;
		}

		// Token: 0x0400046D RID: 1133
		private int _supportedFeatures;
	}
}
