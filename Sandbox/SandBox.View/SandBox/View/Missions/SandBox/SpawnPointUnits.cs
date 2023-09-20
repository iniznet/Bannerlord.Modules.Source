using System;

namespace SandBox.View.Missions.SandBox
{
	public class SpawnPointUnits
	{
		public string SpName { get; private set; }

		public SpawnPointUnits.SceneType Place { get; private set; }

		public int MinCount { get; private set; }

		public int MaxCount { get; private set; }

		public string Type { get; private set; }

		public SpawnPointUnits(string sp_name, SpawnPointUnits.SceneType place, int minCount, int maxCount)
		{
			this.SpName = sp_name;
			this.Place = place;
			this.MinCount = minCount;
			this.MaxCount = maxCount;
			this.CurrentCount = 0;
			this.SpawnedAgentCount = 0;
			this.Type = "other";
		}

		public SpawnPointUnits(string sp_name, SpawnPointUnits.SceneType place, string type, int minCount, int maxCount)
		{
			this.SpName = sp_name;
			this.Place = place;
			this.Type = type;
			this.MinCount = minCount;
			this.MaxCount = maxCount;
			this.CurrentCount = 0;
			this.SpawnedAgentCount = 0;
		}

		public int CurrentCount;

		public int SpawnedAgentCount;

		public enum SceneType
		{
			Center,
			Tavern,
			VillageCenter,
			Arena,
			LordsHall,
			Castle,
			Dungeon,
			EmptyShop,
			All,
			NotDetermined
		}
	}
}
