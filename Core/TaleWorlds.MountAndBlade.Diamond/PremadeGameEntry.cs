using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PremadeGameEntry
	{
		public Guid Id { get; private set; }

		public string Name { get; private set; }

		public string Region { get; private set; }

		public string GameType { get; private set; }

		public string MapName { get; private set; }

		public string FactionA { get; private set; }

		public string FactionB { get; private set; }

		public bool IsPasswordProtected { get; private set; }

		public PremadeGameType PremadeGameType { get; private set; }

		public PremadeGameEntry(Guid id, string name, string region, string gameType, string mapName, string factionA, string factionB, bool isPasswordProtected, PremadeGameType premadeGameType)
		{
			this.Id = id;
			this.Name = name;
			this.Region = region;
			this.GameType = gameType;
			this.MapName = mapName;
			this.FactionA = factionA;
			this.FactionB = factionB;
			this.IsPasswordProtected = isPasswordProtected;
			this.PremadeGameType = premadeGameType;
		}
	}
}
