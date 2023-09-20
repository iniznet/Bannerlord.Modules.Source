using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectImpulse : GameNetworkMessage
	{
		public MissionObjectId MissionObjectId { get; private set; }

		public Vec3 Position { get; private set; }

		public Vec3 Impulse { get; private set; }

		public SetMissionObjectImpulse(MissionObjectId missionObjectId, Vec3 position, Vec3 impulse)
		{
			this.MissionObjectId = missionObjectId;
			this.Position = position;
			this.Impulse = impulse;
		}

		public SetMissionObjectImpulse()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.LocalPositionCompressionInfo, ref flag);
			this.Impulse = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.ImpulseCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.MissionObjectId);
			GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionBasic.LocalPositionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.Impulse, CompressionBasic.ImpulseCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			return "Set impulse on MissionObject with ID: " + this.MissionObjectId;
		}
	}
}
