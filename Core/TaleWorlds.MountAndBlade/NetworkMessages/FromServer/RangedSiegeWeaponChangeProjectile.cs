using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RangedSiegeWeaponChangeProjectile : GameNetworkMessage
	{
		public MissionObjectId RangedSiegeWeaponId { get; private set; }

		public int Index { get; private set; }

		public RangedSiegeWeaponChangeProjectile(MissionObjectId rangedSiegeWeaponId, int index)
		{
			this.RangedSiegeWeaponId = rangedSiegeWeaponId;
			this.Index = index;
		}

		public RangedSiegeWeaponChangeProjectile()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RangedSiegeWeaponId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.Index = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.RangedSiegeWeaponId);
			GameNetworkMessage.WriteIntToPacket(this.Index, CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Changed Projectile Type Index to: ", this.Index, " on RangedSiegeWeapon with ID: ", this.RangedSiegeWeaponId });
		}
	}
}
