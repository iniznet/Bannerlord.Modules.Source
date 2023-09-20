using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000097 RID: 151
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectColors : GameNetworkMessage
	{
		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000620 RID: 1568 RVA: 0x0000B585 File Offset: 0x00009785
		// (set) Token: 0x06000621 RID: 1569 RVA: 0x0000B58D File Offset: 0x0000978D
		public MissionObject MissionObject { get; private set; }

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000622 RID: 1570 RVA: 0x0000B596 File Offset: 0x00009796
		// (set) Token: 0x06000623 RID: 1571 RVA: 0x0000B59E File Offset: 0x0000979E
		public uint Color { get; private set; }

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06000624 RID: 1572 RVA: 0x0000B5A7 File Offset: 0x000097A7
		// (set) Token: 0x06000625 RID: 1573 RVA: 0x0000B5AF File Offset: 0x000097AF
		public uint Color2 { get; private set; }

		// Token: 0x06000626 RID: 1574 RVA: 0x0000B5B8 File Offset: 0x000097B8
		public SetMissionObjectColors(MissionObject missionObject, uint color, uint color2)
		{
			this.MissionObject = missionObject;
			this.Color = color;
			this.Color2 = color2;
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x0000B5D5 File Offset: 0x000097D5
		public SetMissionObjectColors()
		{
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x0000B5E0 File Offset: 0x000097E0
		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Color = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref flag);
			this.Color2 = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x0000B621 File Offset: 0x00009821
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteUintToPacket(this.Color, CompressionGeneric.ColorCompressionInfo);
			GameNetworkMessage.WriteUintToPacket(this.Color2, CompressionGeneric.ColorCompressionInfo);
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x0000B64E File Offset: 0x0000984E
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x0000B658 File Offset: 0x00009858
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
