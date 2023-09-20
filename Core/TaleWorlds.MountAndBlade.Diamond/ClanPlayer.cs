using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class ClanPlayer
	{
		public PlayerId PlayerId { get; private set; }

		public Guid ClanId { get; private set; }

		public ClanPlayerRole Role { get; private set; }

		public ClanPlayer(PlayerId playerId, Guid clanId, ClanPlayerRole role)
		{
			this.PlayerId = playerId;
			this.ClanId = clanId;
			this.Role = role;
		}
	}
}
