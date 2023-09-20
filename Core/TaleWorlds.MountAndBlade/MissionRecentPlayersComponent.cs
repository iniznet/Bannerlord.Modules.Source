using System;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000294 RID: 660
	public class MissionRecentPlayersComponent : MissionNetwork
	{
		// Token: 0x0600239B RID: 9115 RVA: 0x0008391C File Offset: 0x00081B1C
		public override void AfterStart()
		{
			base.AfterStart();
			MissionPeer.OnTeamChanged += this.TeamChange;
			MissionPeer.OnPlayerKilled += this.OnPlayerKilled;
			this._myId = NetworkMain.GameClient.PlayerID;
		}

		// Token: 0x0600239C RID: 9116 RVA: 0x00083956 File Offset: 0x00081B56
		private void TeamChange(NetworkCommunicator player, Team oldTeam, Team nextTeam)
		{
			if (player.VirtualPlayer.Id != this._myId)
			{
				RecentPlayersManager.AddOrUpdatePlayerEntry(player.VirtualPlayer.Id, player.UserName, InteractionType.InGameTogether, player.ForcedAvatarIndex);
			}
		}

		// Token: 0x0600239D RID: 9117 RVA: 0x00083990 File Offset: 0x00081B90
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

		// Token: 0x0600239E RID: 9118 RVA: 0x00083A40 File Offset: 0x00081C40
		public override void OnRemoveBehavior()
		{
			MissionPeer.OnTeamChanged -= this.TeamChange;
			MissionPeer.OnPlayerKilled -= this.OnPlayerKilled;
			base.OnRemoveBehavior();
		}

		// Token: 0x04000D09 RID: 3337
		private PlayerId _myId;
	}
}
