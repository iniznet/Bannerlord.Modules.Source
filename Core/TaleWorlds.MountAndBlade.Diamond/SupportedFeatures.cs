using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class SupportedFeatures
	{
		public SupportedFeatures()
		{
			this._supportedFeatures = -1;
		}

		public SupportedFeatures(int features)
		{
			this._supportedFeatures = features;
		}

		public bool SupportsFeatures(Features feature)
		{
			return (this._supportedFeatures & (int)feature) == (int)feature;
		}

		private int _supportedFeatures;
	}
}
