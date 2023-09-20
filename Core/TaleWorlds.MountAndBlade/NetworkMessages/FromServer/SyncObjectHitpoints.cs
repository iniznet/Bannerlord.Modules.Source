using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SyncObjectHitpoints : GameNetworkMessage
	{
		public MissionObjectId MissionObjectId { get; private set; }

		public float Hitpoints { get; private set; }

		public SyncObjectHitpoints(MissionObjectId missionObjectId, float hitpoints)
		{
			this.MissionObjectId = missionObjectId;
			this.Hitpoints = hitpoints;
		}

		public SyncObjectHitpoints()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.Hitpoints = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.UsableGameObjectHealthCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.MissionObjectId);
			GameNetworkMessage.WriteFloatToPacket(MathF.Max(this.Hitpoints, 0f), CompressionMission.UsableGameObjectHealthCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Synchronize HitPoints: ", this.Hitpoints, " of MissionObject with Id: ", this.MissionObjectId });
		}
	}
}
