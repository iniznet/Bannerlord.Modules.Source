using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public class PartyPlayerInLobbyClient
	{
		public PlayerId PlayerId { get; private set; }

		public string Name { get; private set; }

		public bool WaitingInvitation { get; private set; }

		public bool IsPartyLeader { get; private set; }

		public PartyPlayerInLobbyClient(PlayerId playerId, string name, bool isPartyLeader = false)
		{
			this.PlayerId = playerId;
			this.Name = name;
			this.IsPartyLeader = isPartyLeader;
			this.WaitingInvitation = true;
		}

		public void SetAtParty()
		{
			this.WaitingInvitation = false;
		}

		public void SetLeader()
		{
			this.IsPartyLeader = true;
		}

		public void SetMember()
		{
			this.IsPartyLeader = false;
		}
	}
}
