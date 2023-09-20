using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000077 RID: 119
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreateMissionObject : GameNetworkMessage
	{
		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060004B0 RID: 1200 RVA: 0x000090D3 File Offset: 0x000072D3
		// (set) Token: 0x060004B1 RID: 1201 RVA: 0x000090DB File Offset: 0x000072DB
		public MissionObjectId ObjectId { get; private set; }

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x000090E4 File Offset: 0x000072E4
		// (set) Token: 0x060004B3 RID: 1203 RVA: 0x000090EC File Offset: 0x000072EC
		public string Prefab { get; private set; }

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060004B4 RID: 1204 RVA: 0x000090F5 File Offset: 0x000072F5
		// (set) Token: 0x060004B5 RID: 1205 RVA: 0x000090FD File Offset: 0x000072FD
		public MatrixFrame Frame { get; private set; }

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060004B6 RID: 1206 RVA: 0x00009106 File Offset: 0x00007306
		// (set) Token: 0x060004B7 RID: 1207 RVA: 0x0000910E File Offset: 0x0000730E
		public List<MissionObjectId> ChildObjectIds { get; private set; }

		// Token: 0x060004B8 RID: 1208 RVA: 0x00009117 File Offset: 0x00007317
		public CreateMissionObject(MissionObjectId objectId, string prefab, MatrixFrame frame, List<MissionObjectId> childObjectIds)
		{
			this.ObjectId = objectId;
			this.Prefab = prefab;
			this.Frame = frame;
			this.ChildObjectIds = childObjectIds;
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x0000913C File Offset: 0x0000733C
		public CreateMissionObject()
		{
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x00009144 File Offset: 0x00007344
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

		// Token: 0x060004BB RID: 1211 RVA: 0x000091B8 File Offset: 0x000073B8
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

		// Token: 0x060004BC RID: 1212 RVA: 0x00009240 File Offset: 0x00007440
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x00009248 File Offset: 0x00007448
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Create a MissionObject with index: ", this.ObjectId, " from prefab: ", this.Prefab, " at frame: ", this.Frame });
		}
	}
}
