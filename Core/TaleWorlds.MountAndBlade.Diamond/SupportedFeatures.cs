using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class SupportedFeatures
	{
		public SupportedFeatures()
		{
			this.Features = -1;
		}

		public SupportedFeatures(int features)
		{
			this.Features = features;
		}

		public bool SupportsFeatures(Features feature)
		{
			return (this.Features & (int)feature) == (int)feature;
		}

		public int Features;
	}
}
