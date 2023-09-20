using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class KillDeathCountChange : GameNetworkMessage
	{
		public NetworkCommunicator VictimPeer { get; private set; }

		public NetworkCommunicator AttackerPeer { get; private set; }

		public int KillCount { get; private set; }

		public int AssistCount { get; private set; }

		public int DeathCount { get; private set; }

		public int Score { get; private set; }

		public KillDeathCountChange(NetworkCommunicator peer, NetworkCommunicator attackerPeer, int killCount, int assistCount, int deathCount, int score)
		{
			this.VictimPeer = peer;
			this.AttackerPeer = attackerPeer;
			this.KillCount = killCount;
			this.AssistCount = assistCount;
			this.DeathCount = deathCount;
			this.Score = score;
		}

		public KillDeathCountChange()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.VictimPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.AttackerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, true);
			this.KillCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.AssistCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.DeathCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.Score = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.ScoreCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.VictimPeer);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.AttackerPeer);
			GameNetworkMessage.WriteIntToPacket(this.KillCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.AssistCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.DeathCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.Score, CompressionMatchmaker.ScoreCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			object[] array = new object[11];
			array[0] = "Kill-Death Count Changed. Peer: ";
			int num = 1;
			NetworkCommunicator victimPeer = this.VictimPeer;
			array[num] = ((victimPeer != null) ? victimPeer.UserName : null) ?? "NULL";
			array[2] = " killed peer: ";
			int num2 = 3;
			NetworkCommunicator attackerPeer = this.AttackerPeer;
			array[num2] = ((attackerPeer != null) ? attackerPeer.UserName : null) ?? "NULL";
			array[4] = " and now has ";
			array[5] = this.KillCount;
			array[6] = " kills, ";
			array[7] = this.AssistCount;
			array[8] = " assists, and ";
			array[9] = this.DeathCount;
			array[10] = " deaths.";
			return string.Concat(array);
		}
	}
}
