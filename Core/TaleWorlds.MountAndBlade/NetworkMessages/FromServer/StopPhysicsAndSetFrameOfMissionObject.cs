using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class StopPhysicsAndSetFrameOfMissionObject : GameNetworkMessage
	{
		public MissionObjectId ObjectId { get; private set; }

		public MissionObjectId ParentId { get; private set; }

		public MatrixFrame Frame { get; private set; }

		public StopPhysicsAndSetFrameOfMissionObject(MissionObjectId objectId, MissionObjectId parentId, MatrixFrame frame)
		{
			this.ObjectId = objectId;
			this.ParentId = parentId;
			this.Frame = frame;
		}

		public StopPhysicsAndSetFrameOfMissionObject()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.ObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.ParentId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.Frame = GameNetworkMessage.ReadNonUniformTransformFromPacket(CompressionBasic.PositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.ObjectId);
			GameNetworkMessage.WriteMissionObjectIdToPacket((this.ParentId.Id >= 0) ? this.ParentId : MissionObjectId.Invalid);
			GameNetworkMessage.WriteNonUniformTransformToPacket(this.Frame, CompressionBasic.PositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Stop physics and set frame of MissionObject with ID: ", this.ObjectId, " Parent Index: ", this.ParentId });
		}
	}
}
