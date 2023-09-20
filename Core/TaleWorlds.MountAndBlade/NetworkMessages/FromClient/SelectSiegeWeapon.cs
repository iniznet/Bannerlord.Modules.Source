using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SelectSiegeWeapon : GameNetworkMessage
	{
		public SiegeWeapon SiegeWeapon { get; private set; }

		public SelectSiegeWeapon(SiegeWeapon siegeWeapon)
		{
			this.SiegeWeapon = siegeWeapon;
		}

		public SelectSiegeWeapon()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.SiegeWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as SiegeWeapon;
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SiegeWeapon);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Select SiegeWeapon with ID: ",
				this.SiegeWeapon.Id,
				" and with name: ",
				this.SiegeWeapon.GameEntity.Name
			});
		}
	}
}
