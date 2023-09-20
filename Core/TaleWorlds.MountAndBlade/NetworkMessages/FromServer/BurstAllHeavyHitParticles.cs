using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class BurstAllHeavyHitParticles : GameNetworkMessage
	{
		public MissionObjectId MissionObjectId { get; private set; }

		public BurstAllHeavyHitParticles(MissionObjectId missionObjectId)
		{
			this.MissionObjectId = missionObjectId;
		}

		public BurstAllHeavyHitParticles()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.MissionObjectId);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Bursting all heavy-hit particles for the DestructableComponent of MissionObject with Id: " + this.MissionObjectId;
		}
	}
}
