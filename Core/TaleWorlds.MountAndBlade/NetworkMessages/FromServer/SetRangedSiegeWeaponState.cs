using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetRangedSiegeWeaponState : GameNetworkMessage
	{
		public MissionObjectId RangedSiegeWeaponId { get; private set; }

		public RangedSiegeWeapon.WeaponState State { get; private set; }

		public SetRangedSiegeWeaponState(MissionObjectId rangedSiegeWeaponId, RangedSiegeWeapon.WeaponState state)
		{
			this.RangedSiegeWeaponId = rangedSiegeWeaponId;
			this.State = state;
		}

		public SetRangedSiegeWeaponState()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RangedSiegeWeaponId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.State = (RangedSiegeWeapon.WeaponState)GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponStateCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.RangedSiegeWeaponId);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.RangedSiegeWeaponStateCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Set RangedSiegeWeapon State to: ", this.State, " on RangedSiegeWeapon with ID: ", this.RangedSiegeWeaponId });
		}
	}
}
