using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000030 RID: 48
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SetMachineRotation : GameNetworkMessage
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000175 RID: 373 RVA: 0x00003B26 File Offset: 0x00001D26
		// (set) Token: 0x06000176 RID: 374 RVA: 0x00003B2E File Offset: 0x00001D2E
		public UsableMachine UsableMachine { get; private set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000177 RID: 375 RVA: 0x00003B37 File Offset: 0x00001D37
		// (set) Token: 0x06000178 RID: 376 RVA: 0x00003B3F File Offset: 0x00001D3F
		public float HorizontalRotation { get; private set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000179 RID: 377 RVA: 0x00003B48 File Offset: 0x00001D48
		// (set) Token: 0x0600017A RID: 378 RVA: 0x00003B50 File Offset: 0x00001D50
		public float VerticalRotation { get; private set; }

		// Token: 0x0600017B RID: 379 RVA: 0x00003B59 File Offset: 0x00001D59
		public SetMachineRotation(UsableMachine missionObject, float horizontalRotation, float verticalRotation)
		{
			this.UsableMachine = missionObject;
			this.HorizontalRotation = horizontalRotation;
			this.VerticalRotation = verticalRotation;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00003B76 File Offset: 0x00001D76
		public SetMachineRotation()
		{
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00003B80 File Offset: 0x00001D80
		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableMachine = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as UsableMachine;
			this.HorizontalRotation = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.HighResRadianCompressionInfo, ref flag);
			this.VerticalRotation = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.HighResRadianCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00003BC6 File Offset: 0x00001DC6
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.UsableMachine);
			GameNetworkMessage.WriteFloatToPacket(this.HorizontalRotation, CompressionBasic.HighResRadianCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.VerticalRotation, CompressionBasic.HighResRadianCompressionInfo);
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00003BF3 File Offset: 0x00001DF3
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00003BFC File Offset: 0x00001DFC
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set rotation of UsableMachine with ID: ",
				this.UsableMachine.Id,
				" and with name: ",
				this.UsableMachine.GameEntity.Name
			});
		}
	}
}
