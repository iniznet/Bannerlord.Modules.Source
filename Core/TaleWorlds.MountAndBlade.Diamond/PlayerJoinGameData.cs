using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PlayerJoinGameData
	{
		public PlayerData PlayerData { get; set; }

		public PlayerId PlayerId
		{
			get
			{
				return this.PlayerData.PlayerId;
			}
		}

		public string Name { get; set; }

		public Guid? PartyId { get; set; }

		public Dictionary<string, List<string>> UsedCosmetics { get; set; }

		public string IpAddress { get; }

		public PlayerJoinGameData()
		{
		}

		public PlayerJoinGameData(PlayerData playerData, string name, Guid? partyId, Dictionary<string, List<string>> usedCosmetics, string ipAddress)
		{
			this.PlayerData = playerData;
			this.Name = name;
			this.PartyId = partyId;
			this.UsedCosmetics = usedCosmetics;
			this.IpAddress = ipAddress;
		}
	}
}
