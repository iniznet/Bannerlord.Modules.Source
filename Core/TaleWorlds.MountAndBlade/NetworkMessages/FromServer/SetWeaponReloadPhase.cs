using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetWeaponReloadPhase : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public EquipmentIndex EquipmentIndex { get; private set; }

		public short ReloadPhase { get; private set; }

		public SetWeaponReloadPhase(Agent agent, EquipmentIndex equipmentIndex, short reloadPhase)
		{
			this.Agent = agent;
			this.EquipmentIndex = equipmentIndex;
			this.ReloadPhase = reloadPhase;
		}

		public SetWeaponReloadPhase()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.EquipmentIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
			this.ReloadPhase = (short)GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponReloadPhaseCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.EquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.ReloadPhase, CompressionMission.WeaponReloadPhaseCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.EquipmentDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Reload Phase: ",
				this.ReloadPhase,
				" for weapon with EquipmentIndex: ",
				this.EquipmentIndex,
				" on Agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
