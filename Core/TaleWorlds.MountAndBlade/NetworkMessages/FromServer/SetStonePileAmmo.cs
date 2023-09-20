using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetStonePileAmmo : GameNetworkMessage
	{
		public MissionObjectId StonePileId { get; private set; }

		public int AmmoCount { get; private set; }

		public SetStonePileAmmo(MissionObjectId stonePileId, int ammoCount)
		{
			this.StonePileId = stonePileId;
			this.AmmoCount = ammoCount;
		}

		public SetStonePileAmmo()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.StonePileId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.AmmoCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.StonePileId);
			GameNetworkMessage.WriteIntToPacket(this.AmmoCount, CompressionMission.RangedSiegeWeaponAmmoCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Set ammo left to: ", this.AmmoCount, " on StonePile with ID: ", this.StonePileId });
		}
	}
}
