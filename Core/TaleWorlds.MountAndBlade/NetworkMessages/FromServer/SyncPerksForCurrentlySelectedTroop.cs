using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SyncPerksForCurrentlySelectedTroop : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public int[] PerkIndices { get; private set; }

		public SyncPerksForCurrentlySelectedTroop()
		{
		}

		public SyncPerksForCurrentlySelectedTroop(NetworkCommunicator peer, int[] perkIndices)
		{
			this.Peer = peer;
			this.PerkIndices = perkIndices;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			for (int i = 0; i < 3; i++)
			{
				GameNetworkMessage.WriteIntToPacket(this.PerkIndices[i], CompressionMission.PerkIndexCompressionInfo);
			}
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.PerkIndices = new int[3];
			for (int i = 0; i < 3; i++)
			{
				this.PerkIndices[i] = GameNetworkMessage.ReadIntFromPacket(CompressionMission.PerkIndexCompressionInfo, ref flag);
			}
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			string text = "";
			for (int i = 0; i < 3; i++)
			{
				text += string.Format("[{0}]", this.PerkIndices[i]);
			}
			return string.Concat(new string[]
			{
				"Selected perks for ",
				this.Peer.UserName,
				" has been updated as ",
				text,
				"."
			});
		}
	}
}
