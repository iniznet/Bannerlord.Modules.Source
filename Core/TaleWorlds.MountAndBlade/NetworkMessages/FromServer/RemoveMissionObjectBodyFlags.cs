using System;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000088 RID: 136
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RemoveMissionObjectBodyFlags : GameNetworkMessage
	{
		// Token: 0x17000133 RID: 307
		// (get) Token: 0x0600056F RID: 1391 RVA: 0x0000A365 File Offset: 0x00008565
		// (set) Token: 0x06000570 RID: 1392 RVA: 0x0000A36D File Offset: 0x0000856D
		public MissionObject MissionObject { get; private set; }

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000571 RID: 1393 RVA: 0x0000A376 File Offset: 0x00008576
		// (set) Token: 0x06000572 RID: 1394 RVA: 0x0000A37E File Offset: 0x0000857E
		public BodyFlags BodyFlags { get; private set; }

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06000573 RID: 1395 RVA: 0x0000A387 File Offset: 0x00008587
		// (set) Token: 0x06000574 RID: 1396 RVA: 0x0000A38F File Offset: 0x0000858F
		public bool ApplyToChildren { get; private set; }

		// Token: 0x06000575 RID: 1397 RVA: 0x0000A398 File Offset: 0x00008598
		public RemoveMissionObjectBodyFlags(MissionObject missionObject, BodyFlags bodyFlags, bool applyToChildren)
		{
			this.MissionObject = missionObject;
			this.BodyFlags = bodyFlags;
			this.ApplyToChildren = applyToChildren;
		}

		// Token: 0x06000576 RID: 1398 RVA: 0x0000A3B5 File Offset: 0x000085B5
		public RemoveMissionObjectBodyFlags()
		{
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x0000A3C0 File Offset: 0x000085C0
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.BodyFlags = (BodyFlags)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.FlagsCompressionInfo, ref flag);
			this.ApplyToChildren = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x0000A3FC File Offset: 0x000085FC
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteIntToPacket((int)this.BodyFlags, CompressionBasic.FlagsCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.ApplyToChildren);
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x0000A424 File Offset: 0x00008624
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x0000A42C File Offset: 0x0000862C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Remove bodyflags: ",
				this.BodyFlags,
				" from MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name,
				this.ApplyToChildren ? "" : " and from all of its children."
			});
		}
	}
}
