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

		public MissionObject Parent { get; private set; }

		public MatrixFrame Frame { get; private set; }

		public StopPhysicsAndSetFrameOfMissionObject(MissionObjectId objectId, MissionObject parent, MatrixFrame frame)
		{
			this.ObjectId = objectId;
			this.Parent = parent;
			this.Frame = frame;
		}

		public StopPhysicsAndSetFrameOfMissionObject()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.ObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.Parent = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Frame = GameNetworkMessage.ReadNonUniformTransformFromPacket(CompressionBasic.PositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.ObjectId);
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.Parent);
			GameNetworkMessage.WriteNonUniformTransformToPacket(this.Frame, CompressionBasic.PositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			object[] array = new object[4];
			array[0] = "Stop physics and set frame of MissionObject with ID: ";
			array[1] = this.ObjectId;
			array[2] = " Parent Index: ";
			int num = 3;
			MissionObject parent = this.Parent;
			array[num] = ((parent != null) ? parent.Id.ToString() : null) ?? "-1";
			return string.Concat(array);
		}
	}
}
