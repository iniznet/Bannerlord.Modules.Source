using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000092 RID: 146
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetBoundariesState : GameNetworkMessage
	{
		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060005E5 RID: 1509 RVA: 0x0000AF62 File Offset: 0x00009162
		// (set) Token: 0x060005E6 RID: 1510 RVA: 0x0000AF6A File Offset: 0x0000916A
		public bool IsOutside { get; private set; }

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060005E7 RID: 1511 RVA: 0x0000AF73 File Offset: 0x00009173
		// (set) Token: 0x060005E8 RID: 1512 RVA: 0x0000AF7B File Offset: 0x0000917B
		public float StateStartTimeInSeconds { get; private set; }

		// Token: 0x060005E9 RID: 1513 RVA: 0x0000AF84 File Offset: 0x00009184
		public SetBoundariesState()
		{
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x0000AF8C File Offset: 0x0000918C
		public SetBoundariesState(bool isOutside)
		{
			this.IsOutside = isOutside;
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x0000AF9B File Offset: 0x0000919B
		public SetBoundariesState(bool isOutside, long stateStartTimeInTicks)
			: this(isOutside)
		{
			this.StateStartTimeInSeconds = (float)stateStartTimeInTicks / 10000000f;
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x0000AFB2 File Offset: 0x000091B2
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.IsOutside);
			if (this.IsOutside)
			{
				GameNetworkMessage.WriteFloatToPacket(this.StateStartTimeInSeconds, CompressionMatchmaker.MissionTimeCompressionInfo);
			}
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x0000AFD8 File Offset: 0x000091D8
		protected override bool OnRead()
		{
			bool flag = true;
			this.IsOutside = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			if (this.IsOutside)
			{
				this.StateStartTimeInSeconds = GameNetworkMessage.ReadFloatFromPacket(CompressionMatchmaker.MissionTimeCompressionInfo, ref flag);
			}
			return flag;
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x0000B00F File Offset: 0x0000920F
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x0000B017 File Offset: 0x00009217
		protected override string OnGetLogFormat()
		{
			if (!this.IsOutside)
			{
				return "I am now inside the level boundaries";
			}
			return "I am now outside of the level boundaries";
		}
	}
}
