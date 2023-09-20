using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectColors : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public uint Color { get; private set; }

		public uint Color2 { get; private set; }

		public SetMissionObjectColors(MissionObject missionObject, uint color, uint color2)
		{
			this.MissionObject = missionObject;
			this.Color = color;
			this.Color2 = color2;
		}

		public SetMissionObjectColors()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Color = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref flag);
			this.Color2 = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteUintToPacket(this.Color, CompressionGeneric.ColorCompressionInfo);
			GameNetworkMessage.WriteUintToPacket(this.Color2, CompressionGeneric.ColorCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Colors of MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
