using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetRangedSiegeWeaponAmmo : GameNetworkMessage
	{
		public MissionObjectId RangedSiegeWeaponId { get; private set; }

		public int AmmoCount { get; private set; }

		public SetRangedSiegeWeaponAmmo(MissionObjectId rangedSiegeWeaponId, int ammoCount)
		{
			this.RangedSiegeWeaponId = rangedSiegeWeaponId;
			this.AmmoCount = ammoCount;
		}

		public SetRangedSiegeWeaponAmmo()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RangedSiegeWeaponId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.AmmoCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.RangedSiegeWeaponId);
			GameNetworkMessage.WriteIntToPacket(this.AmmoCount, CompressionMission.RangedSiegeWeaponAmmoCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Set ammo left to: ", this.AmmoCount, " on RangedSiegeWeapon with ID: ", this.RangedSiegeWeaponId });
		}
	}
}
