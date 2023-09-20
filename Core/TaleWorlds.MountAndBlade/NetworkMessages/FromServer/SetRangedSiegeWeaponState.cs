using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetRangedSiegeWeaponState : GameNetworkMessage
	{
		public RangedSiegeWeapon RangedSiegeWeapon { get; private set; }

		public RangedSiegeWeapon.WeaponState State { get; private set; }

		public SetRangedSiegeWeaponState(RangedSiegeWeapon rangedSiegeWeapon, RangedSiegeWeapon.WeaponState state)
		{
			this.RangedSiegeWeapon = rangedSiegeWeapon;
			this.State = state;
		}

		public SetRangedSiegeWeaponState()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.RangedSiegeWeapon = missionObject as RangedSiegeWeapon;
			this.State = (RangedSiegeWeapon.WeaponState)GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponStateCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.RangedSiegeWeapon);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.RangedSiegeWeaponStateCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set RangedSiegeWeapon State to: ",
				this.State,
				" on RangedSiegeWeapon with ID: ",
				this.RangedSiegeWeapon.Id,
				" and name: ",
				this.RangedSiegeWeapon.GameEntity.Name
			});
		}
	}
}
