using System;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000061 RID: 97
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AddMissionObjectBodyFlags : GameNetworkMessage
	{
		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000366 RID: 870 RVA: 0x000069F0 File Offset: 0x00004BF0
		// (set) Token: 0x06000367 RID: 871 RVA: 0x000069F8 File Offset: 0x00004BF8
		public MissionObject MissionObject { get; private set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000368 RID: 872 RVA: 0x00006A01 File Offset: 0x00004C01
		// (set) Token: 0x06000369 RID: 873 RVA: 0x00006A09 File Offset: 0x00004C09
		public BodyFlags BodyFlags { get; private set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600036A RID: 874 RVA: 0x00006A12 File Offset: 0x00004C12
		// (set) Token: 0x0600036B RID: 875 RVA: 0x00006A1A File Offset: 0x00004C1A
		public bool ApplyToChildren { get; private set; }

		// Token: 0x0600036C RID: 876 RVA: 0x00006A23 File Offset: 0x00004C23
		public AddMissionObjectBodyFlags(MissionObject missionObject, BodyFlags bodyFlags, bool applyToChildren)
		{
			this.MissionObject = missionObject;
			this.BodyFlags = bodyFlags;
			this.ApplyToChildren = applyToChildren;
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00006A40 File Offset: 0x00004C40
		public AddMissionObjectBodyFlags()
		{
		}

		// Token: 0x0600036E RID: 878 RVA: 0x00006A48 File Offset: 0x00004C48
		protected override bool OnRead()
		{
			bool flag = true;
			GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.BodyFlags = (BodyFlags)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.FlagsCompressionInfo, ref flag);
			this.ApplyToChildren = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x0600036F RID: 879 RVA: 0x00006A7F File Offset: 0x00004C7F
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteIntToPacket((int)this.BodyFlags, CompressionBasic.FlagsCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.ApplyToChildren);
		}

		// Token: 0x06000370 RID: 880 RVA: 0x00006AA7 File Offset: 0x00004CA7
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x06000371 RID: 881 RVA: 0x00006AB0 File Offset: 0x00004CB0
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Add bodyflags: ",
				this.BodyFlags,
				" to MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name,
				this.ApplyToChildren ? "" : " and to all of its children."
			});
		}
	}
}
