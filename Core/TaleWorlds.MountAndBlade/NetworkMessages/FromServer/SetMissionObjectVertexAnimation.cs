using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectVertexAnimation : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public int BeginKey { get; private set; }

		public int EndKey { get; private set; }

		public float Speed { get; private set; }

		public SetMissionObjectVertexAnimation(MissionObject missionObject, int beginKey, int endKey, float speed)
		{
			this.MissionObject = missionObject;
			this.BeginKey = beginKey;
			this.EndKey = endKey;
			this.Speed = speed;
		}

		public SetMissionObjectVertexAnimation()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.BeginKey = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationKeyCompressionInfo, ref flag);
			this.EndKey = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationKeyCompressionInfo, ref flag);
			this.Speed = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.VertexAnimationSpeedCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteIntToPacket(this.BeginKey, CompressionBasic.AnimationKeyCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.EndKey, CompressionBasic.AnimationKeyCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.Speed, CompressionBasic.VertexAnimationSpeedCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Vertex Animation on MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
