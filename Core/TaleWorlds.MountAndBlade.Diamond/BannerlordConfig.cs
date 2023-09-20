using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	internal class BannerlordConfig
	{
		public int AdmittancePercentage { get; private set; }

		public BannerlordConfig(int admittancePercentage)
		{
			this.AdmittancePercentage = admittancePercentage;
		}
	}
}
