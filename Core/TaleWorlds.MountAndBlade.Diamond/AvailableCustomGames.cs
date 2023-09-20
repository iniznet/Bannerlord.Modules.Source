using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class AvailableCustomGames
	{
		[JsonProperty]
		public List<GameServerEntry> CustomGameServerInfos { get; private set; }

		public AvailableCustomGames()
		{
			this.CustomGameServerInfos = new List<GameServerEntry>();
		}

		public AvailableCustomGames GetCustomGamesByPermission(int playerPermission)
		{
			AvailableCustomGames availableCustomGames = new AvailableCustomGames();
			foreach (GameServerEntry gameServerEntry in this.CustomGameServerInfos)
			{
				if (gameServerEntry.Permission <= playerPermission)
				{
					availableCustomGames.CustomGameServerInfos.Add(gameServerEntry);
				}
			}
			return availableCustomGames;
		}
	}
}
