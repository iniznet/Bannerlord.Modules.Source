using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class UnselectSiegeWeapon : GameNetworkMessage
	{
		public MissionObjectId SiegeWeaponId { get; private set; }

		public UnselectSiegeWeapon(MissionObjectId siegeWeaponId)
		{
			this.SiegeWeaponId = siegeWeaponId;
		}

		public UnselectSiegeWeapon()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.SiegeWeaponId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.SiegeWeaponId);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Deselect SiegeWeapon with ID: " + this.SiegeWeaponId;
		}
	}
}
