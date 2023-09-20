using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PlayerJoinGameData
	{
		public PlayerData PlayerData { get; }

		public PlayerId PlayerId
		{
			get
			{
				return this.PlayerData.PlayerId;
			}
		}

		public string Name { get; }

		public Guid? PartyId { get; }

		public Dictionary<string, List<string>> UsedCosmetics { get; }

		public string IpAddress { get; }

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
