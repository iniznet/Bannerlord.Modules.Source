using System;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	public class MissionRecentPlayersComponent : MissionNetwork
	{
		public override void AfterStart()
		{
			base.AfterStart();
			MissionPeer.OnTeamChanged += this.TeamChange;
			MissionPeer.OnPlayerKilled += this.OnPlayerKilled;
			this._myId = NetworkMain.GameClient.PlayerID;
		}

		private void TeamChange(NetworkCommunicator player, Team oldTeam, Team nextTeam)
		{
			if (player.VirtualPlayer.Id != this._myId)
			{
				RecentPlayersManager.AddOrUpdatePlayerEntry(player.VirtualPlayer.Id, player.UserName, InteractionType.InGameTogether, player.ForcedAvatarIndex);
			}
		}

		private void OnPlayerKilled(MissionPeer killerPeer, MissionPeer killedPeer)
		{
			if (killerPeer != null && killedPeer != null && killerPeer.Peer != null && killedPeer.Peer != null)
			{
				PlayerId id = killerPeer.Peer.Id;
				PlayerId id2 = killedPeer.Peer.Id;
				if (id == this._myId && id2 != this._myId)
				{
					RecentPlayersManager.AddOrUpdatePlayerEntry(id2, killedPeer.Name, InteractionType.Killed, killedPeer.GetNetworkPeer().ForcedAvatarIndex);
					return;
				}
				if (id2 == this._myId && id != this._myId)
				{
					RecentPlayersManager.AddOrUpdatePlayerEntry(id, killerPeer.Name, InteractionType.KilledBy, killerPeer.GetNetworkPeer().ForcedAvatarIndex);
				}
			}
		}

		public override void OnRemoveBehavior()
		{
			MissionPeer.OnTeamChanged -= this.TeamChange;
			MissionPeer.OnPlayerKilled -= this.OnPlayerKilled;
			base.OnRemoveBehavior();
		}

		private PlayerId _myId;
	}
}
