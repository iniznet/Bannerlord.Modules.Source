using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000093 RID: 147
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMachineTargetRotation : GameNetworkMessage
	{
		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060005F0 RID: 1520 RVA: 0x0000B02C File Offset: 0x0000922C
		// (set) Token: 0x060005F1 RID: 1521 RVA: 0x0000B034 File Offset: 0x00009234
		public UsableMachine UsableMachine { get; private set; }

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060005F2 RID: 1522 RVA: 0x0000B03D File Offset: 0x0000923D
		// (set) Token: 0x060005F3 RID: 1523 RVA: 0x0000B045 File Offset: 0x00009245
		public float HorizontalRotation { get; private set; }

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060005F4 RID: 1524 RVA: 0x0000B04E File Offset: 0x0000924E
		// (set) Token: 0x060005F5 RID: 1525 RVA: 0x0000B056 File Offset: 0x00009256
		public float VerticalRotation { get; private set; }

		// Token: 0x060005F6 RID: 1526 RVA: 0x0000B05F File Offset: 0x0000925F
		public SetMachineTargetRotation(UsableMachine usableMachine, float horizontalRotaiton, float verticalRotation)
		{
			this.UsableMachine = usableMachine;
			this.HorizontalRotation = horizontalRotaiton;
			this.VerticalRotation = verticalRotation;
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x0000B07C File Offset: 0x0000927C
		public SetMachineTargetRotation()
		{
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x0000B084 File Offset: 0x00009284
		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableMachine = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as UsableMachine;
			this.HorizontalRotation = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.HighResRadianCompressionInfo, ref flag);
			this.VerticalRotation = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.HighResRadianCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x0000B0CA File Offset: 0x000092CA
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.UsableMachine);
			GameNetworkMessage.WriteFloatToPacket(this.HorizontalRotation, CompressionBasic.HighResRadianCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.VerticalRotation, CompressionBasic.HighResRadianCompressionInfo);
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x0000B0F7 File Offset: 0x000092F7
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x0000B100 File Offset: 0x00009300
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set target rotation of UsableMachine with ID: ",
				this.UsableMachine.Id,
				" and with name: ",
				this.UsableMachine.GameEntity.Name
			});
		}
	}
}
