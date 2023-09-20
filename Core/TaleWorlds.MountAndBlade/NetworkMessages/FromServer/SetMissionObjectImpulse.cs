using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectImpulse : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public Vec3 Position { get; private set; }

		public Vec3 Impulse { get; private set; }

		public SetMissionObjectImpulse(MissionObject missionObject, Vec3 position, Vec3 impulse)
		{
			this.MissionObject = missionObject;
			this.Position = position;
			this.Impulse = impulse;
		}

		public SetMissionObjectImpulse()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.LocalPositionCompressionInfo, ref flag);
			this.Impulse = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.ImpulseCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionBasic.LocalPositionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.Impulse, CompressionBasic.ImpulseCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set impulse on MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
