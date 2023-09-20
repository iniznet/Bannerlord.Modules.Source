using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000E2 RID: 226
	[Serializable]
	public class AvailableCustomGames
	{
		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000367 RID: 871 RVA: 0x000046B2 File Offset: 0x000028B2
		// (set) Token: 0x06000368 RID: 872 RVA: 0x000046BA File Offset: 0x000028BA
		public List<GameServerEntry> CustomGameServerInfos { get; private set; }

		// Token: 0x06000369 RID: 873 RVA: 0x000046C3 File Offset: 0x000028C3
		public AvailableCustomGames()
		{
			this.CustomGameServerInfos = new List<GameServerEntry>();
		}

		// Token: 0x0600036A RID: 874 RVA: 0x000046D8 File Offset: 0x000028D8
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
