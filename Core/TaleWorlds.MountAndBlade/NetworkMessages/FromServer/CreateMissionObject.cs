using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreateMissionObject : GameNetworkMessage
	{
		public MissionObjectId ObjectId { get; private set; }

		public string Prefab { get; private set; }

		public MatrixFrame Frame { get; private set; }

		public List<MissionObjectId> ChildObjectIds { get; private set; }

		public CreateMissionObject(MissionObjectId objectId, string prefab, MatrixFrame frame, List<MissionObjectId> childObjectIds)
		{
			this.ObjectId = objectId;
			this.Prefab = prefab;
			this.Frame = frame;
			this.ChildObjectIds = childObjectIds;
		}

		public CreateMissionObject()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.ObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.Prefab = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.Frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref flag);
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.EntityChildCountCompressionInfo, ref flag);
			if (flag)
			{
				this.ChildObjectIds = new List<MissionObjectId>(num);
				for (int i = 0; i < num; i++)
				{
					if (flag)
					{
						this.ChildObjectIds.Add(GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag));
					}
				}
			}
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.ObjectId);
			GameNetworkMessage.WriteStringToPacket(this.Prefab);
			GameNetworkMessage.WriteMatrixFrameToPacket(this.Frame);
			GameNetworkMessage.WriteIntToPacket(this.ChildObjectIds.Count, CompressionBasic.EntityChildCountCompressionInfo);
			foreach (MissionObjectId missionObjectId in this.ChildObjectIds)
			{
				GameNetworkMessage.WriteMissionObjectIdToPacket(missionObjectId);
			}
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Create a MissionObject with index: ", this.ObjectId, " from prefab: ", this.Prefab, " at frame: ", this.Frame });
		}
	}
}
