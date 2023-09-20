using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class BurstMissionObjectParticles : GameNetworkMessage
	{
		public MissionObjectId MissionObjectId { get; private set; }

		public bool DoChildren { get; private set; }

		public BurstMissionObjectParticles(MissionObjectId missionObjectId, bool doChildren)
		{
			this.MissionObjectId = missionObjectId;
			this.DoChildren = doChildren;
		}

		public BurstMissionObjectParticles()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.DoChildren = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.MissionObjectId);
			GameNetworkMessage.WriteBoolToPacket(this.DoChildren);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.Particles;
		}

		protected override string OnGetLogFormat()
		{
			return "Burst MissionObject particles on MissionObject with ID: " + this.MissionObjectId;
		}
	}
}
