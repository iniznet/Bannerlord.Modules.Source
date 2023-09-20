using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RangedSiegeWeaponChangeProjectile : GameNetworkMessage
	{
		public RangedSiegeWeapon RangedSiegeWeapon { get; private set; }

		public int Index { get; private set; }

		public RangedSiegeWeaponChangeProjectile(RangedSiegeWeapon rangedSiegeWeapon, int index)
		{
			this.RangedSiegeWeapon = rangedSiegeWeapon;
			this.Index = index;
		}

		public RangedSiegeWeaponChangeProjectile()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RangedSiegeWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as RangedSiegeWeapon;
			this.Index = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.RangedSiegeWeapon);
			GameNetworkMessage.WriteIntToPacket(this.Index, CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Changed Projectile Type Index to: ",
				this.Index,
				" on RangedSiegeWeapon with ID: ",
				this.RangedSiegeWeapon.Id,
				" and name: ",
				this.RangedSiegeWeapon.GameEntity.Name
			});
		}
	}
}
