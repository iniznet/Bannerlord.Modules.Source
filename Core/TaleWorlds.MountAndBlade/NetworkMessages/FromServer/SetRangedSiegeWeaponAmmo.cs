using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetRangedSiegeWeaponAmmo : GameNetworkMessage
	{
		public RangedSiegeWeapon RangedSiegeWeapon { get; private set; }

		public int AmmoCount { get; private set; }

		public SetRangedSiegeWeaponAmmo(RangedSiegeWeapon rangedSiegeWeapon, int ammoCount)
		{
			this.RangedSiegeWeapon = rangedSiegeWeapon;
			this.AmmoCount = ammoCount;
		}

		public SetRangedSiegeWeaponAmmo()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RangedSiegeWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as RangedSiegeWeapon;
			this.AmmoCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.RangedSiegeWeapon);
			GameNetworkMessage.WriteIntToPacket(this.AmmoCount, CompressionMission.RangedSiegeWeaponAmmoCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set ammo left to: ",
				this.AmmoCount,
				" on RangedSiegeWeapon with ID: ",
				this.RangedSiegeWeapon.Id,
				" and name: ",
				this.RangedSiegeWeapon.GameEntity.Name
			});
		}
	}
}
