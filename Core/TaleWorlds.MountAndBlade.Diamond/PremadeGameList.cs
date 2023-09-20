using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PremadeGameList
	{
		public static PremadeGameList Empty { get; private set; } = new PremadeGameList(new PremadeGameEntry[0]);

		public PremadeGameEntry[] PremadeGameEntries { get; private set; }

		public PremadeGameList(PremadeGameEntry[] entries)
		{
			this.PremadeGameEntries = entries;
		}
	}
}
